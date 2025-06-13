namespace TO.FSharp.Apps.Planets

open Friflo.Engine.ECS
open Godot
open Godot.Abstractions.Extensions.Chunks
open Godot.Abstractions.Extensions.Planets
open TO.FSharp.Commons.DataStructures
open TO.FSharp.Repos.Functions.HexSpheres
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.Meshes
open TO.FSharp.Repos.Models.PathFindings
open TO.FSharp.Repos.Models.Shaders
open TO.FSharp.Repos.Models.HexSpheres.Tiles
open TO.FSharp.Services.Functions

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 19:41:30
type PlanetApp(planet: IPlanet, catlikeCodingNoise: ICatlikeCodingNoise, chunkLoader: IChunkLoader) =
    let store = EntityStore()
    let chunkVpTree = VpTree<Vector3>()
    let tileVpTree = VpTree<Vector3>()
    let tileShaderData = TileShaderData()
    let tileSearcher = TileSearcher()
    let lodMeshCache = LodMeshCache()

    let chunkDep =
        { Store = store
          TagChunk = Tags.Get<TagChunk>()
          TagTile = Tags.Get<TagTile>() }

    let pointRepoDep = PointRepo.getDependency chunkDep chunkVpTree tileVpTree
    let faceRepoDep = FaceRepo.getDependency chunkDep
    let tileRepoDep = TileRepo.getDependency store
    let chunkRepoDep = ChunkRepo.getDependency store

    let hexSphereServiceDep =
        HexSphereService.getDependency
            planet
            chunkLoader
            tileShaderData
            tileSearcher
            pointRepoDep
            faceRepoDep
            tileRepoDep
            chunkRepoDep

    let hexGridChunkServiceDep =
        HexGridChunkService.getDependency
            planet
            catlikeCodingNoise
            chunkLoader
            lodMeshCache
            pointRepoDep
            faceRepoDep
            tileRepoDep
            chunkRepoDep

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()
        GD.Print $"[===DrawHexSphereMesh===] radius {planet.Radius}, divisions {planet.Divisions}, start at: {time}"
        hexSphereServiceDep.ClearOldData()
        hexSphereServiceDep.InitHexSphere()
        hexGridChunkServiceDep.InitChunkNodes()

    member this.OnChunkLoaderProcessed() =
        hexGridChunkServiceDep.OnChunkLoaderProcessed()

    member this.OnHexGridChunkProcessed(chunk: IHexGridChunk) =
        hexGridChunkServiceDep.OnHexGridChunkProcessed chunk

    member this.UpdateInsightChunks() =
        hexGridChunkServiceDep.UpdateInsightChunks()
