namespace TO.Domains.Types.HexMeshes

open System.Collections.Generic
open Godot
open TO.Domains.Types.DataStructures
open TO.Domains.Types.HexSpheres

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

    member this.Cache = cache

type GetLodMeshes = ChunkLodEnum -> int -> Mesh array option

[<Interface>]
type ILodMeshCacheQuery =
    abstract LodMeshCache: LodMeshCache
    abstract GetLodMeshes: GetLodMeshes

type AddLodMeshes = ChunkLodEnum -> int -> Mesh array -> unit
type RemoveAllLodMeshes = unit -> unit
type RemoveLodMeshes = int -> unit

[<Interface>]
type ILodMeshCacheCommand =
    abstract AddLodMeshes: AddLodMeshes
    abstract RemoveAllLodMeshes: RemoveAllLodMeshes
    abstract RemoveLodMeshes: RemoveLodMeshes
