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
open TO.Domains.Functions.PathFindings
open TO.Domains.Functions.PlanetHuds
open TO.Domains.Functions.Planets
open TO.Domains.Functions.Shaders
open TO.Domains.Types.Cameras
open TO.Domains.Types.Chunks
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.Geos
open TO.Domains.Types.HexMeshes
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.PathFindings
open TO.Domains.Types.PlanetHuds
open TO.Domains.Types.Planets
open TO.Domains.Types.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:33:19
type PlanetEnv
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
        chunkLoader,
        planetHud
    ) =
    member this.GetEntityById = EntityStoreQuery.getEntityById this

    member this.Query<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> IComponent and 'T :> System.ValueType>() =
        EntityStoreQuery.query<'T, PlanetEnv> this ()

    interface IEntityStoreQuery with
        member this.EntityStore = store
        member this.GetEntityById = this.GetEntityById

        member this.Query<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> IComponent and 'T :> System.ValueType>
            ()
            =
            this.Query<'T>()

    member this.ExecuteInCommandBuffer = EntityStoreCommand.executeInCommandBuffer this

    interface IEntityStoreCommand with
        member this.ExecuteInCommandBuffer = this.ExecuteInCommandBuffer

    member this.TryHeadByPosition = PointQuery.tryHeadByPosition this
    member this.TryHeadEntityByCenterId = PointQuery.tryHeadEntityByCenterId this
    member this.GetNeighborByIdAndIdx = PointQuery.getNeighborByIdAndIdx this
    member this.GetNeighborIdx = PointQuery.getNeighborIdx this
    member this.GetNeighborIdsById = PointQuery.getNeighborIdsById this
    member this.GetNeighborCenterPointIds = PointQuery.getNeighborCenterPointIds this
    member this.SearchNearestCenterPos = PointQuery.searchNearestCenterPos this

    interface IPointQuery with
        member this.TryHeadByPosition = this.TryHeadByPosition
        member this.TryHeadEntityByCenterId = this.TryHeadEntityByCenterId
        member this.GetNeighborByIdAndIdx = this.GetNeighborByIdAndIdx
        member this.GetNeighborIdx = this.GetNeighborIdx
        member this.GetNeighborIdsById = this.GetNeighborIdsById
        member this.GetNeighborCenterPointIds = this.GetNeighborCenterPointIds
        member this.SearchNearestCenterPos = this.SearchNearestCenterPos

    member this.AddPoint = PointCommand.add this
    member this.CreateVpTree = PointCommand.createVpTree this

    interface IPointCommand with
        member this.AddPoint = this.AddPoint
        member this.CreateVpTree = this.CreateVpTree

    member this.GetOrderedFaces = FaceQuery.getOrderedFaces this

    interface IFaceQuery with
        member this.GetOrderedFaces = this.GetOrderedFaces

    member this.AddFace = FaceCommand.add this

    interface IFaceCommand with
        member this.AddFace = this.AddFace

    member this.AddTile = TileCommand.add this

    interface ITileCommand with
        member this.AddTile = this.AddTile

    member this.IsHandlingLodGaps = ChunkQuery.isHandlingLodGaps this
    member this.GetLod = ChunkQuery.getLod this

    interface IChunkQuery with
        member this.IsHandlingLodGaps = this.IsHandlingLodGaps
        member this.GetLod = this.GetLod

    member this.AddChunk = ChunkCommand.add this
    member this.UpdateChunkInsightAndLod = ChunkCommand.updateChunkInsightAndLod this

    interface IChunkCommand with
        member this.AddChunk = this.AddChunk
        member this.UpdateChunkInsightAndLod = this.UpdateChunkInsightAndLod

    member this.GetHexFacesAndNeighborCenterIds =
        HexSphereQuery.getHexFacesAndNeighborCenterIds this

    member this.SearchNearest = HexSphereQuery.searchNearest this

    interface IHexSphereQuery with
        member this.GetHexFacesAndNeighborCenterIds = this.GetHexFacesAndNeighborCenterIds
        member this.SearchNearest = this.SearchNearest

    member this.InitChunks = HexSphereInitCommand.initChunks this
    member this.InitTiles = HexSphereInitCommand.initTiles this
    member this.ClearHexSphereOldData = HexSphereInitCommand.clearOldData this
    member this.InitHexSphere = HexSphereInitCommand.initHexSphere this

    interface IHexSphereInitCommand with
        member this.InitChunks = this.InitChunks
        member this.InitTiles = this.InitTiles
        member this.ClearHexSphereOldData = this.ClearHexSphereOldData
        member this.InitHexSphere = this.InitHexSphere

    member this.ChooseVpTreeByChunky = ChunkyVpTreesQuery.chooseByChunky this

    interface IChunkyVpTreesQuery with
        member this.ChunkyVpTrees = chunkyVpTrees
        member this.ChooseVpTreeByChunky = this.ChooseVpTreeByChunky

    member this.GetLodMeshes = LodMeshCacheQuery.getLodMeshes this

    interface ILodMeshCacheQuery with
        member this.LodMeshCache = lodMeshCache
        member this.GetLodMeshes = this.GetLodMeshes

    member this.AddLodMeshes = LodMeshCacheCommand.addLodMeshes this
    member this.RemoveAllLodMeshes = LodMeshCacheCommand.removeAllLodMeshes this
    member this.RemoveLodMeshes = LodMeshCacheCommand.removeLodMeshes this

    interface ILodMeshCacheCommand with
        member this.AddLodMeshes = this.AddLodMeshes
        member this.RemoveAllLodMeshes = this.RemoveAllLodMeshes
        member this.RemoveLodMeshes = this.RemoveLodMeshes

    interface ITileSearcherQuery with
        member this.TileSearcher = tileSearcher

    member this.InitSearchData = TileSearcherCommand.initSearchData this
    member this.RefreshTileSearchData = TileSearcherCommand.refreshTileSearchData this

    interface ITileSearcherCommand with
        member this.InitSearchData = this.InitSearchData
        member this.RefreshTileSearchData = this.RefreshTileSearchData

    interface ITileShaderDataQuery with
        member this.TileShaderData = tileShaderData

    member this.InitShaderData = TileShaderDataCommand.initShaderData this
    member this.RefreshTileShaderDataCiv = TileShaderDataCommand.refreshCiv this
    member this.RefreshTileShaderDataTerrain = TileShaderDataCommand.refreshTerrain this

    member this.RefreshTileShaderDataVisibility =
        TileShaderDataCommand.refreshVisibility this

    member this.UpdateTileShaderData = TileShaderDataCommand.updateData this
    member this.ViewElevationChanged = TileShaderDataCommand.viewElevationChanged this

    interface ITileShaderDataCommand with
        member this.InitShaderData = this.InitShaderData
        member this.RefreshTileShaderDataCiv = this.RefreshTileShaderDataCiv
        member this.RefreshTileShaderDataTerrain = this.RefreshTileShaderDataTerrain
        member this.RefreshTileShaderDataVisibility = this.RefreshTileShaderDataVisibility
        member this.UpdateData = this.UpdateTileShaderData
        member this.ViewElevationChanged = this.ViewElevationChanged

    member this.GetTileLen = PlanetConfigQuery.getTileLen this

    interface IPlanetConfigQuery with
        member this.PlanetConfig = planetConfig
        member this.GetTileLen = this.GetTileLen

    member this.SampleHashGrid = CatlikeCodingNoiseQuery.sampleHashGrid this
    member this.SampleNoise = CatlikeCodingNoiseQuery.sampleNoise this
    member this.Perturb = CatlikeCodingNoiseQuery.perturb this
    member this.GetHeight = CatlikeCodingNoiseQuery.getHeight this

    interface ICatlikeCodingNoiseQuery with
        member this.CatlikeCodingNoise = catlikeCodingNoise
        member this.SampleHashGrid = this.SampleHashGrid
        member this.SampleNoise = this.SampleNoise
        member this.Perturb = this.Perturb
        member this.GetHeight = this.GetHeight

    member this.GetOrbitCameraRigFocusBasePos = OrbitCameraRigQuery.getFocusBasePos this
    member this.IsAutoPiloting = OrbitCameraRigQuery.isAutoPiloting this

    interface IOrbitCameraRigQuery with
        member this.OrbitCameraRig = cameraRig
        member this.GetFocusBasePos = this.GetOrbitCameraRigFocusBasePos
        member this.IsAutoPiloting = this.IsAutoPiloting

    member this.OrbitCameraRigOnZoomChanged = OrbitCameraRigCommand.onZoomChanged this

    member this.OrbitCameraRigOnPlanetParamsChanged =
        OrbitCameraRigCommand.onPlanetParamsChanged this

    member this.ResetOrbitCamera = OrbitCameraRigCommand.reset this
    member this.SetAutoPilot = OrbitCameraRigCommand.setAutoPilot this
    member this.CancelAutoPilot = OrbitCameraRigCommand.cancelAutoPilot this
    member this.RotateCamera = OrbitCameraRigCommand.rotateCamera this
    member this.OrbitCameraRigOnProcessed = OrbitCameraRigCommand.onProcessed this

    interface IOrbitCameraRigCommand with
        member this.OnZoomChanged = this.OrbitCameraRigOnZoomChanged
        member this.OnPlanetParamsChanged = this.OrbitCameraRigOnPlanetParamsChanged
        member this.Reset = this.ResetOrbitCamera
        member this.SetAutoPilot = this.SetAutoPilot
        member this.CancelAutoPilot = this.CancelAutoPilot
        member this.RotateCamera = this.RotateCamera
        member this.OnProcessed = this.OrbitCameraRigOnProcessed

    interface ILonLatGridQuery with
        member this.LonLatGrid = lonLatGrid

    member this.DoDrawLonLatGrid = LonLatGridCommand.doDraw this
    member this.DrawLonLatGridOnPlanet = LonLatGridCommand.drawOnPlanet this
    member this.ToggleFixFullVisibility = LonLatGridCommand.toggleFixFullVisibility this
    member this.LonLatGridOnCameraMoved = LonLatGridCommand.onCameraMoved this

    interface ILonLatGridCommand with
        member this.DoDraw = this.DoDrawLonLatGrid
        member this.DrawOnPlanet = this.DrawLonLatGridOnPlanet
        member this.ToggleFixFullVisibility = this.ToggleFixFullVisibility
        member this.OnCameraMoved = this.LonLatGridOnCameraMoved

    interface ICelestialMotionQuery with
        member this.CelestialMotion = celestialMotion

    member this.ToggleAllMotions = CelestialMotionCommand.toggleAllMotions this
    member this.UpdateLunarDist = CelestialMotionCommand.updateLunarDist this
    member this.UpdateMoonMeshRadius = CelestialMotionCommand.updateMoonMeshRadius this

    interface ICelestialMotionCommand with
        member this.ToggleAllMotions = this.ToggleAllMotions
        member this.UpdateLunarDist = this.UpdateLunarDist
        member this.UpdateMoonMeshRadius = this.UpdateMoonMeshRadius

    member this.TryGetUsingChunk = ChunkLoaderQuery.tryGetUsingChunk this
    member this.GetAllUsingChunks = ChunkLoaderQuery.getAllUsingChunks this
    member this.GetViewportCamera = ChunkLoaderQuery.getViewportCamera this

    interface IChunkLoaderQuery with
        member this.ChunkLoader = chunkLoader
        member this.TryGetUsingChunk = this.TryGetUsingChunk
        member this.GetAllUsingChunks = this.GetAllUsingChunks
        member this.GetViewportCamera = this.GetViewportCamera

    member this.Triangulate = ChunkTriangulationCommand.triangulate this

    interface IChunkTriangulationCommand with
        member this.Triangulate = this.Triangulate

    member this.ClearChunkLoaderOldData = ChunkLoaderCommand.clearOldData this
    member this.AddUsingChunk = ChunkLoaderCommand.addUsingChunk this
    member this.OnChunkLoaderProcessed = ChunkLoaderCommand.onChunkLoaderProcessed this
    member this.InitChunkNodes = ChunkLoaderCommand.initChunkNodes this
    member this.OnHexGridChunkProcessed = ChunkLoaderCommand.onHexGridChunkProcessed this
    member this.UpdateInsightChunks = ChunkLoaderCommand.updateInsightChunks this

    interface IChunkLoaderCommand with
        member this.ClearChunkLoaderOldData = this.ClearChunkLoaderOldData
        member this.AddUsingChunk = this.AddUsingChunk
        member this.OnChunkLoaderProcessed = this.OnChunkLoaderProcessed
        member this.InitChunkNodes = this.InitChunkNodes
        member this.OnHexGridChunkProcessed = this.OnHexGridChunkProcessed
        member this.UpdateInsightChunks = this.UpdateInsightChunks

    interface IPlanetHudQuery with
        member this.PlanetHudOpt = if planetHud = null then None else Some planetHud

    member this.PlanetHudOnOrbitCameraRigMoved =
        PlanetHudCommand.onOrbitCameraRigMoved this

    member this.PlanetHudOnOrbitCameraRigTransformed =
        PlanetHudCommand.onOrbitCameraRigTransformed this

    member this.InitElevationAndWaterVSlider =
        PlanetHudCommand.initElevationAndWaterVSlider this

    member this.UpdateRadiusLineEdit = PlanetHudCommand.updateRadiusLineEdit this
    member this.UpdateDivisionLineEdit = PlanetHudCommand.updateDivisionLineEdit this

    interface IPlanetHudCommand with
        member this.OnOrbitCameraRigMoved = this.PlanetHudOnOrbitCameraRigMoved
        member this.OnOrbitCameraRigTransformed = this.PlanetHudOnOrbitCameraRigTransformed
        member this.InitElevationAndWaterVSlider = this.InitElevationAndWaterVSlider
        member this.UpdateRadiusLineEdit = this.UpdateRadiusLineEdit
        member this.UpdateDivisionLineEdit = this.UpdateDivisionLineEdit
