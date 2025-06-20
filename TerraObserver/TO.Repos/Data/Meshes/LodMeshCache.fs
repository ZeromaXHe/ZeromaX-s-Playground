namespace TO.Repos.Data.Meshes

open System.Collections.Generic
open Godot
open TO.Domains.Enums.HexSpheres.Chunks
open TO.Repos.Data.Commons

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 16:33:08
type LodMeshCache() =
    let cache = Dictionary<ChunkLodEnum, LruCache<int, Mesh array>>()
    do cache.Add(ChunkLodEnum.JustHex, LruCache<int, Mesh[]>(2500))
    do cache.Add(ChunkLodEnum.PlaneHex, LruCache<int, Mesh[]>(5000))
    do cache.Add(ChunkLodEnum.SimpleHex, LruCache<int, Mesh[]>(10000))
    do cache.Add(ChunkLodEnum.TerracesHex, LruCache<int, Mesh[]>(20000))
    do cache.Add(ChunkLodEnum.Full, LruCache<int, Mesh[]>(40000))

    member this.GetLodMeshes (lod: ChunkLodEnum) (id: int) =
        match cache.TryGetValue lod with
        | true, cache -> cache.Get id
        | _ -> None

    member this.AddLodMeshes (lod: ChunkLodEnum) (id: int) (meshes: Mesh array) =
        match cache.TryGetValue lod with
        | true, cache ->
            if cache.Exist id then
                GD.PrintErr $"overwriting Chunk {id} Lod {lod} Mesh!"

            cache.Put id meshes
        | _ -> ()

    member this.RemoveAllLodMeshes() =
        for lod in ChunkLodEnum.GetValues() do
            match cache.TryGetValue lod with
            | true, cache -> cache.Clear()
            | _ -> ()

    member this.RemoveLodMeshes(id: int) =
        for lod in ChunkLodEnum.GetValues() do
            match cache.TryGetValue lod with
            | true, cache -> cache.Remove id
            | _ -> ()

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:31:19
[<Interface>]
type ILodMeshCache =
    abstract LodMeshCache: LodMeshCache
