namespace TO.Domains.Types.Planets

open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 14:10:29
[<Interface>]
type ICelestialMotion =
    inherit INode3D
    // =====【Export】=====
    abstract Moon: Node3D
    abstract PlanetRevolution: bool with get, set
    abstract PlanetRotation: bool with get, set
    abstract SatelliteRevolution: bool with get, set
    abstract SatelliteRotation: bool with get, set
    abstract SatelliteRadiusRatio: float32
    abstract SatelliteDistRatio: float32
    // =====【on-ready】=====
    abstract LunarDist: Node3D
