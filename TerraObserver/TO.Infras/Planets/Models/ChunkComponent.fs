namespace TO.Infras.Planets.Models

open Friflo.Engine.ECS
open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-16 19:30:16
[<Struct>]
type ChunkComponent =
    interface IComponent
    val CenterId: int
    val Pos: Vector3
    val HexFaceIds: int array
    val NeighborCenterIds: int array
