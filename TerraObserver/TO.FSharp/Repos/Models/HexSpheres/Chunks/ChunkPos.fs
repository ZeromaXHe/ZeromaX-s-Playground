namespace TO.FSharp.Repos.Models.HexSpheres.Chunks

open Friflo.Engine.ECS
open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 13:35:08
[<Struct>]
type ChunkPos =
    interface IComponent
    val Pos: Vector3 // 这里存储是实际分块中心位置（带有星球半径）
    new(pos: Vector3) = { Pos = pos }