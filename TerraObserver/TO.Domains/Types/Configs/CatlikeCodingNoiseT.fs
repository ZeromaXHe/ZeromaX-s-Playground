namespace TO.Domains.Types.Configs

open Godot
open TO.Domains.Types.Godots
open TO.Domains.Types.Hashes
open TO.Domains.Types.HexSpheres.Components.Tiles

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

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 21:08:29
type SampleHashGrid = Vector3 -> HexHash
type SampleNoise = Vector3 -> Vector4
type Perturb = Vector3 -> Vector3
type GetHeight = TileValue -> TileUnitCentroid -> float32

[<Interface>]
type ICatlikeCodingNoiseQuery =
    abstract CatlikeCodingNoise: ICatlikeCodingNoise
    abstract SampleHashGrid: SampleHashGrid
    abstract SampleNoise: SampleNoise
    abstract Perturb: Perturb
    abstract GetHeight: GetHeight
