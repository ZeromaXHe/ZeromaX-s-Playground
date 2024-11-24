namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot

type HexCellFS() =
    inherit Node3D()

    [<DefaultValue>]
    val mutable Coordinates: HexCoordinates

    [<DefaultValue>]
    val mutable Color: Color

    let mutable elevation: int = 0

    member this.Elevation
        with get () = elevation
        and set value =
            elevation <- value
            let pos = this.Position
            this.Position <- Vector3(pos.X, float32 value * HexMetrics.elevationStep, pos.Z)

    member val neighbors: HexCellFS option array = Array.create 6 None

    member this.GetNeighbor(direction: HexDirection) = this.neighbors[int direction]

    member this.SetNeighbor (direction: HexDirection) cell =
        this.neighbors[int direction] <- cell

        cell
        |> Option.iter (fun c -> c.neighbors[int <| direction.Opposite()] <- Some this)

    member this.GetEdgeType direction =
        this.GetNeighbor direction
        |> Option.map (fun n -> HexMetrics.getEdgeType elevation n.Elevation)

    member this.GetEdgeType(otherCell: HexCellFS) =
        HexMetrics.getEdgeType elevation otherCell.Elevation
