namespace FrontEndToolFS.Tool

open System
open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot

type IChunk =
    interface
        abstract member Refresh: unit -> unit
    end

type HexCellFS() as this =
    inherit Node3D()

    let _label = lazy this.GetNode<Label3D> "Label"

    [<DefaultValue>]
    val mutable Coordinates: HexCoordinates

    [<DefaultValue>]
    val mutable Chunk: IChunk

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

    member this.RefreshSelfOnly() = this.Chunk.Refresh()

    let mutable color: Color = Colors.White

    member this.Color
        with get () = color
        and set value =
            if color = value then
                ()
            else
                color <- value
                refresh ()

    /// 高度
    let mutable elevation: int = Int32.MinValue

    member this.Elevation
        with get () = elevation
        and set value =
            if elevation = value then
                ()
            else
                elevation <- value
                let pos = this.Position
                let y = float32 value * HexMetrics.elevationStep

                let perturbY =
                    ((HexMetrics.sampleNoise pos).Y * 2f - 1f) * HexMetrics.elevationPerturbStrength

                this.Position <- Vector3(pos.X, y + perturbY, pos.Z)

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

    member this.ShowUI visible = _label.Value.Visible <- visible

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
                neighbor.RemoveIncomingRiver()
                neighbor.IncomingRiver <- Some <| direction.Opposite()
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
                waterLevel <- value
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

        if incomingRiver.IsSome && not << this.IsValidRiverDestination <| Some this then
            this.RemoveIncomingRiver()
