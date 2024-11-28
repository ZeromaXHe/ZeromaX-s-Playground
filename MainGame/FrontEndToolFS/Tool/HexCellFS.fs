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

    let _label = lazy this.GetNode<Label3D>("Label")

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

                if outgoingRiver.IsSome then
                    match this.GetNeighbor outgoingRiver.Value with
                    | Some n when elevation < n.Elevation -> this.RemoveOutgoingRiver()
                    | _ -> ()

                if incomingRiver.IsSome then
                    match this.GetNeighbor incomingRiver.Value with
                    | Some n when elevation < n.Elevation -> this.RemoveIncomingRiver()
                    | _ -> ()

                refresh ()

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

    member this.StreamBedY =
        (float32 elevation + HexMetrics.streamBedElevationOffset)
        * HexMetrics.elevationStep

    member this.RiverSurfaceY =
        (float32 elevation + HexMetrics.riverSurfaceElevationOffset)
        * HexMetrics.elevationStep

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
            match this.GetNeighbor direction with
            | Some neighbor when elevation >= neighbor.Elevation ->
                GD.Print $"SetOutgoingRiver refresh {direction}"
                this.RemoveOutgoingRiver()

                if incomingRiver.IsSome && incomingRiver.Value = direction then
                    this.RemoveIncomingRiver()

                this.OutgoingRiver <- Some direction
                this.RefreshSelfOnly()
                neighbor.RemoveIncomingRiver()
                neighbor.IncomingRiver <- Some <| direction.Opposite()
                neighbor.RefreshSelfOnly()
            | _ ->
                GD.Print $"SetOutgoingRiver no neighbor or low {direction}"
                ()
