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

    [<DefaultValue>]
    val mutable Coordinates: HexCoordinates

    [<DefaultValue>]
    val mutable Chunk: IChunk option

    member val neighbors: HexCellFS option array = Array.create 6 None

    member this.GetNeighbor(direction: HexDirection) = this.neighbors[int direction]

    member this.SetNeighbor (direction: HexDirection) cell =
        this.neighbors[int direction] <- cell

        cell
        |> Option.iter (fun c -> c.neighbors[int <| direction.Opposite()] <- Some this)

    let refresh () =
        this.Chunk
        |> Option.iter (fun c ->
            c.Refresh()

            this.neighbors
            |> Array.iter (fun n ->
                if n.IsSome && n.Value.Chunk <> this.Chunk then
                    n.Value.Chunk.Value.Refresh()))

    let mutable color: Color = Colors.White
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
                refresh ()

    member this.Color
        with get () = color
        and set value =
            if color = value then
                ()
            else
                color <- value
                refresh ()

    member this.GetEdgeType direction =
        this.GetNeighbor direction
        |> Option.map (fun n -> HexMetrics.getEdgeType elevation n.Elevation)

    member this.GetEdgeType(otherCell: HexCellFS) =
        HexMetrics.getEdgeType elevation otherCell.Elevation
