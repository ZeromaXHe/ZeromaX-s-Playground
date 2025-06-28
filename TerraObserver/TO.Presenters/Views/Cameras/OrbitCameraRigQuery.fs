namespace TO.Presenters.Views.Cameras

open Godot
open TO.Abstractions.Views.Cameras

type GetFocusBasePos = unit -> Vector3
type IsAutoPiloting = unit -> bool

[<Interface>]
type IOrbitCameraRigQuery =
    abstract GetFocusBasePos: GetFocusBasePos
    abstract IsAutoPiloting: IsAutoPiloting

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 21:24:24
module OrbitCameraRigQuery =
    let getFocusBasePos (camRig: IOrbitCameraRig): GetFocusBasePos =
        fun () -> camRig.FocusBase.GlobalPosition

    let isAutoPiloting (camRig: IOrbitCameraRig): IsAutoPiloting =
        fun () -> camRig.DestinationDirection <> Vector3.Zero
