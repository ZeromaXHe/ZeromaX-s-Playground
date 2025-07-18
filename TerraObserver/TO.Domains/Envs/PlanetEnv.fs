namespace TO.Domains.Envs

open Friflo.Engine.ECS
open TO.Domains.Functions.Cameras
open TO.Domains.Functions.Chunks
open TO.Domains.Functions.Configs
open TO.Domains.Functions.Friflos
open TO.Domains.Functions.Geos
open TO.Domains.Functions.HexMeshes
open TO.Domains.Functions.HexSpheres
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.Maps
open TO.Domains.Functions.PathFindings
open TO.Domains.Functions.PlanetHuds
open TO.Domains.Functions.Planets
open TO.Domains.Functions.Shaders
open TO.Domains.Functions.Units
open TO.Domains.Types.Cameras
open TO.Domains.Types.Chunks
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.Geos
open TO.Domains.Types.HexMeshes
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.Maps
open TO.Domains.Types.PathFindings
open TO.Domains.Types.PlanetHuds
open TO.Domains.Types.Planets
open TO.Domains.Types.Shaders
open TO.Domains.Types.Units

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:33:19
[<Struct>]
type PlanetEnv =
    // 后端
    val Store: EntityStore
    val ChunkyVpTrees: ChunkyVpTrees
    val LodMeshCache: LodMeshCache
    val TileSearcher: TileSearcher
    val TileShaderData: TileShaderData
    // 前端
    val PlanetConfig: IPlanetConfig
    val CatlikeCodingNoise: ICatlikeCodingNoise
    val CameraRig: IOrbitCameraRig
    val LonLatGrid: ILonLatGrid
    val CelestialMotion: ICelestialMotion
    val UnitManager: IUnitManager
    val HexUnitPathPool: IHexUnitPathPool
    val FeatureMeshManager: IFeatureMeshManager
    val FeaturePreviewManager: IFeaturePreviewManager
    val ChunkLoader: IChunkLoader
    val SelectTileViewer: ISelectTileViewer
    val EditPreviewChunk: IEditPreviewChunk
    val MiniMapManager: IMiniMapManager
    val HexMapGenerator: IHexMapGenerator
    val PlanetHud: IPlanetHud

    new
        (
            // 后端
            store,
            chunkyVpTrees,
            lodMeshCache,
            tileSearcher,
            tileShaderData,
            // 前端
            planetConfig,
            catlikeCodingNoise,
            cameraRig,
            lonLatGrid,
            celestialMotion,
            unitManager,
            hexUnitPathPool,
            featureMeshManager,
            featurePreviewManager,
            chunkLoader,
            selectTileViewer,
            editPreviewChunk,
            miniMapManager,
            hexMapGenerator,
            planetHud
        ) =
        {
          // 后端
          Store = store
          ChunkyVpTrees = chunkyVpTrees
          LodMeshCache = lodMeshCache
          TileSearcher = tileSearcher
          TileShaderData = tileShaderData
          // 前端
          PlanetConfig = planetConfig
          CatlikeCodingNoise = catlikeCodingNoise
          CameraRig = cameraRig
          LonLatGrid = lonLatGrid
          CelestialMotion = celestialMotion
          UnitManager = unitManager
          HexUnitPathPool = hexUnitPathPool
          FeatureMeshManager = featureMeshManager
          FeaturePreviewManager = featurePreviewManager
          ChunkLoader = chunkLoader
          SelectTileViewer = selectTileViewer
          EditPreviewChunk = editPreviewChunk
          MiniMapManager = miniMapManager
          HexMapGenerator = hexMapGenerator
          PlanetHud = planetHud }

    interface IEntityStoreQuery with
        member this.EntityStore = this.Store
        member this.GetEntityById = EntityStoreQuery.getEntityById this

        member this.Query<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> IComponent and 'T :> System.ValueType>
            ()
            =
            EntityStoreQuery.query<'T, PlanetEnv> this ()

    interface IEntityStoreCommand with
        member this.ExecuteInCommandBuffer = EntityStoreCommand.executeInCommandBuffer this

    interface IPointQuery with
        member this.TryHeadByPosition = PointQuery.tryHeadByPosition this
        member this.TryHeadEntityByCenterId = PointQuery.tryHeadEntityByCenterId this
        member this.GetNeighborByIdAndIdx = PointQuery.getNeighborByIdAndIdx this
        member this.GetNeighborIdx = PointQuery.getNeighborIdx this
        member this.GetNeighborIdsById = PointQuery.getNeighborIdsById this
        member this.GetNeighborCenterPointIds = PointQuery.getNeighborCenterPointIds this
        member this.SearchNearestCenterPos = PointQuery.searchNearestCenterPos this

    interface IPointCommand with
        member this.AddPoint = PointCommand.add this
        member this.CreateVpTree = PointCommand.createVpTree this

    interface IFaceQuery with
        member this.GetOrderedFaces = FaceQuery.getOrderedFaces this
        member this.GetFaceCenter = FaceQuery.getFaceCenter this

    interface IFaceCommand with
        member this.AddFace = FaceCommand.add this

    interface ITileQuery with
        member this.GetTile = TileQuery.getTile this
        member this.GetTileByCountId = TileQuery.getTileByCountId this
        member this.GetTilesByChunkId = TileQuery.getTilesByChunkId this
        member this.GetAllTiles = TileQuery.getAllTiles this
        member this.GetSphereAxial = TileQuery.getSphereAxial this
        member this.IsNeighborTile = TileQuery.isNeighborTile this
        member this.GetNeighborTileByIdx = TileQuery.getNeighborTileByIdx this
        member this.GetNeighborTiles = TileQuery.getNeighborTiles this
        member this.GetTilesInDistance = TileQuery.getTilesInDistance this

    interface ITileCommand with
        member this.AddTile = TileCommand.add this
        member this.AddTileOtherComponents = TileCommand.addTileOtherComponents this
        member this.RemoveRoads = TileCommand.removeRoads this
        member this.AddRoad = TileCommand.addRoad this
        member this.RemoveRivers = TileCommand.removeRivers this
        member this.SetOutgoingRiver = TileCommand.setOutgoingRiver this
        member this.SetElevation = TileCommand.setElevation this
        member this.SetTerrainTypeIndex = TileCommand.setTerrainTypeIndex this
        member this.SetWaterLevel = TileCommand.setWaterLevel this
        member this.SetUrbanLevel = TileCommand.setUrbanLevel this
        member this.SetFarmLevel = TileCommand.setFarmLevel this
        member this.SetPlantLevel = TileCommand.setPlantLevel this
        member this.SetWalled = TileCommand.setWalled this
        member this.SetSpecialIndex = TileCommand.setSpecialIndex this

    interface IChunkQuery with
        member this.IsHandlingLodGaps = ChunkQuery.isHandlingLodGaps this
        member this.GetChunkLod = ChunkQuery.getLod this

    interface IChunkCommand with
        member this.AddChunk = ChunkCommand.add this
        member this.UpdateChunkInsightAndLod = ChunkCommand.updateChunkInsightAndLod this

    interface IHexSphereQuery with
        member this.GetHexFacesAndNeighborCenterIds =
            HexSphereQuery.getHexFacesAndNeighborCenterIds this

        member this.SearchNearest = HexSphereQuery.searchNearest this

    interface IHexSphereInitCommand with
        member this.InitChunks = HexSphereInitCommand.initChunks this
        member this.InitTiles = HexSphereInitCommand.initTiles this
        member this.ClearHexSphereOldData = HexSphereInitCommand.clearOldData this
        member this.InitHexSphere = HexSphereInitCommand.initHexSphere this

    interface IChunkyVpTreesQuery with
        member this.ChunkyVpTrees = this.ChunkyVpTrees
        member this.ChooseVpTreeByChunky = ChunkyVpTreesQuery.chooseByChunky this

    interface ILodMeshCacheQuery with
        member this.LodMeshCache = this.LodMeshCache
        member this.GetLodMeshes = LodMeshCacheQuery.getLodMeshes this

    interface ILodMeshCacheCommand with
        member this.AddLodMeshes = LodMeshCacheCommand.addLodMeshes this
        member this.RemoveAllLodMeshes = LodMeshCacheCommand.removeAllLodMeshes this
        member this.RemoveLodMeshes = LodMeshCacheCommand.removeLodMeshes this

    interface ITileSearcherQuery with
        member this.TileSearcher = this.TileSearcher

    interface ITileSearcherCommand with
        member this.InitSearchData = TileSearcherCommand.initSearchData this
        member this.RefreshTileSearchData = TileSearcherCommand.refreshTileSearchData this
        member this.GetVisibleTiles = TileSearcherCommand.getVisibleTiles this
        member this.FindTileSearchPath = TileSearcherCommand.findTileSearchPath this
        member this.ClearTileSearchPath = TileSearcherCommand.clearTileSearchPath this

    interface ITileShaderDataQuery with
        member this.TileShaderData = this.TileShaderData

    interface ITileShaderDataCommand with
        member this.InitShaderData = TileShaderDataCommand.initShaderData this
        member this.RefreshTileShaderDataCiv = TileShaderDataCommand.refreshCiv this
        member this.RefreshTileShaderDataTerrain = TileShaderDataCommand.refreshTerrain this

        member this.RefreshTileShaderDataVisibility =
            TileShaderDataCommand.refreshVisibility this

        member this.UpdateTileShaderData = TileShaderDataCommand.updateData this
        member this.ViewElevationChanged = TileShaderDataCommand.viewElevationChanged this

    interface IFeatureQuery with
        member this.GetFeatureMultiMesh = FeatureQuery.getMultiMesh this

    interface IFeatureCommand with
        member this.AddFeature = FeatureCommand.add this
        member this.ShowFeatures = FeatureCommand.showFeatures this
        member this.HideFeatures = FeatureCommand.hideFeatures this
        member this.DeleteFeatures = FeatureCommand.deleteFeatures this
        member this.AddTower = FeatureCommand.addTower this
        member this.AddBridge = FeatureCommand.addBridge this
        member this.AddSpecialFeature = FeatureCommand.addSpecialFeature this
        member this.AddNormalFeature = FeatureCommand.addNormalFeature this

    interface IPlanetConfigQuery with
        member this.PlanetConfig = this.PlanetConfig
        member this.GetTileLen = PlanetConfigQuery.getTileLen this

    interface ICatlikeCodingNoiseQuery with
        member this.CatlikeCodingNoise = this.CatlikeCodingNoise
        member this.SampleHashGrid = CatlikeCodingNoiseQuery.sampleHashGrid this
        member this.SampleNoise = CatlikeCodingNoiseQuery.sampleNoise this
        member this.Perturb = CatlikeCodingNoiseQuery.perturb this
        member this.GetHeight = CatlikeCodingNoiseQuery.getHeight this
        member this.GetPerturbHeight = CatlikeCodingNoiseQuery.getPerturbHeight this

    interface ICatlikeCodingNoiseCommand with
        member this.InitializeHashGrid = CatlikeCodingNoiseCommand.initializeHashGrid this

    interface IOrbitCameraRigQuery with
        member this.OrbitCameraRig = this.CameraRig
        member this.GetFocusBasePos = OrbitCameraRigQuery.getFocusBasePos this
        member this.IsAutoPiloting = OrbitCameraRigQuery.isAutoPiloting this

    interface IOrbitCameraRigCommand with
        member this.OnZoomChanged = OrbitCameraRigCommand.onZoomChanged this

        member this.OnPlanetParamsChanged = OrbitCameraRigCommand.onPlanetParamsChanged this

        member this.Reset = OrbitCameraRigCommand.reset this
        member this.SetAutoPilot = OrbitCameraRigCommand.setAutoPilot this
        member this.CancelAutoPilot = OrbitCameraRigCommand.cancelAutoPilot this
        member this.RotateCamera = OrbitCameraRigCommand.rotateCamera this
        member this.OnProcessed = OrbitCameraRigCommand.onProcessed this

    interface ILonLatGridQuery with
        member this.LonLatGrid = this.LonLatGrid

    interface ILonLatGridCommand with
        member this.DoDraw = LonLatGridCommand.doDraw this
        member this.DrawOnPlanet = LonLatGridCommand.drawOnPlanet this
        member this.ToggleFixFullVisibility = LonLatGridCommand.toggleFixFullVisibility this
        member this.OnCameraMoved = LonLatGridCommand.onCameraMoved this

    interface ICelestialMotionQuery with
        member this.CelestialMotion = this.CelestialMotion

    interface ICelestialMotionCommand with
        member this.ToggleAllMotions = CelestialMotionCommand.toggleAllMotions this
        member this.UpdateLunarDist = CelestialMotionCommand.updateLunarDist this
        member this.UpdateMoonMeshRadius = CelestialMotionCommand.updateMoonMeshRadius this

    interface IHexUnitCommand with
        member this.OnHexUnitProcessed = HexUnitCommand.onHexUnitProcessed this
        member this.ValidateUnitLocation = HexUnitCommand.validateUnitLocation this
        member this.ChangeUnitTileId = HexUnitCommand.changeUnitTileId this
        member this.KillUnit = HexUnitCommand.killUnit this
        member this.TravelUnit = HexUnitCommand.travelUnit this

    interface IHexUnitPathPoolCommand with
        member this.NewUnitPathTask = HexUnitPathPoolCommand.newUnitPathTask this

    interface IUnitManagerQuery with
        member this.UnitManagerOpt =
            if this.UnitManager <> null then
                Some this.UnitManager
            else
                None

    interface IUnitManagerCommand with
        member this.AddUnit = UnitManagerCommand.addUnit this
        member this.RemoveUnit = UnitManagerCommand.removeUnit this
        member this.ValidateUnitLocationById = UnitManagerCommand.validateUnitLocationById this
        member this.FindUnitPath = UnitManagerCommand.findUnitPath this
        member this.ClearAllUnits = UnitManagerCommand.clearAllUnits this

    interface IHexUnitPathPoolQuery with
        member this.HexUnitPathPoolOpt =
            if this.HexUnitPathPool <> null then
                Some this.HexUnitPathPool
            else
                None

    interface IFeatureMeshManagerQuery with
        member this.FeatureMeshManager = this.FeatureMeshManager

    interface IFeaturePreviewManagerCommand with
        member this.ClearFeaturePreviewManagerOldData =
            FeaturePreviewManagerCommand.clearFeaturePreviewManagerOldData this

        member this.ShowFeaturePreview = FeaturePreviewManagerCommand.showFeaturePreview this
        member this.HideFeaturePreview = FeaturePreviewManagerCommand.hideFeaturePreview this

    interface IFeaturePreviewManagerQuery with
        member this.FeaturePreviewManager = this.FeaturePreviewManager

    interface IFeatureMeshManagerCommand with
        member this.ClearFeatureMashOldData =
            FeatureMeshManagerCommand.clearFeatureMashOldData this

        member this.HideFeatureMesh = FeatureMeshManagerCommand.hideFeatureMesh this
        member this.ShowFeatureMesh = FeatureMeshManagerCommand.showFeatureMesh this

    interface IChunkLoaderQuery with
        member this.ChunkLoader = this.ChunkLoader
        member this.TryGetUsingChunk = ChunkLoaderQuery.tryGetUsingChunk this
        member this.GetAllUsingChunks = ChunkLoaderQuery.getAllUsingChunks this
        member this.GetViewportCamera = ChunkLoaderQuery.getViewportCamera this

    interface IChunkLoaderCommand with
        member this.ResetInsightSetIdx = ChunkLoaderCommand.resetInsightSetIdx this
        member this.UpdateInsightSetNextIdx = ChunkLoaderCommand.updateInsightSetNextIdx this
        member this.ClearChunkLoaderOldData = ChunkLoaderCommand.clearOldData this
        member this.AddUsingChunk = ChunkLoaderCommand.addUsingChunk this
        member this.OnChunkLoaderProcessed = ChunkLoaderCommand.onChunkLoaderProcessed this
        member this.InitChunkNodes = ChunkLoaderCommand.initChunkNodes this
        member this.OnHexGridChunkProcessed = ChunkLoaderCommand.onHexGridChunkProcessed this
        member this.UpdateInsightChunks = ChunkLoaderCommand.updateInsightChunks this
        member this.RefreshChunk = ChunkLoaderCommand.refreshChunk this

    interface ITileOverriderQuery with
        member this.GetOverrideEdgeType = TileOverriderQuery.getOverrideEdgeType this
        member this.GetOverrideElevation = TileOverriderQuery.getOverrideElevation this
        member this.GetOverrideFarmLevel = TileOverriderQuery.getOverrideFarmLevel this
        member this.GetOverrideHeight = TileOverriderQuery.getOverrideHeight this
        member this.GetOverridePlantLevel = TileOverriderQuery.getOverridePlantLevel this
        member this.GetOverrideRiverSurfaceY = TileOverriderQuery.getOverrideRiverSurfaceY this
        member this.GetOverrideSpecialIndex = TileOverriderQuery.getOverrideSpecialIndex this
        member this.GetOverrideStreamBedY = TileOverriderQuery.getOverrideStreamBedY this
        member this.GetOverrideUrbanLevel = TileOverriderQuery.getOverrideUrbanLevel this
        member this.GetOverrideWalled = TileOverriderQuery.getOverrideWalled this
        member this.GetOverrideWaterLevel = TileOverriderQuery.getOverrideWaterLevel this
        member this.GetOverrideWaterSurfaceY = TileOverriderQuery.getOverrideWaterSurfaceY this
        member this.HasOverrideIncomingRiver = TileOverriderQuery.hasOverrideIncomingRiver this

        member this.HasOverrideIncomingRiverThroughEdge =
            TileOverriderQuery.hasOverrideIncomingRiverThroughEdge this

        member this.HasOverrideOutgoingRiver = TileOverriderQuery.hasOverrideOutgoingRiver this

        member this.HasOverrideRiverBeginOrEnd =
            TileOverriderQuery.hasOverrideRiverBeginOrEnd this

        member this.HasOverrideRiverThroughEdge =
            TileOverriderQuery.hasOverrideRiverThroughEdge this

        member this.HasOverrideRivers = TileOverriderQuery.hasOverrideRivers this

        member this.HasOverrideRoadThroughEdge =
            TileOverriderQuery.hasOverrideRoadThroughEdge this

        member this.HasOverrideRoads = TileOverriderQuery.hasOverrideRoads this
        member this.IsOverrideSpecial = TileOverriderQuery.isOverrideSpecial this
        member this.IsOverrideUnderwater = TileOverriderQuery.isOverrideUnderwater this

        member this.IsOverridingTileConnection =
            TileOverriderQuery.isOverridingTileConnection this

    interface IChunkTriangulationCommand with
        member this.Triangulate = ChunkTriangulationCommand.triangulate this

    interface ISelectTileViewerQuery with
        member this.SelectTileViewerOpt =
            if this.SelectTileViewer = null then
                None
            else
                Some this.SelectTileViewer

        member this.GetTileIdUnderCursor = SelectTileViewerQuery.getTileIdUnderCursor this

    interface ISelectTileViewerCommand with
        member this.UpdateInEditMode = SelectTileViewerCommand.updateInEditMode this
        member this.UpdateInPlayMode = SelectTileViewerCommand.updateInPlayMode this

    interface IEditPreviewChunkQuery with
        member this.EditPreviewChunkOpt =
            if this.EditPreviewChunk = null then
                None
            else
                Some this.EditPreviewChunk

    interface IEditPreviewChunkCommand with
        member this.RefreshEditPreview = EditPreviewChunkCommand.refreshEditPreview this

    interface IMiniMapManagerQuery with
        member this.MiniMapManagerOpt =
            if this.MiniMapManager = null then
                None
            else
                Some this.MiniMapManager

    interface IMiniMapManagerCommand with
        member this.InitMiniMap = MiniMapManagerCommand.initMiniMap this
        member this.SyncCameraIconPos = MiniMapManagerCommand.syncCameraIconPos this
        member this.ClickOnMiniMap = MiniMapManagerCommand.clickOnMiniMap this
        member this.RefreshMiniMapTile = MiniMapManagerCommand.refreshMiniMapTile this

    interface IHexMapGeneratorQuery with
        member this.HexMapGenerator = this.HexMapGenerator

    interface IHexMapGeneratorCommand with
        member this.GenerateMap = HexMapGeneratorCommand.generateMap this
        member this.ChangeLandGenerator = HexMapGeneratorCommand.changeLandGenerator this

    interface IPlanetHudQuery with
        member this.PlanetHudOpt = if this.PlanetHud = null then None else Some this.PlanetHud
        member this.GetEditMode = PlanetHudQuery.getEditMode this
        member this.GetLabelMode = PlanetHudQuery.getLabelMode this

    interface IPlanetHudCommand with
        member this.OnOrbitCameraRigMoved = PlanetHudCommand.onOrbitCameraRigMoved this

        member this.OnOrbitCameraRigTransformed =
            PlanetHudCommand.onOrbitCameraRigTransformed this

        member this.InitElevationAndWaterVSlider =
            PlanetHudCommand.initElevationAndWaterVSlider this

        member this.InitRectMiniMap = PlanetHudCommand.initRectMiniMap this
        member this.UpdateRadiusLineEdit = PlanetHudCommand.updateRadiusLineEdit this
        member this.UpdateDivisionLineEdit = PlanetHudCommand.updateDivisionLineEdit this
        member this.UpdateChosenTileInfo = PlanetHudCommand.updateChosenTileInfo this
        member this.UpdateNewPlanetInfo = PlanetHudCommand.updateNewPlanetInfo this
        member this.OnPlanetHudProcessed = PlanetHudCommand.onPlanetHudProcessed this
