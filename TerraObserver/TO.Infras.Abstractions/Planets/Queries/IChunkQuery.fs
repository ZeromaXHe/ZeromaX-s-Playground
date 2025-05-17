namespace TO.Infras.Abstractions.Planets.Queries

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-16 10:11:16
[<Interface>]
type IChunkQuery =
    abstract SearchNearestChunk: pos: Vector3 -> Vector3