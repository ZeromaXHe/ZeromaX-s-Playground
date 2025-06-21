namespace TO.Controllers.Apps.Planets

open Friflo.Engine.ECS
open Godot
open TO.Abstractions.Cameras
open TO.Abstractions.Chunks
open TO.Abstractions.Planets
open TO.Controllers.Apps.Envs
open TO.Presenters.Commands.Cameras
open TO.Repos.Commands.HexSpheres
open TO.Repos.Data.Commons
open TO.Controllers.Services.Planets
open TO.Repos.Data.HexSpheres
open TO.Repos.Data.Meshes
open TO.Repos.Data.PathFindings
open TO.Repos.Data.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 19:41:30
type PlanetApp
    (planet: IPlanet, catlikeCodingNoise: ICatlikeCodingNoise, cameraRig: IOrbitCameraRig, chunkLoader: IChunkLoader) =
    let store = EntityStore()

    let chunkyVpTrees =
        { ChunkVpTree = VpTree<Vector3>()
          TileVpTree = VpTree<Vector3>() }

    let tileShaderData = TileShaderData()
    let tileSearcher = TileSearcher()
    let lodMeshCache = LodMeshCache()
    let repoEnv = PlanetRepoEnv(store, chunkyVpTrees)
    let preEnv = PlanetPreEnv(planet, cameraRig)
    let orbitCameraRigCommand = preEnv :> IOrbitCameraRigCommand
    member this.InitOrbitCameraRig() = orbitCameraRigCommand.Reset()

    member this.OnOrbitCameraRigPlanetParamsChanged() =
        orbitCameraRigCommand.OnPlanetParamsChanged()
        orbitCameraRigCommand.OnZoomChanged()

    member this.OnOrbitCameraRigProcessed(delta: float32) = orbitCameraRigCommand.OnProcessed delta
    member this.OnOrbitCameraRigZoomChanged() = orbitCameraRigCommand.OnZoomChanged()

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()
        GD.Print $"[===DrawHexSphereMesh===] radius {planet.Radius}, divisions {planet.Divisions}, start at: {time}"
        HexSphereInitCommand.clearOldData store ()
        HexSphereService.initHexSphere planet store tileShaderData tileSearcher repoEnv
        HexGridChunkService.initChunkNodes chunkLoader store

    member this.OnChunkLoaderProcessed() =
        HexGridChunkService.onChunkLoaderProcessed chunkLoader lodMeshCache store repoEnv

    member this.OnHexGridChunkProcessed(chunk: IHexGridChunk) =
        HexGridChunkService.onHexGridChunkProcessed planet catlikeCodingNoise lodMeshCache store repoEnv chunk

    member this.UpdateInsightChunks() =
        HexGridChunkService.updateInsightChunks chunkLoader store
