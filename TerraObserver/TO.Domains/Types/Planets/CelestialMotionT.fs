namespace TO.Domains.Types.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 22:44:29
[<Interface>]
type ICelestialMotionQuery =
    abstract CelestialMotion: ICelestialMotion

type ToggleAllMotions = bool -> unit
type UpdateLunarDist = unit -> unit
type UpdateMoonMeshRadius = unit -> unit

[<Interface>]
type ICelestialMotionCommand =
    abstract ToggleAllMotions: ToggleAllMotions
    abstract UpdateLunarDist: UpdateLunarDist
    abstract UpdateMoonMeshRadius: UpdateMoonMeshRadius
