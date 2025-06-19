namespace TO.FSharp.Apps.Planets

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Repos.Data.Commons
open TO.FSharp.Repos.Data.HexSpheres
open TO.FSharp.Repos.Data.PathFindings
open TO.FSharp.Repos.Data.Shaders
open TO.FSharp.Repos.Data.Meshes

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:33:19
type PlanetDataEnv() =
    let entityStore = EntityStore()

    let chunkyVpTrees =
        { ChunkVpTree = VpTree<Vector3>()
          TileVpTree = VpTree<Vector3>() }

    let tileShaderData = TileShaderData()
    let tileSearcher = TileSearcher()
    let lodMeshCache = LodMeshCache()

    interface IEntityStore with
        override _.EntityStore = entityStore

    interface IChunkyVpTrees with
        override _.ChunkyVpTrees = chunkyVpTrees

    interface ITileShaderData with
        override _.TileShaderData = tileShaderData

    interface ITileSearcher with
        override _.TileSearcher = tileSearcher

    interface ILodMeshCache with
        override _.LodMeshCache = lodMeshCache
