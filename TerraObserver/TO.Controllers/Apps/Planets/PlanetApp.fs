namespace TO.Controllers.Apps.Planets

open Friflo.Engine.ECS
open Godot
open TO.Abstractions.Views.Chunks
open TO.Controllers.Apps.Envs
open TO.Presenters.Views.Cameras
open TO.Presenters.Views.Chunks
open TO.Presenters.Views.Geos
open TO.Presenters.Views.Planets
open TO.Presenters.Views.Uis
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
type PlanetApp(planet, catlikeCodingNoise, cameraRig, lonLatGrid, celestialMotion, chunkLoader, planetHud) =
    let store = EntityStore()

    let chunkyVpTrees =
        { ChunkVpTree = VpTree<Vector3>()
          TileVpTree = VpTree<Vector3>() }

    let tileShaderData = TileShaderData()
    let tileSearcher = TileSearcher()
    let lodMeshCache = LodMeshCache()
    let repoEnv = PlanetRepoEnv(store, chunkyVpTrees, lodMeshCache)

    let preEnv =
        PlanetPreEnv(planet, catlikeCodingNoise, cameraRig, lonLatGrid, celestialMotion, chunkLoader, planetHud)

    let orbitCameraRigCommand = preEnv :> IOrbitCameraRigCommand
    let lonLatGridCommand = preEnv :> ILonLatGridCommand
    let celestialMotionCommand = preEnv :> ICelestialMotionCommand
    let chunkLoaderCommand = preEnv :> IChunkLoaderCommand
    let planetHudCommand = preEnv :> IPlanetHudCommand

    member this.Init() =
        orbitCameraRigCommand.Reset()
        lonLatGridCommand.DrawOnPlanet()

        if not <| Engine.IsEditorHint() then
            planetHudCommand.InitElevationAndWaterVSlider()

            planetHudCommand.OnOrbitCameraRigMoved
            <| (preEnv :> IOrbitCameraRigQuery).GetFocusBasePos() // TODO: 临时措施，待优化
            <| 0f

    member this.OnPlanetParamsChanged() =
        orbitCameraRigCommand.OnPlanetParamsChanged()
        orbitCameraRigCommand.OnZoomChanged()
        lonLatGridCommand.DrawOnPlanet()
        // TODO: 判断 Radius 变化时才调用
        celestialMotionCommand.UpdateMoonMeshRadius()
        celestialMotionCommand.UpdateLunarDist()

    member this.OnOrbitCameraRigProcessed delta = orbitCameraRigCommand.OnProcessed delta
    member this.OnOrbitCameraRigZoomChanged() = orbitCameraRigCommand.OnZoomChanged()
    member this.OnLonLatGridDoDrawRequested() = lonLatGridCommand.DoDraw()

    member this.OnLonLatGridCameraMoved(pos, delta) =
        lonLatGridCommand.OnCameraMoved pos delta

    member this.LonLatGridToggleFixFullVisibility toggle =
        lonLatGridCommand.ToggleFixFullVisibility toggle

    member this.OnCelestialMotionSatelliteRadiusRatioChanged() =
        celestialMotionCommand.UpdateMoonMeshRadius()
        celestialMotionCommand.UpdateLunarDist()

    member this.OnCelestialMotionSatelliteDistRatioChanged() =
        celestialMotionCommand.UpdateLunarDist()

    member this.CelestialMotionToggleAllMotions toggle =
        celestialMotionCommand.ToggleAllMotions toggle

    member this.OnPlanetHudOrbitCameraRigTransformed(transform, delta: float32) =
        planetHudCommand.OnOrbitCameraRigTransformed transform

    member this.OnPlanetHudRadiusLineEditTextSubmitted text =
        planetHudCommand.UpdateRadiusLineEdit text

    member this.OnPlanetHudDivisionLineEditTextSubmitted text =
        planetHudCommand.UpdateDivisionLineEdit false text

    member this.OnPlanetHudChunkDivisionLineEditTextSubmitted text =
        planetHudCommand.UpdateDivisionLineEdit true text

    member this.OnPlanetHudOrbitCameraRigMoved pos delta =
        planetHudCommand.OnOrbitCameraRigMoved pos delta

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()
        GD.Print $"[===DrawHexSphereMesh===] radius {planet.Radius}, divisions {planet.Divisions}, start at: {time}"
        HexSphereInitCommand.clearOldData store ()
        chunkLoaderCommand.ClearOldData()
        lodMeshCache.RemoveAllLodMeshes()
        HexSphereService.initHexSphere planet repoEnv store tileShaderData tileSearcher
        HexGridChunkService.initChunkNodes planet preEnv repoEnv store

    member this.OnChunkLoaderProcessed() =

        HexGridChunkService.onChunkLoaderProcessed preEnv repoEnv

    member this.OnHexGridChunkProcessed(chunk: IHexGridChunk) =
        HexGridChunkService.onHexGridChunkProcessed planet preEnv lodMeshCache store repoEnv chunk

    member this.UpdateInsightChunks() =
        HexGridChunkService.updateInsightChunks preEnv planet chunkLoader repoEnv store
