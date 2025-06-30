namespace TO.Domains.Types.PlanetHuds

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 23:23:29
[<Interface>]
type IPlanetHudQuery =
    abstract PlanetHudOpt: IPlanetHud option // 因为 PlanetHud 不是 Tool，所以编辑器内可能为空

type OnOrbitCameraRigMoved = Vector3 -> float32 -> unit
type OnOrbitCameraRigTransformed = Transform3D -> unit
type InitElevationAndWaterVSlider = unit -> unit
type UpdateRadiusLineEdit = string -> unit
type UpdateDivisionLineEdit = bool -> string -> unit

[<Interface>]
type IPlanetHudCommand =
    abstract OnOrbitCameraRigMoved: OnOrbitCameraRigMoved
    abstract OnOrbitCameraRigTransformed: OnOrbitCameraRigTransformed
    abstract InitElevationAndWaterVSlider: InitElevationAndWaterVSlider
    abstract UpdateRadiusLineEdit: UpdateRadiusLineEdit
    abstract UpdateDivisionLineEdit: UpdateDivisionLineEdit
