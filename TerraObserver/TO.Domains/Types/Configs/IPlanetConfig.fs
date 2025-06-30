namespace TO.Domains.Types.Configs

open Godot.Abstractions.Bases

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 13:06:29
[<Interface>]
type IPlanetConfig =
    inherit IResource
    abstract Radius: float32 with get, set
    abstract Divisions: int with get, set
    abstract ChunkDivisions: int with get, set
    abstract UnitHeight: float32
    abstract MaxHeight: float32
    abstract MaxHeightRatio: float32
    abstract StandardScale: float32
    abstract ElevationStep: int
