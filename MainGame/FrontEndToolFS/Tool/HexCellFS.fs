namespace FrontEndToolFS.Tool

open System
open System.IO
open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open FrontEndToolFS.HexPlane.HexFlags
open Godot

type IChunk =
    abstract member Refresh: unit -> unit

type IUnit =
    abstract member ValidateLocation: unit -> unit
    abstract member Die: unit -> unit

type IGridForCell =
    abstract member GetCell: HexCoordinates -> HexCellFS option
    abstract member ShaderData: HexCellShaderData
    abstract member CellData: HexCellData array
    abstract member CellPositions: Vector3 array

and HexCellFS() as this =

    interface ICell with
        override this.Index = this.Index
        override this.TerrainTypeIndex = this.TerrainTypeIndex
        override this.IsExplored = this.IsExplored
        override this.IsUnderWater = this.IsUnderWater

    member this.Coordinates = this.Grid.CellData[this.Index].coordinates
    member this.Position = this.Grid.CellPositions[this.Index]
    member val Chunk: IChunk option = None with get, set

    [<DefaultValue>]
    val mutable uiRect: HexCellLabelFS

    [<DefaultValue>]
    val mutable Grid: IGridForCell
    // 标志
    member this.Flags
        with get () = this.Grid.CellData[this.Index].flags
        and set value = this.Grid.CellData[this.Index].flags <- value
    // 值
    member this.Values
        with get () = this.Grid.CellData[this.Index].values
        and set value = this.Grid.CellData[this.Index].values <- value

    member this.GetNeighbor(direction: HexDirection) =
        this.Grid.GetCell <| this.Coordinates.Step direction

    member this.TerrainTypeIndex
        with get () = this.Values.TerrainTypeIndex
        and set value =
            if this.Values.TerrainTypeIndex <> value then
                this.Values <- this.Values.WithTerrainTypeIndex value
                this.Grid.ShaderData.RefreshTerrain this.Index

    let refresh () =
        if this.Chunk.IsSome then
            this.Chunk.Value.Refresh()

            allHexDirs ()
            |> List.map this.GetNeighbor
            |> List.filter (fun (n: HexCellFS option) -> n.IsSome && n.Value.Chunk <> this.Chunk)
            |> List.iter _.Value.Chunk.Value.Refresh()

            this.Unit |> Option.iter _.ValidateLocation()

    let refreshPosition () =
        let pos = this.Position
        let y = float32 this.Elevation * HexMetrics.elevationStep

        let perturbY =
            ((HexMetrics.sampleNoise pos).Y * 2f - 1f) * HexMetrics.elevationPerturbStrength

        this.Grid.CellPositions[this.Index] <- Vector3(pos.X, y + perturbY, pos.Z)
        this.uiRect.Position <- this.Position + Vector3.Up * 0.01f

    member this.RefreshAll() =
        refreshPosition()
        this.Grid.ShaderData.RefreshTerrain this.Index
        this.Grid.ShaderData.RefreshVisibility this.Index

    /// 高度
    member this.Elevation
        with get (): int = this.Values.Elevation
        and set value =
            if this.Values.Elevation = value then
                ()
            else
                this.Values <- this.Values.WithElevation value
                this.Grid.ShaderData.ViewElevationChanged this.Index
                refreshPosition ()
                this.ValidateRivers()

                allHexDirs ()
                |> List.filter (fun d -> this.Flags.HasRoad d && getElevationDifference d > 1)
                |> List.iter this.RemoveRoad

                refresh ()

    /// 获取某个方向上的高度差
    let getElevationDifference (direction: HexDirection) =
        match this.GetNeighbor direction with
        | Some n -> this.Elevation - n.Elevation |> Mathf.Abs
        | None -> Int32.MaxValue

    member this.GetEdgeType(otherCell: HexCellFS) =
        HexMetrics.getEdgeType this.Elevation otherCell.Elevation

    member this.IncomingRiver = this.Flags.RiverInDirection
    member this.OutgoingRiver = this.Flags.RiverOutDirection
    member this.HasIncomingRiver = this.Flags.HasAny HexFlags.RiverIn
    member this.HasOutgoingRiver = this.Flags.HasAny HexFlags.RiverOut
    member this.HasRiver = this.Flags.HasAny HexFlags.River

    member this.HasRiverThroughEdge(direction: HexDirection) =
        this.Flags.HasRiverIn direction || this.Flags.HasRiverOut direction

    /// 移除流出河流
    member this.RemoveOutgoingRiver() =
        if this.HasOutgoingRiver then
            match this.GetNeighbor this.OutgoingRiver.Value with
            | Some neighbor ->
                this.Flags <- this.Flags.Without HexFlags.RiverOut
                neighbor.Flags <- neighbor.Flags.Without HexFlags.RiverIn
                neighbor.Chunk |> Option.iter _.Refresh()
                this.Chunk |> Option.iter _.Refresh()
            | None -> ()

    /// 移除流入河流
    member this.RemoveIncomingRiver() =
        if this.HasIncomingRiver then
            match this.GetNeighbor this.IncomingRiver.Value with
            | Some neighbor ->
                this.Flags <- this.Flags.Without HexFlags.RiverIn
                neighbor.Flags <- neighbor.Flags.Without HexFlags.RiverOut
                neighbor.Chunk |> Option.iter _.Refresh()
                this.Chunk |> Option.iter _.Refresh()
            | None -> ()

    /// 移除河流
    member this.RemoveRiver() =
        this.RemoveIncomingRiver()
        this.RemoveOutgoingRiver()

    member this.SetOutgoingRiver(direction: HexDirection) =
        if this.HasOutgoingRiver && this.OutgoingRiver.Value = direction then
            GD.Print $"SetOutgoingRiver already river {direction}"
            ()
        else
            let neighborOpt = this.GetNeighbor direction

            if this.IsValidRiverDestination neighborOpt then
                let neighbor = neighborOpt.Value
                // GD.Print $"SetOutgoingRiver refresh {direction}"
                this.RemoveOutgoingRiver()

                if this.Flags.HasRiverIn direction then
                    this.RemoveIncomingRiver()

                this.Flags <- this.Flags.WithRiverOut direction
                this.SpecialIndex <- 0
                neighbor.RemoveIncomingRiver()
                neighbor.Flags <- neighbor.Flags.WithRiverIn direction.Opposite
                neighbor.SpecialIndex <- 0
                this.RemoveRoad direction
            else
                GD.Print $"SetOutgoingRiver no neighbor or low {direction}"
                ()

    /// 某个方向上是否有道路
    member this.HasRoadThroughEdge(direction: HexDirection) = this.Flags.HasRoad direction

    member this.RemoveRoad(direction: HexDirection) =
        this.Flags <- this.Flags.WithoutRoad direction

        match this.GetNeighbor direction with
        | Some neighbor ->
            neighbor.Flags <- neighbor.Flags.WithoutRoad direction.Opposite
            neighbor.Chunk |> Option.iter _.Refresh()
            this.Chunk |> Option.iter _.Refresh()
        | None -> ()

    /// 移除道路
    member this.RemoveRoads() =
        allHexDirs () |> List.filter this.Flags.HasRoad |> List.iter this.RemoveRoad

    /// 添加道路
    member this.AddRoad(direction: HexDirection) =
        if
            not <| this.Flags.HasRoad direction
            && not <| this.HasRiverThroughEdge direction
            && not this.IsSpecial
            // AddRoad 的入口保证这里一定有邻居
            && (this.GetNeighbor direction)
               |> Option.map _.IsSpecial
               |> Option.defaultValue true
               |> not
            && getElevationDifference direction <= 1
        then
            this.Flags <- this.Flags.WithRoad direction

            match this.GetNeighbor direction with
            | Some neighbor ->
                neighbor.Flags <- neighbor.Flags.WithRoad direction.Opposite
                neighbor.Chunk |> Option.iter _.Refresh()
                this.Chunk |> Option.iter _.Refresh()
            | None -> ()

    /// 水位
    member this.WaterLevel
        with get () = this.Values.WaterLevel
        and set value =
            if this.Values.WaterLevel = value then
                ()
            else
                this.Values <- this.Values.WithWaterLevel value
                this.Grid.ShaderData.ViewElevationChanged this.Index
                this.ValidateRivers()
                refresh ()

    /// 是否在水下
    member this.IsUnderWater = this.WaterLevel > this.Elevation

    /// 判断某个邻居是否可以作为河流目的地
    member this.IsValidRiverDestination(neighborOpt: HexCellFS option) =
        neighborOpt.IsSome
        && (this.Elevation >= neighborOpt.Value.Elevation
            || this.WaterLevel = neighborOpt.Value.Elevation)

    member this.ValidateRivers() =
        if
            this.HasOutgoingRiver
            && not << this.IsValidRiverDestination <| this.GetNeighbor this.OutgoingRiver.Value
        then
            this.RemoveOutgoingRiver()

        if
            this.HasIncomingRiver
            && not << (this.GetNeighbor this.IncomingRiver.Value).Value.IsValidRiverDestination
               <| Some this
        then
            this.RemoveIncomingRiver()

    // 城市级别
    member this.UrbanLevel
        with get () = this.Values.UrbanLevel
        and set value =
            if this.Values.UrbanLevel <> value then
                this.Values <- this.Values.WithUrbanLevel value
                this.Chunk |> Option.iter _.Refresh()

    // 农场级别
    member this.FarmLevel
        with get () = this.Values.FarmLevel
        and set value =
            if this.Values.FarmLevel <> value then
                this.Values <- this.Values.WithFarmLevel value
                this.Chunk |> Option.iter _.Refresh()

    // 植物级别
    member this.PlantLevel
        with get () = this.Values.PlantLevel
        and set value =
            if this.Values.PlantLevel <> value then
                this.Values <- this.Values.WithPlantLevel value
                this.Chunk |> Option.iter _.Refresh()

    // 围墙
    member this.Walled
        with get () = this.Flags.HasAny HexFlags.Walled
        and set value =
            let newFlags =
                if value then
                    this.Flags.With HexFlags.Walled
                else
                    this.Flags.Without HexFlags.Walled

            if this.Flags <> newFlags then
                this.Flags <- newFlags
                refresh ()

    // 特殊特征
    member this.SpecialIndex
        with get () = this.Values.SpecialIndex
        and set value =
            if this.Values.SpecialIndex <> value && (not this.HasRiver || value = 0) then
                this.Values <- this.Values.WithSpecialIndex value

                if value <> 0 then
                    this.RemoveRoads()

                this.Chunk |> Option.iter _.Refresh()

    member this.IsSpecial = this.SpecialIndex > 0

    // 保存和加载
    member this.Save(writer: BinaryWriter) =
        this.Values.Save(writer)
        this.Flags.Save writer

    member this.Load (reader: BinaryReader) header =
        this.Values <- HexValues.Load reader header
        this.Flags <- this.Flags.Load reader header
        refreshPosition ()
        this.Grid.ShaderData.RefreshTerrain this.Index
        this.Grid.ShaderData.RefreshVisibility this.Index
    // 突出显示
    member this.DisableHighlight() =
        let highlight = this.uiRect.GetNode<Sprite3D> "Highlight"
        highlight.Visible <- false

    member this.EnableHighlight color =
        let highlight = this.uiRect.GetNode<Sprite3D> "Highlight"
        highlight.Modulate <- color
        highlight.Visible <- true
    // 修改标签
    member this.SetLabel text =
        let label = this.uiRect
        label.Text <- text
    // 单位
    member val Unit: IUnit option = None with get, set
    // 索引
    [<DefaultValue>]
    val mutable Index: int
    // 探索
    member this.IsExplored: bool =
        this.Flags.HasAll(HexFlags.Explored ||| HexFlags.Explorable)

    member this.MarkAsExplored() =
        this.Flags <- this.Flags.With HexFlags.Explored

    member this.Explorable
        with get () = this.Flags.HasAny HexFlags.Explorable
        and set value =
            this.Flags <-
                if value then
                    this.Flags.With HexFlags.Explorable
                else
                    this.Flags.Without HexFlags.Explorable
    // 视野高度
    member this.ViewElevation =
        if this.Elevation >= this.WaterLevel then
            this.Elevation
        else
            this.WaterLevel

    // member this.SetMapData data =
    //     // GD.Print $"Setting {this.Coordinates} map data {data}"
    //     this.Grid.ShaderData.SetMapData this data
    // 包覆
    member val ColumnIndex = 0 with get, set
