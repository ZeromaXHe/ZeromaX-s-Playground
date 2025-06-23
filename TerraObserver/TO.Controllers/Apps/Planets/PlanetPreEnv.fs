namespace TO.Controllers.Apps.Envs

open TO.Abstractions.Models.Planets
open TO.Abstractions.Views.Cameras
open TO.Abstractions.Views.Geos
open TO.Abstractions.Views.Planets
open TO.Abstractions.Views.Uis
open TO.Presenters.Commands.Cameras
open TO.Presenters.Commands.Geos
open TO.Presenters.Commands.Planets
open TO.Presenters.Commands.Uis
open TO.Presenters.Queries.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 19:06:19
type PlanetPreEnv
    (
        planet: IPlanet,
        catlikeCodingNoise: ICatlikeCodingNoise,
        cameraRig: IOrbitCameraRig,
        lonLatGrid: ILonLatGrid,
        celestialMotion: ICelestialMotion,
        planetHud: IPlanetHud
    ) =
    interface ICatlikeCodingNoiseQuery with
        member this.GetHeight = CatlikeCodingNoiseQuery.getHeight planet catlikeCodingNoise
        member this.Perturb = CatlikeCodingNoiseQuery.perturb planet catlikeCodingNoise

    interface IOrbitCameraRigCommand with
        member this.OnPlanetParamsChanged =
            OrbitCameraRigCommand.onPlanetParamsChanged planet cameraRig

        member this.OnProcessed = OrbitCameraRigCommand.onProcessed planet cameraRig
        member this.OnZoomChanged = OrbitCameraRigCommand.onZoomChanged planet cameraRig
        member this.Reset = OrbitCameraRigCommand.reset planet cameraRig

    interface ILonLatGridCommand with
        member this.DrawOnPlanet = LonLatGridCommand.drawOnPlanet planet lonLatGrid

    interface ICelestialMotionCommand with
        member this.UpdateLunarDist =
            CelestialMotionCommand.updateLunarDist planet celestialMotion

        member this.UpdateMoonMeshRadius =
            CelestialMotionCommand.updateMoonMeshRadius planet celestialMotion

    interface IPlanetHudCommand with
        member this.OnOrbitCameraRigTransformed =
            PlanetHudCommand.onOrbitCameraRigMoved cameraRig planetHud

        member this.InitElevationAndWaterVSlider =
            PlanetHudCommand.initElevationAndWaterVSlider planet planetHud

        member this.UpdateRadiusLineEdit =
            PlanetHudCommand.updateRadiusLineEdit planet planetHud

        member this.UpdateDivisionLineEdit =
            PlanetHudCommand.updateDivisionLineEdit planet planetHud
