namespace TO.Controllers.Apps.Planets

open Friflo.Engine.ECS
open Godot
open TO.Abstractions.Views.Chunks
open TO.Domains.Components.HexSpheres.Tiles
open TO.Presenters.Views.Cameras
open TO.Presenters.Views.Chunks
open TO.Presenters.Views.Geos
open TO.Presenters.Views.Planets
open TO.Presenters.Views.Uis
open TO.Repos.Commands.HexSpheres
open TO.Repos.Commands.PathFindings
open TO.Repos.Commands.Shaders
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

    let env =
        PlanetEnv(
            // 前端
            planet,
            catlikeCodingNoise,
            cameraRig,
            lonLatGrid,
            celestialMotion,
            chunkLoader,
            planetHud,
            // 后端
            store,
            chunkyVpTrees,
            lodMeshCache,
            tileSearcher,
            tileShaderData
        )

    member this.Init() =
        OrbitCameraRigCommand.reset planet cameraRig ()
        LonLatGridCommand.drawOnPlanet planet lonLatGrid ()

        if not <| Engine.IsEditorHint() then
            PlanetHudCommand.initElevationAndWaterVSlider planet planetHud ()

            PlanetHudCommand.onOrbitCameraRigMoved planetHud
            <| OrbitCameraRigQuery.getFocusBasePos cameraRig ()
            <| 0f

    member this.OnPlanetParamsChanged() =
        OrbitCameraRigCommand.onPlanetParamsChanged planet cameraRig ()
        OrbitCameraRigCommand.onZoomChanged planet cameraRig ()
        LonLatGridCommand.drawOnPlanet planet lonLatGrid ()
        // TODO: 判断 Radius 变化时才调用
        CelestialMotionCommand.updateMoonMeshRadius planet celestialMotion ()
        CelestialMotionCommand.updateLunarDist planet celestialMotion ()

    member this.OnOrbitCameraRigProcessed delta =
        OrbitCameraRigCommand.onProcessed planet cameraRig delta

    member this.OnOrbitCameraRigZoomChanged() =
        OrbitCameraRigCommand.onZoomChanged planet cameraRig ()

    member this.OnLonLatGridDoDrawRequested() = LonLatGridCommand.doDraw lonLatGrid ()

    member this.OnLonLatGridCameraMoved(pos, delta) =
        LonLatGridCommand.onCameraMoved lonLatGrid pos delta

    member this.LonLatGridToggleFixFullVisibility toggle =
        LonLatGridCommand.toggleFixFullVisibility lonLatGrid toggle

    member this.OnCelestialMotionSatelliteRadiusRatioChanged() =
        CelestialMotionCommand.updateMoonMeshRadius planet celestialMotion ()
        CelestialMotionCommand.updateLunarDist planet celestialMotion ()

    member this.OnCelestialMotionSatelliteDistRatioChanged() =
        CelestialMotionCommand.updateLunarDist planet celestialMotion ()

    member this.CelestialMotionToggleAllMotions toggle =
        CelestialMotionCommand.toggleAllMotions celestialMotion toggle

    member this.OnChunkLoaderProcessed() =
        HexGridChunkService.onChunkLoaderProcessed env

    member this.OnHexGridChunkProcessed(chunk: IHexGridChunk) =
        HexGridChunkService.onHexGridChunkProcessed env chunk

    member this.OnPlanetHudOrbitCameraRigTransformed(transform, delta: float32) =
        PlanetHudCommand.onOrbitCameraRigTransformed cameraRig planetHud transform

    member this.OnPlanetHudRadiusLineEditTextSubmitted text =
        PlanetHudCommand.updateRadiusLineEdit planet planetHud text

    member this.OnPlanetHudDivisionLineEditTextSubmitted text =
        PlanetHudCommand.updateDivisionLineEdit planet planetHud false text

    member this.OnPlanetHudChunkDivisionLineEditTextSubmitted text =
        PlanetHudCommand.updateDivisionLineEdit planet planetHud true text

    member this.OnPlanetHudOrbitCameraRigMoved pos delta =
        PlanetHudCommand.onOrbitCameraRigMoved planetHud pos delta

    member this.OnProcessed delta =
        TileShaderDataCommand.updateData store tileShaderData delta

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()
        GD.Print $"[===DrawHexSphereMesh===] radius {planet.Radius}, divisions {planet.Divisions}, start at: {time}"
        // 清理旧数据
        HexSphereInitCommand.clearOldData store ()
        ChunkLoaderCommand.clearOldData chunkLoader ()
        lodMeshCache.RemoveAllLodMeshes()
        // 初始化新数据
        HexSphereService.initHexSphere env
        HexGridChunkService.initChunkNodes env

        store
            .Query<TileCountId, TileValue, TileFlag, TileVisibility>()
            .ForEachEntity(fun tileCountId tileValue tileFlag tileVisibility tile ->
                TileSearcherCommand.refreshTileSearchData tileSearcher tileCountId

                TileShaderDataCommand.refreshTerrain
                    tileShaderData
                    planet.UnitHeight
                    planet.MaxHeight
                    tileCountId
                    tileValue

                TileShaderDataCommand.refreshVisibility tileShaderData tile.Id tileCountId tileFlag tileVisibility)

        GD.Print $"[===DrawHexSphereMesh===] total cost: {Time.GetTicksMsec() - time} ms"

    member this.UpdateInsightChunks() =
        HexGridChunkService.updateInsightChunks env
