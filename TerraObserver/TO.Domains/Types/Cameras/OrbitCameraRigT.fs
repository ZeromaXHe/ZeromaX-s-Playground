namespace TO.Domains.Types.Cameras

open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 13:22:29
[<Interface>]
type IOrbitCameraRig =
    inherit INode3D
    // =====【事件】=====
    abstract EmitMoved: Vector3 -> float32 -> unit
    abstract EmitTransformed: Transform3D -> float32 -> unit
    // =====【Export】=====
    abstract StickMinZoom: float32
    abstract StickMaxZoom: float32
    abstract SwivelMinZoom: float32
    abstract SwivelMaxZoom: float32
    abstract MoveSpeedMinZoom: float32
    abstract MoveSpeedMaxZoom: float32
    abstract RotationSpeed: float32
    abstract Sun: Node3D
    abstract AutoPilotSpeed: float32
    // =====【on-ready】=====
    abstract FocusBase: Node3D
    abstract FocusBox: CsgBox3D
    abstract FocusBackStick: Node3D
    abstract BackBox: CsgBox3D
    abstract Light: SpotLight3D
    abstract Swivel: Node3D
    abstract Stick: Node3D
    abstract CamRig: RemoteTransform3D
    // =====【普通属性】=====
    abstract Zoom: float32 with get, set
    abstract AntiStuckSpeedMultiplier: float32 with get, set
    abstract AutoPilotProgress: float32 with get, set
    abstract FromDirection: Vector3 with get, set
    abstract DestinationDirection: Vector3 with get, set

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
