namespace FrontEndToolFS.Tool

open System
open System.IO
open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot

type IChunk =
    interface
        abstract member Refresh: unit -> unit
    end

type IUnit =
    interface
        abstract member ValidateLocation: unit -> unit
        abstract member Die: unit -> unit
    end

type HexCellFS() as this =
    inherit Node3D()

    interface ICell with
        override this.Index = this.Index
        override this.TerrainTypeIndex = this.TerrainTypeIndex
        override this.IsVisible = this.IsVisible
        override this.IsExplored = this.IsExplored

    [<DefaultValue>]
    val mutable Coordinates: HexCoordinates

    [<DefaultValue>]
    val mutable Chunk: IChunk

    [<DefaultValue>]
    val mutable uiRect: HexCellLabelFS

    member val neighbors: HexCellFS option array = Array.create 6 None

    member this.GetNeighbor(direction: HexDirection) = this.neighbors[int direction]

    member this.SetNeighbor (direction: HexDirection) cell =
        this.neighbors[int direction] <- cell

        cell
        |> Option.iter (fun c -> c.neighbors[int <| direction.Opposite()] <- Some this)

    let refresh () =
        this.Chunk.Refresh()

        this.neighbors
        |> Array.iter (fun n ->
            if n.IsSome && n.Value.Chunk <> this.Chunk then
                n.Value.Chunk.Refresh())

        this.Unit |> Option.iter _.ValidateLocation()

    member this.RefreshSelfOnly() =
        this.Chunk.Refresh()
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

                if this.ViewElevation <> originalViewElevation then
                    this.ShaderData.ViewElevationChanged()

                refreshPosition ()
                this.ValidateRivers()

                for i in 0 .. this.roads.Length - 1 do
                    if this.roads[i] && this.GetElevationDifference <| enum<HexDirection> i > 1 then
                        this.SetRoad i false

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

    let mutable incomingRiver: HexDirection option = None
    let mutable outgoingRiver: HexDirection option = None

    member this.IncomingRiver
        with get () = incomingRiver
        and private set value = incomingRiver <- value

    member this.OutgoingRiver
        with get () = outgoingRiver
        and private set value = outgoingRiver <- value

    member this.HasRiver = incomingRiver.IsSome || outgoingRiver.IsSome
    member this.HasRiverBeginOrEnd = incomingRiver.IsSome <> outgoingRiver.IsSome

    member this.RiverBeginOrEndDirection =
        if incomingRiver.IsSome then
            incomingRiver.Value
        else
            outgoingRiver.Value

    member this.StreamBedY =
        (float32 elevation + HexMetrics.streamBedElevationOffset)
        * HexMetrics.elevationStep

    member this.RiverSurfaceY =
        (float32 elevation + HexMetrics.waterElevationOffset) * HexMetrics.elevationStep

    member this.HasRiverThroughEdge(direction: HexDirection) =
        (incomingRiver.IsSome && incomingRiver.Value = direction)
        || (outgoingRiver.IsSome && outgoingRiver.Value = direction)

    /// 移除流出河流
    member this.RemoveOutgoingRiver() =
        if outgoingRiver.IsSome then
            match this.GetNeighbor outgoingRiver.Value with
            | Some neighbor ->
                neighbor.IncomingRiver <- None
                neighbor.RefreshSelfOnly()
            | None -> ()

            outgoingRiver <- None
            this.RefreshSelfOnly()

    /// 移除流入河流
    member this.RemoveIncomingRiver() =
        if incomingRiver.IsSome then
            match this.GetNeighbor incomingRiver.Value with
            | Some neighbor ->
                neighbor.OutgoingRiver <- None
                neighbor.RefreshSelfOnly()
            | None -> ()

            incomingRiver <- None
            this.RefreshSelfOnly()

    /// 移除河流
    member this.RemoveRiver() =
        this.RemoveIncomingRiver()
        this.RemoveOutgoingRiver()

    member this.SetOutgoingRiver(direction: HexDirection) =
        if outgoingRiver.IsSome && outgoingRiver.Value = direction then
            GD.Print $"SetOutgoingRiver already river {direction}"
            ()
        else
            let neighborOpt = this.GetNeighbor direction

            if this.IsValidRiverDestination neighborOpt then
                let neighbor = neighborOpt.Value
                // GD.Print $"SetOutgoingRiver refresh {direction}"
                this.RemoveOutgoingRiver()

                if incomingRiver.IsSome && incomingRiver.Value = direction then
                    this.RemoveIncomingRiver()

                this.OutgoingRiver <- Some direction
                specialIndex <- 0
                neighbor.RemoveIncomingRiver()
                neighbor.IncomingRiver <- Some <| direction.Opposite()
                neighbor.SpecialIndex <- 0
                this.SetRoad <| int direction <| false
            else
                GD.Print $"SetOutgoingRiver no neighbor or low {direction}"
                ()

    /// 道路
    member val roads: bool array = Array.create 6 false
    /// 某个方向上是否有道路
    member this.HasRoadThroughEdge(direction: HexDirection) = this.roads[int direction]
    /// 是否至少有一条道路
    member this.HasRoads = this.roads |> Array.exists id

    member this.SetRoad index state =
        this.roads[index] <- state

        match this.neighbors[index] with
        | Some neighbor ->
            neighbor.roads[int <| (enum<HexDirection> index).Opposite()] <- state
            neighbor.RefreshSelfOnly()
            this.RefreshSelfOnly()
        | None -> ()

    /// 移除道路
    member this.RemoveRoads() =
        for i in 0 .. this.neighbors.Length - 1 do
            if this.roads[i] then
                this.SetRoad i false

    /// 添加道路
    member this.AddRoad(direction: HexDirection) =
        if
            not <| this.HasRoadThroughEdge direction
            && not <| this.HasRiverThroughEdge direction
            && not this.IsSpecial
            // AddRoad 的入口保证这里一定有邻居
            && not <| (this.GetNeighbor direction).Value.IsSpecial
            && this.GetElevationDifference direction <= 1
        then
            this.SetRoad (int direction) true

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

                if this.ViewElevation <> originalViewElevation then
                    this.ShaderData.ViewElevationChanged()

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
            outgoingRiver.IsSome
            && not << this.IsValidRiverDestination <| this.GetNeighbor outgoingRiver.Value
        then
            this.RemoveOutgoingRiver()

        if
            incomingRiver.IsSome
            && not << (this.GetNeighbor incomingRiver.Value).Value.IsValidRiverDestination
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
    let mutable walled = false

    member this.Walled
        with get () = walled
        and set value =
            if walled <> value then
                walled <- value
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
        writer.Write walled
        writer.Write(incomingRiver |> Option.map byte |> Option.defaultValue Byte.MaxValue)
        writer.Write(outgoingRiver |> Option.map byte |> Option.defaultValue Byte.MaxValue)

        writer.Write(
            this.roads
            |> Array.fold (fun (i, w) b -> (i + 1), (if b then w ||| (1uy <<< i) else w)) (0, 0uy)
            |> snd
        )

        writer.Write explored

    member this.Load (reader: BinaryReader) header =
        terrainTypeIndex <- int <| reader.ReadByte()
        this.ShaderData.RefreshTerrain this
        elevation <- int <| reader.ReadByte()

        if header >= 4 then
            elevation <- elevation - 127

        refreshPosition ()
        waterLevel <- int <| reader.ReadByte()
        urbanLevel <- int <| reader.ReadByte()
        farmLevel <- int <| reader.ReadByte()
        plantLevel <- int <| reader.ReadByte()
        specialIndex <- int <| reader.ReadByte()
        walled <- reader.ReadBoolean()

        incomingRiver <-
            reader.ReadByte()
            |> function
                | Byte.MaxValue -> None
                | x -> int x |> enum<HexDirection> |> Some

        outgoingRiver <-
            reader.ReadByte()
            |> function
                | Byte.MaxValue -> None
                | x -> int x |> enum<HexDirection> |> Some

        let roadFlags = reader.ReadByte()

        this.roads
        |> Array.iteri (fun i _ -> this.roads[i] <- (roadFlags &&& (1uy <<< i)) <> 0uy)

        explored <- if header >= 3 then reader.ReadBoolean() else false
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
            explored <- true
            this.ShaderData.RefreshVisibility this

    member this.DecreaseVisibility() =
        visibility <- visibility - 1

        if visibility = 0 then
            this.ShaderData.RefreshVisibility this
    // 探索
    let mutable explored = false
    member this.IsExplored = explored && this.Explorable
    member val Explorable = false with get, set
    // 视野高度
    member this.ViewElevation = if elevation >= waterLevel then elevation else waterLevel

    member this.ResetVisibility() =
        if visibility > 0 then
            visibility <- 0
            this.ShaderData.RefreshVisibility this

    member this.SetMapData data =
        // GD.Print $"Setting {this.Coordinates} map data {data}"
        this.ShaderData.SetMapData this data
