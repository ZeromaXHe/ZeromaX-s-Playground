namespace TO.Domains.Types.Configs

open Godot
open Godot.Abstractions.Bases
open TO.Domains.Types.Hashes

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-27 17:55:27
[<Interface>]
type ICatlikeCodingNoise =
    inherit IResource
    abstract NoiseSourceImage: Image
    abstract HashGridSize: int
    abstract HashGrid: HexHash array
    abstract Rng: RandomNumberGenerator
