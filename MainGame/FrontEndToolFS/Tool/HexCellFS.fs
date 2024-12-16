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

and HexCellFS() as this =
    inherit Node3D()

    interface ICell with
        override this.Index = this.Index
        override this.TerrainTypeIndex = this.TerrainTypeIndex
        override this.IsVisible = this.IsVisible
        override this.IsExplored = this.IsExplored
        override this.IsUnderWater = this.IsUnderWater
        override this.WaterSurfaceY = this.WaterSurfaceY

    [<DefaultValue>]
    val mutable Coordinates: HexCoordinates

    member val Chunk: IChunk option = None with get, set

    [<DefaultValue>]
    val mutable uiRect: HexCellLabelFS

    [<DefaultValue>]
    val mutable Grid: IGridForCell
    // 标志
    member val flags: HexFlags = HexFlags.Empty with get, set

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

    member this.RefreshSelfOnly() =
        this.Chunk |> Option.iter _.Refresh()
        this.Unit |> Option.iter _.ValidateLocation()

    let mutable terrainTypeIndex = 0

    member this.TerrainTypeIndex
        with get () = terrainTypeIndex
        and set value =
            if terrainTypeIndex <> value then
                terrainTypeIndex <- value
                this.ShaderData.RefreshTerrain this

    /// 高度
    let mutable elevation: int = Int32.MinValue

    let refreshPosition () =
        let pos = this.Position
        let y = float32 elevation * HexMetrics.elevationStep

        let perturbY =
            ((HexMetrics.sampleNoise pos).Y * 2f - 1f) * HexMetrics.elevationPerturbStrength

        this.Position <- Vector3(pos.X, y + perturbY, pos.Z)
        this.uiRect.Position <- this.Position + Vector3.Up * 0.01f

    member this.Elevation
        with get () = elevation
        and set value =
            if elevation = value then
                ()
            else
                let originalViewElevation = this.ViewElevation
                elevation <- value
                this.ShaderData.ViewElevationChanged this
                refreshPosition ()
                this.ValidateRivers()

                allHexDirs ()
                |> List.filter (fun d -> this.flags.HasRoad d && this.GetElevationDifference d > 1)
                |> List.iter this.RemoveRoad

                refresh ()

    /// 获取某个方向上的高度差
    member this.GetElevationDifference(direction: HexDirection) =
        match this.GetNeighbor direction with
        | Some n -> elevation - n.Elevation |> Mathf.Abs
        | None -> Int32.MaxValue

    member this.GetEdgeType direction =
        this.GetNeighbor direction
        |> Option.map (fun n -> HexMetrics.getEdgeType elevation n.Elevation)

    member this.GetEdgeType(otherCell: HexCellFS) =
        HexMetrics.getEdgeType elevation otherCell.Elevation

    member this.IncomingRiver = this.flags.RiverInDirection
    member this.OutgoingRiver = this.flags.RiverOutDirection
    member this.HasIncomingRiver = this.flags.HasAny HexFlags.RiverIn
    member this.HasOutgoingRiver = this.flags.HasAny HexFlags.RiverOut
    member this.HasRiver = this.flags.HasAny HexFlags.River
    member this.HasRiverBeginOrEnd = this.HasIncomingRiver <> this.HasOutgoingRiver

    member this.StreamBedY =
        (float32 elevation + HexMetrics.streamBedElevationOffset)
        * HexMetrics.elevationStep

    member this.RiverSurfaceY =
        (float32 elevation + HexMetrics.waterElevationOffset) * HexMetrics.elevationStep

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
                neighbor.RefreshSelfOnly()
                this.RefreshSelfOnly()
            | None -> ()

    /// 移除流入河流
    member this.RemoveIncomingRiver() =
        if this.HasIncomingRiver then
            match this.GetNeighbor this.IncomingRiver.Value with
            | Some neighbor ->
                this.flags <- this.flags.Without HexFlags.RiverIn
                neighbor.flags <- neighbor.flags.Without HexFlags.RiverOut
                neighbor.RefreshSelfOnly()
                this.RefreshSelfOnly()
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
                specialIndex <- 0
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
            neighbor.RefreshSelfOnly()
            this.RefreshSelfOnly()
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
            && not <| (this.GetNeighbor direction).Value.IsSpecial
            && this.GetElevationDifference direction <= 1
        then
            this.flags <- this.flags.WithRoad direction

            match this.GetNeighbor direction with
            | Some neighbor ->
                neighbor.flags <- neighbor.flags.WithRoad direction.Opposite
                neighbor.RefreshSelfOnly()
                this.RefreshSelfOnly()
            | None -> ()

    /// 水位
    let mutable waterLevel = 0

    member this.WaterLevel
        with get () = waterLevel
        and set value =
            if waterLevel = value then
                ()
            else
                let originalViewElevation = this.ViewElevation
                waterLevel <- value
                this.ShaderData.ViewElevationChanged this
                this.ValidateRivers()
                refresh ()

    /// 是否在水下
    member this.IsUnderWater = waterLevel > elevation

    member this.WaterSurfaceY =
        (float32 waterLevel + HexMetrics.waterElevationOffset)
        * HexMetrics.elevationStep

    /// 判断某个邻居是否可以作为河流目的地
    member this.IsValidRiverDestination(neighborOpt: HexCellFS option) =
        neighborOpt.IsSome
        && (elevation >= neighborOpt.Value.Elevation
            || waterLevel = neighborOpt.Value.Elevation)

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
    let mutable urbanLevel = 0

    member this.UrbanLevel
        with get () = urbanLevel
        and set value =
            if urbanLevel <> value then
                urbanLevel <- value
                this.RefreshSelfOnly()

    // 农场级别
    let mutable farmLevel = 0

    member this.FarmLevel
        with get () = farmLevel
        and set value =
            if farmLevel <> value then
                farmLevel <- value
                this.RefreshSelfOnly()

    // 植物级别
    let mutable plantLevel = 0

    member this.PlantLevel
        with get () = plantLevel
        and set value =
            if plantLevel <> value then
                plantLevel <- value
                this.RefreshSelfOnly()

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
    let mutable specialIndex = 0

    member this.SpecialIndex
        with get () = specialIndex
        and set value =
            if specialIndex <> value && (not this.HasRiver || value = 0) then
                specialIndex <- value

                if value <> 0 then
                    this.RemoveRoads()

                this.RefreshSelfOnly()

    member this.IsSpecial = specialIndex > 0

    // 保存和加载
    member this.Save(writer: BinaryWriter) =
        writer.Write(byte terrainTypeIndex)
        writer.Write(byte <| elevation + 127)
        writer.Write(byte waterLevel)
        writer.Write(byte urbanLevel)
        writer.Write(byte farmLevel)
        writer.Write(byte plantLevel)
        writer.Write(byte specialIndex)
        writer.Write this.Walled
        writer.Write(this.IncomingRiver |> Option.map byte |> Option.defaultValue Byte.MaxValue)
        writer.Write(this.OutgoingRiver |> Option.map byte |> Option.defaultValue Byte.MaxValue)
        writer.Write(byte (this.flags &&& HexFlags.Roads))
        writer.Write this.IsExplored

    member this.Load (reader: BinaryReader) header =
        this.flags <- this.flags &&& HexFlags.Explorable
        terrainTypeIndex <- int <| reader.ReadByte()
        elevation <- int <| reader.ReadByte()

        if header >= 4 then
            elevation <- elevation - 127

        refreshPosition ()
        waterLevel <- int <| reader.ReadByte()
        urbanLevel <- int <| reader.ReadByte()
        farmLevel <- int <| reader.ReadByte()
        plantLevel <- int <| reader.ReadByte()
        specialIndex <- int <| reader.ReadByte()

        if reader.ReadBoolean() then
            this.flags <- this.flags.With HexFlags.Walled

        match reader.ReadByte() with
        | Byte.MaxValue -> ()
        | x -> this.flags <- int x |> enum<HexDirection> |> this.flags.WithRiverIn

        match reader.ReadByte() with
        | Byte.MaxValue -> ()
        | x -> this.flags <- int x |> enum<HexDirection> |> this.flags.WithRiverOut

        this.flags <- this.flags ||| enum<HexFlags> (int <| reader.ReadByte())
        setExplored <| if header >= 3 then reader.ReadBoolean() else false
        this.ShaderData.RefreshTerrain this
        this.ShaderData.RefreshVisibility this
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
    val mutable PathFrom: HexCellFS

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
    // 单元格着色器数据
    [<DefaultValue>]
    val mutable ShaderData: HexCellShaderData
    // 索引
    [<DefaultValue>]
    val mutable Index: int
    // 可见性
    let mutable visibility = 0
    member this.IsVisible = visibility > 0 && this.Explorable

    member this.IncreaseVisibility() =
        visibility <- visibility + 1

        if visibility = 1 then
            setExplored true
            this.ShaderData.RefreshVisibility this

    member this.DecreaseVisibility() =
        visibility <- visibility - 1

        if visibility = 0 then
            this.ShaderData.RefreshVisibility this
    // 探索
    member this.IsExplored: bool =
        this.flags.HasAll(HexFlags.Explored ||| HexFlags.Explorable)

    let setExplored v =
        this.flags <-
            if v then
                this.flags.With HexFlags.Explored
            else
                this.flags.Without HexFlags.Explored

    member this.Explorable
        with get () = this.flags.HasAny HexFlags.Explorable
        and set value =
            this.flags <-
                if value then
                    this.flags.With HexFlags.Explorable
                else
                    this.flags.Without HexFlags.Explorable
    // 视野高度
    member this.ViewElevation = if elevation >= waterLevel then elevation else waterLevel

    member this.ResetVisibility() =
        if visibility > 0 then
            visibility <- 0
            this.ShaderData.RefreshVisibility this

    member this.SetMapData data =
        // GD.Print $"Setting {this.Coordinates} map data {data}"
        this.ShaderData.SetMapData this data
    // 包覆
    member val ColumnIndex = 0 with get, set
