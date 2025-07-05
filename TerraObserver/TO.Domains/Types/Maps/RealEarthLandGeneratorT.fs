namespace TO.Domains.Types.Maps

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-05 11:39:05
[<Interface>]
type IRealEarthLandGenerator =
    inherit ILandGenerator
    abstract WorldMap: Texture2D
