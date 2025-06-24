namespace TO.Repos.Queries.Meshes

open Godot
open TO.Domains.Enums.HexSpheres.Chunks
open TO.Repos.Data.Meshes

type GetLodMeshesCache = ChunkLodEnum -> int -> Mesh array option

[<Interface>]
type ILodMeshCacheQuery =
    abstract GetLodMeshes: GetLodMeshesCache

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 09:32:24
module LodMeshCacheQuery =
    let getLodMeshes (lodMeshCache: LodMeshCache) : GetLodMeshesCache = lodMeshCache.GetLodMeshes
