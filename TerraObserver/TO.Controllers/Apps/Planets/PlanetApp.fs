namespace TO.Controllers.Apps.Planets

open Friflo.Engine.ECS
open Godot
open TO.Abstractions.Views.Cameras
open TO.Abstractions.Views.Chunks
open TO.Abstractions.Models.Planets
open TO.Abstractions.Views.Geos
open TO.Abstractions.Views.Planets
open TO.Abstractions.Views.Uis
open TO.Controllers.Apps.Envs
open TO.Presenters.Commands.Cameras
open TO.Presenters.Commands.Geos
open TO.Presenters.Commands.Planets
open TO.Presenters.Commands.Uis
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
    (
        planet: IPlanet,
        catlikeCodingNoise: ICatlikeCodingNoise,
        cameraRig: IOrbitCameraRig,
        lonLatGrid: ILonLatGrid,
        celestialMotion: ICelestialMotion,
        chunkLoader: IChunkLoader,
        planetHud: IPlanetHud
    ) =
    let store = EntityStore()

    let chunkyVpTrees =
        { ChunkVpTree = VpTree<Vector3>()
          TileVpTree = VpTree<Vector3>() }

    let tileShaderData = TileShaderData()
    let tileSearcher = TileSearcher()
    let lodMeshCache = LodMeshCache()
    let repoEnv = PlanetRepoEnv(store, chunkyVpTrees)

    let preEnv =
        PlanetPreEnv(planet, catlikeCodingNoise, cameraRig, lonLatGrid, celestialMotion, planetHud)

    let orbitCameraRigCommand = preEnv :> IOrbitCameraRigCommand
    let lonLatGridCommand = preEnv :> ILonLatGridCommand
    let celestialMotionCommand = preEnv :> ICelestialMotionCommand
    let planetHudCommand = preEnv :> IPlanetHudCommand

    member this.Init() =
        orbitCameraRigCommand.Reset()
        lonLatGridCommand.DrawOnPlanet()
        planetHudCommand.InitElevationAndWaterVSlider()

    member this.OnPlanetParamsChanged() =
        orbitCameraRigCommand.OnPlanetParamsChanged()
        orbitCameraRigCommand.OnZoomChanged()
        lonLatGridCommand.DrawOnPlanet()
        // TODO: 判断 Radius 变化时才调用
        celestialMotionCommand.UpdateMoonMeshRadius()
        celestialMotionCommand.UpdateLunarDist()

    member this.OnOrbitCameraRigProcessed(delta: float32) = orbitCameraRigCommand.OnProcessed delta
    member this.OnOrbitCameraRigZoomChanged() = orbitCameraRigCommand.OnZoomChanged()

    member this.OnOrbitCameraRigTransformed(transform: Transform3D, delta: float32) =
        planetHudCommand.OnOrbitCameraRigTransformed transform

    member this.OnCelestialMotionSatelliteRadiusRatioChanged() =
        celestialMotionCommand.UpdateMoonMeshRadius()
        celestialMotionCommand.UpdateLunarDist()

    member this.OnCelestialMotionSatelliteDistRatioChanged() =
        celestialMotionCommand.UpdateLunarDist()

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()
        GD.Print $"[===DrawHexSphereMesh===] radius {planet.Radius}, divisions {planet.Divisions}, start at: {time}"
        HexSphereInitCommand.clearOldData store ()
        chunkLoader.ClearOldData()
        lodMeshCache.RemoveAllLodMeshes()
        HexSphereService.initHexSphere planet store tileShaderData tileSearcher repoEnv
        HexGridChunkService.initChunkNodes planet chunkLoader store

    member this.OnChunkLoaderProcessed() =
        HexGridChunkService.onChunkLoaderProcessed chunkLoader lodMeshCache store repoEnv

    member this.OnHexGridChunkProcessed(chunk: IHexGridChunk) =
        HexGridChunkService.onHexGridChunkProcessed planet preEnv lodMeshCache store repoEnv chunk

    member this.UpdateInsightChunks() =
        HexGridChunkService.updateInsightChunks planet chunkLoader store
