namespace TO.Domains.Apps

open Friflo.Engine.ECS
open Godot
open TO.Domains.Envs
open TO.Domains.Types.Cameras
open TO.Domains.Types.Chunks
open TO.Domains.Types.DataStructures
open TO.Domains.Types.Geos
open TO.Domains.Types.HexMeshes
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.Maps
open TO.Domains.Types.PathFindings
open TO.Domains.Types.PlanetHuds
open TO.Domains.Types.Planets
open TO.Domains.Types.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 19:41:30
type PlanetApp
    (
        planet,
        catlikeCodingNoise,
        cameraRig,
        lonLatGrid,
        celestialMotion,
        chunkLoader,
        selectTileViewer,
        miniMapManager,
        hexMapGenerator,
        planetHud
    ) =
    let store = EntityStore()

    let chunkyVpTrees: ChunkyVpTrees =
        { ChunkVpTree = VpTree<Vector3>()
          TileVpTree = VpTree<Vector3>() }

    let tileShaderData = TileShaderData()
    let tileSearcher = TileSearcher()
    let lodMeshCache = LodMeshCache()

    let env =
        PlanetEnv(
            // 后端
            store,
            chunkyVpTrees,
            lodMeshCache,
            tileSearcher,
            tileShaderData,
            // 前端
            planet,
            catlikeCodingNoise,
            cameraRig,
            lonLatGrid,
            celestialMotion,
            chunkLoader,
            selectTileViewer,
            miniMapManager,
            hexMapGenerator,
            planetHud
        )

    let init
        (env:
            'E
                when 'E :> IOrbitCameraRigQuery
                and 'E :> IOrbitCameraRigCommand
                and 'E :> ILonLatGridCommand
                and 'E :> IMiniMapManagerCommand
                and 'E :> IPlanetHudCommand)
        =
        env.Reset()
        env.DrawOnPlanet()

        if not <| Engine.IsEditorHint() then
            env.InitElevationAndWaterVSlider()
            env.OnOrbitCameraRigMoved <| env.GetFocusBasePos() <| 0f
            env.InitRectMiniMap()
            env.InitMiniMap <| env.GetFocusBasePos()

    let onPlanetConfigParamsChanged
        (env: 'E when 'E :> IOrbitCameraRigCommand and 'E :> ILonLatGridCommand and 'E :> ICelestialMotionCommand)
        =
        env.OnPlanetParamsChanged()
        env.OnZoomChanged()
        env.DrawOnPlanet()
        // TODO: 判断 Radius 变化时才调用
        env.UpdateMoonMeshRadius()
        env.UpdateLunarDist()

    let onOrbitCameraRigProcessed (env: #IOrbitCameraRigCommand) delta = env.OnProcessed delta
    let onOrbitCameraRigZoomChanged (env: #IOrbitCameraRigCommand) = env.OnZoomChanged()
    let onLonLatGridDoDrawRequested (env: #ILonLatGridCommand) = env.DoDraw()
    let onLonLatGridCameraMoved (env: #ILonLatGridCommand) pos delta = env.OnCameraMoved pos delta
    let lonLatGridToggleFixFullVisibility (env: #ILonLatGridCommand) toggle = env.ToggleFixFullVisibility toggle

    let onCelestialMotionSatelliteRadiusRatioChanged (env: #ICelestialMotionCommand) =
        env.UpdateMoonMeshRadius()
        env.UpdateLunarDist()

    let onCelestialMotionSatelliteDistRatioChanged (env: #ICelestialMotionCommand) = env.UpdateLunarDist()
    let celestialMotionToggleAllMotions (env: #ICelestialMotionCommand) toggle = env.ToggleAllMotions toggle
    let onChunkLoaderProcessed (env: #IChunkLoaderCommand) = env.OnChunkLoaderProcessed()
    let onHexGridChunkProcessed (env: #IChunkLoaderCommand) (chunk: IHexGridChunk) = env.OnHexGridChunkProcessed chunk
    let onMiniMapClicked (env: #IOrbitCameraRigCommand) (v: Vector3) = env.SetAutoPilot v
    let onPlanetHudChosenTileIdChanged (env: #IPlanetHudCommand) = env.UpdateChosenTileInfo planetHud

    let onPlanetHudOrbitCameraRigTransformed (env: #IPlanetHudCommand) transform =
        env.OnOrbitCameraRigTransformed transform

    let onPlanetHudRadiusLineEditTextSubmitted (env: #IPlanetHudCommand) text = env.UpdateRadiusLineEdit text

    let onPlanetHudDivisionLineEditTextSubmitted (env: #IPlanetHudCommand) chunky text =
        env.UpdateDivisionLineEdit chunky text

    let onPlanetHudOrbitCameraRigMoved
        (env: 'E when 'E :> IPlanetHudCommand and 'E :> IMiniMapManagerCommand)
        pos
        delta
        =
        env.OnOrbitCameraRigMoved pos delta
        env.SyncCameraIconPos pos

    let onProcessed
        (env: 'E when 'E :> ITileShaderDataCommand and 'E :> ISelectTileViewerCommand and 'E :> IPlanetHudCommand)
        delta
        =
        env.UpdateTileShaderData delta
        env.UpdateInEditMode()
        env.OnPlanetHudProcessed()

    let drawHexSphereMesh
        (env:
            'E
                when 'E :> IHexSphereInitCommand
                and 'E :> IChunkLoaderCommand
                and 'E :> ILodMeshCacheCommand
                and 'E :> ITileSearcherCommand
                and 'E :> ITileShaderDataCommand
                and 'E :> IHexMapGeneratorCommand)
        =
        let time = Time.GetTicksMsec()
        GD.Print $"[===DrawHexSphereMesh===] radius {planet.Radius}, divisions {planet.Divisions}, start at: {time}"
        // 清理旧数据
        env.ClearHexSphereOldData()
        env.ClearChunkLoaderOldData()
        env.RemoveAllLodMeshes()
        // 初始化新数据
        env.InitHexSphere()
        env.GenerateMap()
        env.InitChunkNodes()

        store
            .Query<TileCountId, TileValue, TileFlag, TileVisibility>()
            .ForEachEntity(fun tileCountId tileValue tileFlag tileVisibility tile ->
                env.RefreshTileSearchData tileCountId
                env.RefreshTileShaderDataTerrain tileCountId tileValue
                env.RefreshTileShaderDataVisibility tile.Id tileCountId tileFlag tileVisibility)

        GD.Print $"[===DrawHexSphereMesh===] total cost: {Time.GetTicksMsec() - time} ms"

    let updateInsightChunks (env: #IChunkLoaderCommand) = env.UpdateInsightChunks()

    member this.Init() = init env
    member this.OnPlanetConfigParamsChanged() = onPlanetConfigParamsChanged env
    member this.OnOrbitCameraRigProcessed delta = onOrbitCameraRigProcessed env delta
    member this.OnOrbitCameraRigZoomChanged() = onOrbitCameraRigZoomChanged env
    member this.OnLonLatGridDoDrawRequested() = onLonLatGridDoDrawRequested env
    member this.OnLonLatGridCameraMoved(pos, delta) = onLonLatGridCameraMoved env pos delta

    member this.LonLatGridToggleFixFullVisibility toggle =
        lonLatGridToggleFixFullVisibility env toggle

    member this.OnCelestialMotionSatelliteRadiusRatioChanged() =
        onCelestialMotionSatelliteRadiusRatioChanged env

    member this.OnCelestialMotionSatelliteDistRatioChanged() =
        onCelestialMotionSatelliteDistRatioChanged env

    member this.CelestialMotionToggleAllMotions toggle =
        celestialMotionToggleAllMotions env toggle

    member this.OnChunkLoaderProcessed() = onChunkLoaderProcessed env
    member this.OnHexGridChunkProcessed(chunk: IHexGridChunk) = onHexGridChunkProcessed env chunk
    member this.OnMiniMapClicked(v: Vector3) = onMiniMapClicked env v
    member this.OnPlanetHudChosenTileIdChanged _ = onPlanetHudChosenTileIdChanged env

    member this.OnPlanetHudOrbitCameraRigTransformed(transform, _) =
        onPlanetHudOrbitCameraRigTransformed env transform

    member this.OnPlanetHudRadiusLineEditTextSubmitted text =
        onPlanetHudRadiusLineEditTextSubmitted env text

    member this.OnPlanetHudDivisionLineEditTextSubmitted text =
        onPlanetHudDivisionLineEditTextSubmitted env false text

    member this.OnPlanetHudChunkDivisionLineEditTextSubmitted text =
        onPlanetHudDivisionLineEditTextSubmitted env true text

    member this.OnPlanetHudOrbitCameraRigMoved pos delta =
        onPlanetHudOrbitCameraRigMoved env pos delta

    member this.OnProcessed delta = onProcessed env delta
    member this.DrawHexSphereMesh() = drawHexSphereMesh env
    member this.UpdateInsightChunks() = updateInsightChunks env
