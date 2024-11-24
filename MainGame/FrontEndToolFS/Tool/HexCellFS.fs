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

    let addTriangle v1 v2 v3 (surfaceTool: SurfaceTool) vIndex =
        surfaceTool.AddVertex v1
        surfaceTool.AddVertex v2
        surfaceTool.AddVertex v3
        surfaceTool.AddIndex <| vIndex
        surfaceTool.AddIndex <| vIndex + 1
        surfaceTool.AddIndex <| vIndex + 2
        vIndex + 3

    member val neighbors: HexCellFS option array = Array.create 6 None

    member this.GetNeighbor(direction: HexDirection) = this.neighbors[int direction]

    member this.SetNeighbor (direction: HexDirection) cell =
        this.neighbors[int direction] <- cell
        cell |> Option.iter (fun c -> c.neighbors[int <| direction.Opposite()] <- Some this)
