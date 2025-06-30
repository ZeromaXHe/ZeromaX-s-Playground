namespace TO.Domains.Types.Geos

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 22:22:29
[<Interface>]
type ILonLatGridQuery =
    abstract LonLatGrid: ILonLatGrid

type DoDraw = unit -> unit
type DrawOnPlanet = unit -> unit
type ToggleFixFullVisibility = bool -> unit
type OnCameraMoved = Vector3 -> float32 -> unit

[<Interface>]
type ILonLatGridCommand =
    abstract DoDraw: DoDraw
    abstract DrawOnPlanet: DrawOnPlanet
    abstract ToggleFixFullVisibility: ToggleFixFullVisibility
    abstract OnCameraMoved: OnCameraMoved
