namespace TO.Domains.Types.Geos

open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 14:04:29
[<Interface>]
type ILonLatGrid =
    inherit INode3D
    // =====【Export】=====
    abstract LongitudeInterval: int with get, set
    abstract LatitudeInterval: int with get, set
    abstract Segments: int with get, set
    abstract LineMaterial: Material with get, set
    abstract NormalLineColor: Color with get, set
    abstract DeeperLineColor: Color with get, set
    abstract DeeperLineInterval: int with get, set
    abstract TropicColor: Color with get, set
    abstract CircleColor: Color with get, set
    abstract EquatorColor: Color with get, set
    abstract Degree90LongitudeColor: Color with get, set
    abstract MeridianColor: Color with get, set
    abstract DrawTropicOfCancer: bool with get, set
    abstract DrawTropicOfCapricorn: bool with get, set
    abstract DrawArcticCircle: bool with get, set
    abstract DrawAntarcticCircle: bool with get, set
    abstract FullVisibilityTime: float32 with get, set
    abstract FixFullVisibility: bool with get, set
    // =====【普通属性】=====
    abstract Visibility: float32 with get, set
    abstract FadeVisibility: bool with get, set
    abstract Radius: float32 with get, set
    abstract MeshIns: MeshInstance3D with get, set

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
