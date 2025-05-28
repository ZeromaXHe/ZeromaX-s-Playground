namespace TO.Infras.Abstractions.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Commons.Structs.HexSphereGrid
open TO.Infras.Abstractions.Planets.Models.Faces
open TO.Infras.Abstractions.Planets.Models.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-28 09:21:28
[<Interface>]
type IPointRepo =
    abstract ForEachByChunky: chunky: bool -> forEachPoint: (PointComponent -> Entity -> unit) -> unit
    abstract TryHeadByPosition: chunky: bool -> pos: Vector3 -> Entity option
    abstract Add: chunky: bool -> position: Vector3 -> coords: SphereAxial -> int

    abstract GetNeighborCenterPointIds:
        chunky: bool * hexFaces: FaceComponent list * center: PointComponent inref -> int ResizeArray

    abstract CreateVpTree: chunky: bool -> unit
    abstract SearchNearestCenterPos: pos: Vector3 * chunky: bool -> Vector3
    abstract Truncate: unit -> unit
