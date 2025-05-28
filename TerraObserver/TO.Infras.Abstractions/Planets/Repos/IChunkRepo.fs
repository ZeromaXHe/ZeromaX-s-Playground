namespace TO.Infras.Abstractions.Planets.Repos

open Friflo.Engine.ECS
open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-27 16:06:27
[<Interface>]
type IChunkRepo =
    abstract TryHeadByCenterId: centerId: int -> Entity option
    abstract Add: centerId: int * pos: Vector3 * neighborCenterIds: int array -> int
    abstract Truncate: unit -> unit
