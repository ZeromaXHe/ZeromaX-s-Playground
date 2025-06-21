namespace TO.Controllers.Apps.Envs

open TO.Abstractions.Cameras
open TO.Abstractions.Planets
open TO.Presenters.Commands.Cameras
open TO.Presenters.Queries.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 19:06:19
type PlanetPreEnv(planet: IPlanet, catlikeCodingNoise: ICatlikeCodingNoise, cameraRig: IOrbitCameraRig) =
    interface ICatlikeCodingNoiseQuery with
        member this.GetHeight = CatlikeCodingNoiseQuery.getHeight planet catlikeCodingNoise
        member this.Perturb = CatlikeCodingNoiseQuery.perturb planet catlikeCodingNoise

    interface IOrbitCameraRigCommand with
        member this.OnPlanetParamsChanged =
            OrbitCameraRigCommand.onPlanetParamsChanged planet cameraRig

        member this.OnProcessed = OrbitCameraRigCommand.onProcessed planet cameraRig
        member this.OnZoomChanged = OrbitCameraRigCommand.onZoomChanged planet cameraRig
        member this.Reset = OrbitCameraRigCommand.reset planet cameraRig
