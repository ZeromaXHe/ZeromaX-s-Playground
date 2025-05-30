namespace TO.FSharp.GodotAbstractions.Extensions.Planets

open TO.FSharp.GodotAbstractions.Bases

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-28 23:04:28
[<Interface>]
type IPlanet =
    inherit INode3D
    abstract Radius: float32 with get
    abstract Divisions: int with get
    abstract ChunkDivisions: int with get
    abstract MaxHeight: float32 with get
    abstract StandardScale: float32 with get
