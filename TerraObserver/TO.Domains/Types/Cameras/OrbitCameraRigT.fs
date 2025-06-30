namespace TO.Domains.Types.Cameras

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 21:44:29
type GetFocusBasePos = unit -> Vector3
type IsAutoPiloting = unit -> bool

[<Interface>]
type IOrbitCameraRigQuery =
    abstract OrbitCameraRig: IOrbitCameraRig
    abstract GetFocusBasePos: GetFocusBasePos
    abstract IsAutoPiloting: IsAutoPiloting

type OnZoomChanged = unit -> unit
type OnPlanetParamsChanged = unit -> unit
type Reset = unit -> unit
type SetAutoPilot = Vector3 -> unit
type CancelAutoPilot = unit -> unit
type RotateCamera = float32 -> bool
type OnProcessed = float32 -> unit

[<Interface>]
type IOrbitCameraRigCommand =
    abstract OnZoomChanged: OnZoomChanged
    abstract OnPlanetParamsChanged: OnPlanetParamsChanged
    abstract Reset: Reset
    abstract SetAutoPilot: SetAutoPilot
    abstract CancelAutoPilot: CancelAutoPilot
    abstract RotateCamera: RotateCamera
    abstract OnProcessed: OnProcessed
