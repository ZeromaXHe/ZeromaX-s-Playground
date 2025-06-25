namespace TO.Controllers.Apps.Envs

open TO.Presenters.Views.Cameras
open TO.Presenters.Views.Chunks
open TO.Presenters.Views.Geos
open TO.Presenters.Views.Planets
open TO.Presenters.Views.Uis
open TO.Presenters.Models.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 19:06:19
type PlanetPreEnv(planet, catlikeCodingNoise, cameraRig, lonLatGrid, celestialMotion, chunkLoader, planetHud) =
    interface IPlanetQuery with
        member this.GetRadius = PlanetQuery.getRadius planet
        member this.GetDivisions = PlanetQuery.getDivisions planet
        member this.GetChunkDivisions = PlanetQuery.getChunkDivisions planet
        member this.GetUnitHeight = PlanetQuery.getUnitHeight planet
        member this.GetMaxHeight = PlanetQuery.getMaxHeight planet
        member this.GetStandardScale = PlanetQuery.getStandardScale planet
        member this.GetTileLen = PlanetQuery.getTileLen planet

    interface ICatlikeCodingNoiseQuery with
        member this.GetHeight = CatlikeCodingNoiseQuery.getHeight planet catlikeCodingNoise
        member this.Perturb = CatlikeCodingNoiseQuery.perturb planet catlikeCodingNoise

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
