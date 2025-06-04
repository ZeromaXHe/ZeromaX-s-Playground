namespace TO.FSharp.Services.Types.HexSphereServiceT

open Godot.Abstractions.Extensions.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 22:39:30
type InitHexSphere = IPlanet -> unit
type HexSphereServiceDep = { InitHexSphere: InitHexSphere }
