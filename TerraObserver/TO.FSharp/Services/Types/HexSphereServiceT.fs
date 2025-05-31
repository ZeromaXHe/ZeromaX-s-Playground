namespace TO.FSharp.Services.Types.HexSphereServiceT

open TO.FSharp.GodotAbstractions.Extensions.Planets
open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 22:39:30
type InitHexSphere = IPlanet -> unit

type HexSphereServiceDep =
    { InitHexSphere: InitHexSphere }
