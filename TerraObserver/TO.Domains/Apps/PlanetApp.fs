namespace TO.Domains.Apps

open Friflo.Engine.ECS
open Godot
open TO.Domains.Envs
open TO.Domains.Types.Chunks
open TO.Domains.Types.DataStructures
open TO.Domains.Types.HexMeshes
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.PathFindings
open TO.Domains.Types.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 19:41:30
type PlanetApp(planet, catlikeCodingNoise, cameraRig, lonLatGrid, celestialMotion, chunkLoader, planetHud) =
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
            planetHud
        )

    member this.Init() =
        env.ResetOrbitCamera()
        env.DrawLonLatGridOnPlanet()

        if not <| Engine.IsEditorHint() then
            env.InitElevationAndWaterVSlider()

            env.PlanetHudOnOrbitCameraRigMoved <| env.GetOrbitCameraRigFocusBasePos() <| 0f

    member this.OnPlanetConfigParamsChanged() =
        env.OrbitCameraRigOnPlanetParamsChanged()
        env.OrbitCameraRigOnZoomChanged()
        env.DrawLonLatGridOnPlanet()
        // TODO: 判断 Radius 变化时才调用
        env.UpdateMoonMeshRadius()
        env.UpdateLunarDist()

    member this.OnOrbitCameraRigProcessed delta = env.OrbitCameraRigOnProcessed delta
    member this.OnOrbitCameraRigZoomChanged() = env.OrbitCameraRigOnZoomChanged()
    member this.OnLonLatGridDoDrawRequested() = env.DoDrawLonLatGrid()
    member this.OnLonLatGridCameraMoved(pos, delta) = env.LonLatGridOnCameraMoved pos delta
    member this.LonLatGridToggleFixFullVisibility toggle = env.ToggleFixFullVisibility toggle

    member this.OnCelestialMotionSatelliteRadiusRatioChanged() =
        env.UpdateMoonMeshRadius()
        env.UpdateLunarDist()

    member this.OnCelestialMotionSatelliteDistRatioChanged() = env.UpdateLunarDist()
    member this.CelestialMotionToggleAllMotions toggle = env.ToggleAllMotions toggle
    member this.OnChunkLoaderProcessed() = env.OnChunkLoaderProcessed()
    member this.OnHexGridChunkProcessed(chunk: IHexGridChunk) = env.OnHexGridChunkProcessed chunk

    member this.OnPlanetHudOrbitCameraRigTransformed(transform, delta: float32) =
        env.PlanetHudOnOrbitCameraRigTransformed transform

    member this.OnPlanetHudRadiusLineEditTextSubmitted text = env.UpdateRadiusLineEdit text

    member this.OnPlanetHudDivisionLineEditTextSubmitted text = env.UpdateDivisionLineEdit false text

    member this.OnPlanetHudChunkDivisionLineEditTextSubmitted text = env.UpdateDivisionLineEdit true text

    member this.OnPlanetHudOrbitCameraRigMoved pos delta =
        env.PlanetHudOnOrbitCameraRigMoved pos delta

    member this.OnProcessed delta = env.UpdateTileShaderData delta

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()
        GD.Print $"[===DrawHexSphereMesh===] radius {planet.Radius}, divisions {planet.Divisions}, start at: {time}"
        // 清理旧数据
        env.ClearHexSphereOldData()
        env.ClearChunkLoaderOldData()
        env.RemoveAllLodMeshes()
        // 初始化新数据
        env.InitHexSphere()
        env.InitChunkNodes()

        store
            .Query<TileCountId, TileValue, TileFlag, TileVisibility>()
            .ForEachEntity(fun tileCountId tileValue tileFlag tileVisibility tile ->
                env.RefreshTileSearchData tileCountId
                env.RefreshTileShaderDataTerrain planet.UnitHeight planet.MaxHeight tileCountId tileValue
                env.RefreshTileShaderDataVisibility tile.Id tileCountId tileFlag tileVisibility)

        GD.Print $"[===DrawHexSphereMesh===] total cost: {Time.GetTicksMsec() - time} ms"

    member this.UpdateInsightChunks() = env.UpdateInsightChunks()
