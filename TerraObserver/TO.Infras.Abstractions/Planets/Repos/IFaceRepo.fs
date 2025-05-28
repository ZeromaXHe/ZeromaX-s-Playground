namespace TO.Infras.Abstractions.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Infras.Abstractions.Planets.Models.Faces
open TO.Infras.Abstractions.Planets.Models.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-28 09:22:28
[<Interface>]
type IFaceRepo =
    abstract ForEachByChunky: chunky: bool -> forEachFace: (FaceComponent -> Entity -> unit) -> unit
    abstract Add: chunky: bool -> vertex1: Vector3 -> vertex2: Vector3 -> vertex3: Vector3 -> int
    abstract GetOrderedFaces: pointComponent: PointComponent -> pointEntity: Entity -> Entity list
    abstract Truncate: unit -> unit
