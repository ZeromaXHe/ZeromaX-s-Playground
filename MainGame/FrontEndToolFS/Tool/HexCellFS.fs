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

and HexCellFS() as this =

    interface ICell with
        override this.Index = this.Index
        override this.TerrainTypeIndex = this.TerrainTypeIndex
        override this.IsVisible = this.IsVisible
        override this.IsExplored = this.IsExplored
        override this.IsUnderWater = this.IsUnderWater
        override this.WaterSurfaceY = this.WaterSurfaceY

    [<DefaultValue>]
    val mutable Coordinates: HexCoordinates

    member val Position = Vector3.Zero with get, set
    member val Chunk: IChunk option = None with get, set

    [<DefaultValue>]
    val mutable uiRect: HexCellLabelFS

    [<DefaultValue>]
    val mutable Grid: IGridForCell
    // 标志
    member val flags: HexFlags = HexFlags.Empty with get, set
    // 值
    member val values = HexValues() with get, set

    member this.GetNeighbor(direction: HexDirection) =
        this.Grid.GetCell <| this.Coordinates.Step direction

    let refresh () =
        if this.Chunk.IsSome then
            this.Chunk.Value.Refresh()

            allHexDirs ()
            |> List.map this.GetNeighbor
            |> List.filter (fun (n: HexCellFS option) -> n.IsSome && n.Value.Chunk <> this.Chunk)
            |> List.iter _.Value.Chunk.Value.Refresh()

            this.Unit |> Option.iter _.ValidateLocation()

    member this.TerrainTypeIndex
        with get () = this.values.TerrainTypeIndex
        and set value =
            if this.values.TerrainTypeIndex <> value then
                this.values <- this.values.WithTerrainTypeIndex value
                this.Grid.ShaderData.RefreshTerrain this

    let refreshPosition () =
        let pos = this.Position
        let y = float32 this.Elevation * HexMetrics.elevationStep

        let perturbY =
            ((HexMetrics.sampleNoise pos).Y * 2f - 1f) * HexMetrics.elevationPerturbStrength

        this.Position <- Vector3(pos.X, y + perturbY, pos.Z)
        this.uiRect.Position <- this.Position + Vector3.Up * 0.01f

    /// 高度
    member this.Elevation
        with get (): int = this.values.Elevation
        and set value =
            if this.values.Elevation = value then
                ()
            else
                this.values <- this.values.WithElevation value
                this.Grid.ShaderData.ViewElevationChanged this
                refreshPosition ()
                this.ValidateRivers()

                allHexDirs ()
                |> List.filter (fun d -> this.flags.HasRoad d && getElevationDifference d > 1)
                |> List.iter this.RemoveRoad

                refresh ()

    /// 获取某个方向上的高度差
    let getElevationDifference (direction: HexDirection) =
        match this.GetNeighbor direction with
        | Some n -> this.Elevation - n.Elevation |> Mathf.Abs
        | None -> Int32.MaxValue

    member this.GetEdgeType(otherCell: HexCellFS) =
        HexMetrics.getEdgeType this.Elevation otherCell.Elevation

    member this.IncomingRiver = this.flags.RiverInDirection
    member this.OutgoingRiver = this.flags.RiverOutDirection
    member this.HasIncomingRiver = this.flags.HasAny HexFlags.RiverIn
    member this.HasOutgoingRiver = this.flags.HasAny HexFlags.RiverOut
    member this.HasRiver = this.flags.HasAny HexFlags.River
    member this.HasRiverBeginOrEnd = this.HasIncomingRiver <> this.HasOutgoingRiver

    member this.StreamBedY =
        (float32 this.Elevation + HexMetrics.streamBedElevationOffset)
        * HexMetrics.elevationStep

    member this.RiverSurfaceY =
        (float32 this.Elevation + HexMetrics.waterElevationOffset)
        * HexMetrics.elevationStep

    member this.HasRiverThroughEdge(direction: HexDirection) =
        this.flags.HasRiverIn direction || this.flags.HasRiverOut direction

    member this.HasIncomingRiverThroughEdge(direction: HexDirection) = this.flags.HasRiverIn direction

    /// 移除流出河流
    member this.RemoveOutgoingRiver() =
        if this.HasOutgoingRiver then
            match this.GetNeighbor this.OutgoingRiver.Value with
            | Some neighbor ->
                this.flags <- this.flags.Without HexFlags.RiverOut
                neighbor.flags <- neighbor.flags.Without HexFlags.RiverIn
                neighbor.Chunk |> Option.iter _.Refresh()
                this.Chunk |> Option.iter _.Refresh()
            | None -> ()

    /// 移除流入河流
    member this.RemoveIncomingRiver() =
        if this.HasIncomingRiver then
            match this.GetNeighbor this.IncomingRiver.Value with
            | Some neighbor ->
                this.flags <- this.flags.Without HexFlags.RiverIn
                neighbor.flags <- neighbor.flags.Without HexFlags.RiverOut
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

                if this.flags.HasRiverIn direction then
                    this.RemoveIncomingRiver()

                this.flags <- this.flags.WithRiverOut direction
                this.SpecialIndex <- 0
                neighbor.RemoveIncomingRiver()
                neighbor.flags <- neighbor.flags.WithRiverIn direction.Opposite
                neighbor.SpecialIndex <- 0
                this.RemoveRoad direction
            else
                GD.Print $"SetOutgoingRiver no neighbor or low {direction}"
                ()

    /// 某个方向上是否有道路
    member this.HasRoadThroughEdge(direction: HexDirection) = this.flags.HasRoad direction
    /// 是否至少有一条道路
    member this.HasRoads = this.flags.HasAny HexFlags.Roads

    member this.RemoveRoad(direction: HexDirection) =
        this.flags <- this.flags.WithoutRoad direction

        match this.GetNeighbor direction with
        | Some neighbor ->
            neighbor.flags <- neighbor.flags.WithoutRoad direction.Opposite
            neighbor.Chunk |> Option.iter _.Refresh()
            this.Chunk |> Option.iter _.Refresh()
        | None -> ()

    /// 移除道路
    member this.RemoveRoads() =
        allHexDirs () |> List.filter this.flags.HasRoad |> List.iter this.RemoveRoad

    /// 添加道路
    member this.AddRoad(direction: HexDirection) =
        if
            not <| this.flags.HasRoad direction
            && not <| this.HasRiverThroughEdge direction
            && not this.IsSpecial
            // AddRoad 的入口保证这里一定有邻居
            && (this.GetNeighbor direction)
               |> Option.map _.IsSpecial
               |> Option.defaultValue true
               |> not
            && getElevationDifference direction <= 1
        then
            this.flags <- this.flags.WithRoad direction

            match this.GetNeighbor direction with
            | Some neighbor ->
                neighbor.flags <- neighbor.flags.WithRoad direction.Opposite
                neighbor.Chunk |> Option.iter _.Refresh()
                this.Chunk |> Option.iter _.Refresh()
            | None -> ()

    /// 水位
    member this.WaterLevel
        with get () = this.values.WaterLevel
        and set value =
            if this.values.WaterLevel = value then
                ()
            else
                this.values <- this.values.WithWaterLevel value
                this.Grid.ShaderData.ViewElevationChanged this
                this.ValidateRivers()
                refresh ()

    /// 是否在水下
    member this.IsUnderWater = this.WaterLevel > this.Elevation

    member this.WaterSurfaceY =
        (float32 this.WaterLevel + HexMetrics.waterElevationOffset)
        * HexMetrics.elevationStep

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
        with get () = this.values.UrbanLevel
        and set value =
            if this.values.UrbanLevel <> value then
                this.values <- this.values.WithUrbanLevel value
                this.Chunk |> Option.iter _.Refresh()

    // 农场级别
    member this.FarmLevel
        with get () = this.values.FarmLevel
        and set value =
            if this.values.FarmLevel <> value then
                this.values <- this.values.WithFarmLevel value
                this.Chunk |> Option.iter _.Refresh()

    // 植物级别
    member this.PlantLevel
        with get () = this.values.PlantLevel
        and set value =
            if this.values.PlantLevel <> value then
                this.values <- this.values.WithPlantLevel value
                this.Chunk |> Option.iter _.Refresh()

    // 围墙
    member this.Walled
        with get () = this.flags.HasAny HexFlags.Walled
        and set value =
            let newFlags =
                if value then
                    this.flags.With HexFlags.Walled
                else
                    this.flags.Without HexFlags.Walled

            if this.flags <> newFlags then
                this.flags <- newFlags
                refresh ()

    // 特殊特征
    member this.SpecialIndex
        with get () = this.values.SpecialIndex
        and set value =
            if this.values.SpecialIndex <> value && (not this.HasRiver || value = 0) then
                this.values <- this.values.WithSpecialIndex value

                if value <> 0 then
                    this.RemoveRoads()

                this.Chunk |> Option.iter _.Refresh()

    member this.IsSpecial = this.SpecialIndex > 0

    // 保存和加载
    member this.Save(writer: BinaryWriter) =
        this.values.Save(writer)
        this.flags.Save writer

    member this.Load (reader: BinaryReader) header =
        this.values <- HexValues.Load reader header
        this.flags <- this.flags.Load reader header
        refreshPosition ()
        this.Grid.ShaderData.RefreshTerrain this
        this.Grid.ShaderData.RefreshVisibility this
    // 距离
    let mutable distance = 0

    member this.Distance
        with get () = distance
        and set value = distance <- value
    // 突出显示
    member this.DisableHighlight() =
        let highlight = this.uiRect.GetNode<Sprite3D> "Highlight"
        highlight.Visible <- false

    member this.EnableHighlight color =
        let highlight = this.uiRect.GetNode<Sprite3D> "Highlight"
        highlight.Modulate <- color
        highlight.Visible <- true

    [<DefaultValue>]
    val mutable PathFromIndex: int

    member val SearchHeuristic = 0 with get, set
    member this.SearchPriority = distance + this.SearchHeuristic
    member val NextWithSamePriority: HexCellFS option = None with get, set
    // 修改标签
    member this.SetLabel text =
        let label = this.uiRect
        label.Text <- text
    // 搜索阶段
    member val SearchPhase = 0 with get, set
    // 单位
    member val Unit: IUnit option = None with get, set
    // 索引
    [<DefaultValue>]
    val mutable Index: int
    // 可见性
    let mutable visibility = 0
    member this.IsVisible = visibility > 0 && this.Explorable

    member this.IncreaseVisibility() =
        visibility <- visibility + 1

        if visibility = 1 then
            this.flags <- this.flags.With HexFlags.Explored
            this.Grid.ShaderData.RefreshVisibility this

    member this.DecreaseVisibility() =
        visibility <- visibility - 1

        if visibility = 0 then
            this.Grid.ShaderData.RefreshVisibility this
    // 探索
    member this.IsExplored: bool =
        this.flags.HasAll(HexFlags.Explored ||| HexFlags.Explorable)

    member this.Explorable
        with get () = this.flags.HasAny HexFlags.Explorable
        and set value =
            this.flags <-
                if value then
                    this.flags.With HexFlags.Explorable
                else
                    this.flags.Without HexFlags.Explorable
    // 视野高度
    member this.ViewElevation =
        if this.Elevation >= this.WaterLevel then
            this.Elevation
        else
            this.WaterLevel

    member this.ResetVisibility() =
        if visibility > 0 then
            visibility <- 0
            this.Grid.ShaderData.RefreshVisibility this

    // member this.SetMapData data =
    //     // GD.Print $"Setting {this.Coordinates} map data {data}"
    //     this.Grid.ShaderData.SetMapData this data
    // 包覆
    member val ColumnIndex = 0 with get, set
