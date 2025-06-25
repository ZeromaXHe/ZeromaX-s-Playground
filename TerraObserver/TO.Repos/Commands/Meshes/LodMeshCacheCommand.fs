namespace TO.Repos.Commands.Meshes

open Godot
open TO.Domains.Enums.HexSpheres.Chunks
open TO.Repos.Data.Meshes

type AddLodMeshes = ChunkLodEnum -> int -> Mesh array -> unit

[<Interface>]
type ILodMeshCacheCommand =
    abstract AddLodMeshes: AddLodMeshes

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 10:36:25
module LodMeshCacheCommand =
    let addLodMeshes (cache: LodMeshCache) : AddLodMeshes = cache.AddLodMeshes
