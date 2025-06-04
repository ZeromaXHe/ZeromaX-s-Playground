namespace TO.FSharp.Apps.Cameras

open Godot.Abstractions.Extensions.Cameras
open Godot.Abstractions.Extensions.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-04 10:17:04
type OrbitCameraApp() =
    member this.OnRadiusChanged(orbitCamera: IOrbitCameraRig, planet: IPlanet, radius: float32) =
        orbitCamera.SetRadius(radius, planet.MaxHeightRatio, planet.StandardScale)

    member this.OnZoomChanged(orbitCamera: IOrbitCameraRig, planet: IPlanet, zoom: float32) =
        orbitCamera.SetZoom(zoom, planet.StandardScale)
