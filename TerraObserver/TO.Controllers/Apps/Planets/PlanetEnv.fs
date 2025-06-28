namespace TO.Controllers.Apps.Planets

open TO.Presenters.Models.Planets
open TO.Presenters.Views.Cameras
open TO.Presenters.Views.Chunks
open TO.Presenters.Views.Geos
open TO.Presenters.Views.Planets
open TO.Presenters.Views.Uis
open TO.Repos.Commands.HexSpheres
open TO.Repos.Commands.Meshes
open TO.Repos.Commands.PathFindings
open TO.Repos.Commands.Shaders
open TO.Repos.Queries.Friflos
open TO.Repos.Queries.HexSpheres
open TO.Repos.Queries.Meshes

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:33:19
type PlanetEnv
    (
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
    ) =
    interface IPlanetQuery with
        member this.GetRadius = PlanetQuery.getRadius planet
        member this.GetDivisions = PlanetQuery.getDivisions planet
        member this.GetChunkDivisions = PlanetQuery.getChunkDivisions planet
        member this.GetUnitHeight = PlanetQuery.getUnitHeight planet
        member this.GetMaxHeight = PlanetQuery.getMaxHeight planet
        member this.GetStandardScale = PlanetQuery.getStandardScale planet
        member this.GetTileLen = PlanetQuery.getTileLen planet

    interface ICatlikeCodingNoiseQuery with
        member this.SampleHashGrid = CatlikeCodingNoiseQuery.sampleHashGrid catlikeCodingNoise
        member this.SampleNoise = CatlikeCodingNoiseQuery.sampleNoise planet catlikeCodingNoise
        member this.GetHeight = CatlikeCodingNoiseQuery.getHeight this
        member this.Perturb = CatlikeCodingNoiseQuery.perturb this

    interface IOrbitCameraRigQuery with
        member this.GetFocusBasePos = OrbitCameraRigQuery.getFocusBasePos cameraRig
        member this.IsAutoPiloting = OrbitCameraRigQuery.isAutoPiloting cameraRig

    interface IChunkLoaderQuery with
        member this.GetInsightChunkIdsNow = ChunkLoaderQuery.getInsightChunkIdsNow chunkLoader

        member this.GetInsightChunkIdsNext =
            ChunkLoaderQuery.getInsightChunkIdsNext chunkLoader

        member this.GetChunkQueryQueue = ChunkLoaderQuery.getChunkQueryQueue chunkLoader
        member this.GetVisitedChunkIds = ChunkLoaderQuery.getVisitedChunkIds chunkLoader
        member this.GetRimChunkIds = ChunkLoaderQuery.getRimChunkIds chunkLoader
        member this.GetLoadSet = ChunkLoaderQuery.getLoadSet chunkLoader
        member this.GetRefreshSet = ChunkLoaderQuery.getRefreshSet chunkLoader
        member this.GetUnloadSet = ChunkLoaderQuery.getUnloadSet chunkLoader
        member this.GetUsingChunks = ChunkLoaderQuery.getUsingChunks chunkLoader
        member this.GetStopwatch = ChunkLoaderQuery.getStopwatch chunkLoader
        member this.GetUnusedChunk = ChunkLoaderQuery.getUnusedChunk chunkLoader

        member this.UpdateInsightSetNextIdx =
            ChunkLoaderQuery.updateInsightSetNextIdx chunkLoader

        member this.TryGetUsingChunk = ChunkLoaderQuery.tryGetUsingChunk chunkLoader
        member this.GetAllUsingChunks = ChunkLoaderQuery.getAllUsingChunks chunkLoader
        member this.GetViewportCamera = ChunkLoaderQuery.getViewportCamera chunkLoader

    interface IOrbitCameraRigCommand with
        member this.OnPlanetParamsChanged =
            OrbitCameraRigCommand.onPlanetParamsChanged planet cameraRig

        member this.OnProcessed = OrbitCameraRigCommand.onProcessed planet cameraRig
        member this.Reset = OrbitCameraRigCommand.reset planet cameraRig
        member this.SetAutoPilot = OrbitCameraRigCommand.setAutoPilot cameraRig
        member this.CancelAutoPilot = OrbitCameraRigCommand.cancelAutoPilot cameraRig
        member this.RotateCamera = OrbitCameraRigCommand.rotateCamera cameraRig
        member this.OnZoomChanged = OrbitCameraRigCommand.onZoomChanged planet cameraRig

    interface ILonLatGridCommand with
        member this.DoDraw = LonLatGridCommand.doDraw lonLatGrid
        member this.DrawOnPlanet = LonLatGridCommand.drawOnPlanet planet lonLatGrid
        member this.OnCameraMoved = LonLatGridCommand.onCameraMoved lonLatGrid

        member this.ToggleFixFullVisibility =
            LonLatGridCommand.toggleFixFullVisibility lonLatGrid

    interface ICelestialMotionCommand with
        member this.ToggleAllMotions = CelestialMotionCommand.toggleAllMotions celestialMotion

        member this.UpdateLunarDist =
            CelestialMotionCommand.updateLunarDist planet celestialMotion

        member this.UpdateMoonMeshRadius =
            CelestialMotionCommand.updateMoonMeshRadius planet celestialMotion


    interface IChunkLoaderCommand with
        member this.ClearOldData = ChunkLoaderCommand.clearOldData chunkLoader
        member this.AddUsingChunk = ChunkLoaderCommand.addUsingChunk chunkLoader
        member this.HideChunk = ChunkLoaderCommand.hideChunk chunkLoader
        member this.SetProcess = ChunkLoaderCommand.setProcess chunkLoader

        member this.EnqueueQueryIfNotVisited =
            ChunkLoaderCommand.enqueueQueryIfNotVisited chunkLoader

    interface IPlanetHudCommand with
        member this.OnOrbitCameraRigMoved = PlanetHudCommand.onOrbitCameraRigMoved planetHud

        member this.OnOrbitCameraRigTransformed =
            PlanetHudCommand.onOrbitCameraRigTransformed cameraRig planetHud

        member this.InitElevationAndWaterVSlider =
            PlanetHudCommand.initElevationAndWaterVSlider planet planetHud

        member this.UpdateRadiusLineEdit =
            PlanetHudCommand.updateRadiusLineEdit planet planetHud

        member this.UpdateDivisionLineEdit =
            PlanetHudCommand.updateDivisionLineEdit planet planetHud

    interface IEntityStoreQuery with
        member this.GetEntityById = EntityStoreQuery.getEntityById store
        member this.GetEntityStore = EntityStoreQuery.getEntityStore store
        member this.Query() = EntityStoreQuery.query store ()

    interface IPointQuery with
        member this.GetNeighborByIdAndIdx = PointQuery.getNeighborByIdAndIdx store
        member this.GetNeighborCenterPointIds = PointQuery.getNeighborCenterPointIds store
        member this.GetNeighborIdsById = PointQuery.getNeighborIdsById store
        member this.GetNeighborIdx = PointQuery.getNeighborIdx store
        member this.SearchNearestCenterPos = PointQuery.searchNearestCenterPos chunkyVpTrees
        member this.TryHeadByPosition = PointQuery.tryHeadByPosition store
        member this.TryHeadEntityByCenterId = PointQuery.tryHeadEntityByCenterId store

    interface IChunkQuery with
        member this.IsHandlingLodGaps = ChunkQuery.isHandlingLodGaps store
        member this.GetLod = ChunkQuery.getLod store

    interface ILodMeshCacheQuery with
        member this.GetLodMeshes = LodMeshCacheQuery.getLodMeshes lodMeshCache

    interface IChunkCommand with
        member this.Add = ChunkCommand.add store
        member this.UpdateInsightAndLod = ChunkCommand.updateInsightAndLod store

    interface IHexSphereInitCommand with
        member this.ClearOldData = HexSphereInitCommand.clearOldData store
        member this.InitChunks = HexSphereInitCommand.initChunks store chunkyVpTrees
        member this.InitTiles = HexSphereInitCommand.initTiles store chunkyVpTrees

    interface ILodMeshCacheCommand with
        member this.AddLodMeshes = LodMeshCacheCommand.addLodMeshes lodMeshCache

    interface ITileSearcherCommand with
        member this.InitSearchData = TileSearcherCommand.initSearchData store tileSearcher

        member this.RefreshTileSearchData =
            TileSearcherCommand.refreshTileSearchData tileSearcher

    interface ITileShaderDataCommand with
        member this.InitShaderData = TileShaderDataCommand.initShaderData tileShaderData
        member this.RefreshCiv = TileShaderDataCommand.refreshCiv tileShaderData
        member this.RefreshTerrain = TileShaderDataCommand.refreshTerrain tileShaderData
        member this.RefreshVisibility = TileShaderDataCommand.refreshVisibility tileShaderData
        member this.UpdateData = TileShaderDataCommand.updateData store tileShaderData

        member this.ViewElevationChanged =
            TileShaderDataCommand.viewElevationChanged tileShaderData
