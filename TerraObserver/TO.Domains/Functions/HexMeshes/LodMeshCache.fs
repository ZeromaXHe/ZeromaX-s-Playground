namespace TO.Domains.Functions.HexMeshes

open Godot
open TO.Domains.Functions.DataStructures
open TO.Domains.Types.HexMeshes
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 09:32:24
module LodMeshCacheQuery =
    let getLodMeshes (env: #ILodMeshCacheQuery) : GetLodMeshes =
        fun (lod: ChunkLodEnum) (id: int) ->
            match env.LodMeshCache.Cache.TryGetValue lod with
            | true, cache -> cache |> LruCache.get id
            | _ -> None

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 10:36:25
module LodMeshCacheCommand =
    let addLodMeshes (env: #ILodMeshCacheQuery) : AddLodMeshes =
        fun (lod: ChunkLodEnum) (id: int) (meshes: Mesh array) ->
            match env.LodMeshCache.Cache.TryGetValue lod with
            | true, cache ->
                if cache |> LruCache.exist id then
                    GD.PrintErr $"overwriting Chunk {id} Lod {lod} Mesh!"

                cache |> LruCache.put id meshes
            | _ -> ()

    let removeAllLodMeshes (env: #ILodMeshCacheQuery) : RemoveAllLodMeshes =
        fun () ->
            let lodMeshCache = env.LodMeshCache

            for lod in ChunkLodEnum.GetValues() do
                match lodMeshCache.Cache.TryGetValue lod with
                | true, cache -> cache |> LruCache.clear
                | _ -> ()

    let removeLodMeshes (env: #ILodMeshCacheQuery) : RemoveLodMeshes =
        fun (id: int) ->
            let lodMeshCache = env.LodMeshCache

            for lod in ChunkLodEnum.GetValues() do
                match lodMeshCache.Cache.TryGetValue lod with
                | true, cache -> cache |> LruCache.remove id
                | _ -> ()
