namespace TO.Domains.Types.Configs

open Friflo.Engine.ECS
open Godot
open TO.Domains.Types.Godots
open TO.Domains.Types.Hashes

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-27 17:55:27
[<Interface>]
type ICatlikeCodingNoise =
    inherit IResource
    // =====【Export】=====
    abstract Seed: uint64
    // =====【普通属性】=====
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
type GetPerturbHeight = Entity -> float32
type GetHeight = Entity -> float32

[<Interface>]
type ICatlikeCodingNoiseQuery =
    abstract CatlikeCodingNoise: ICatlikeCodingNoise
    abstract SampleHashGrid: SampleHashGrid
    abstract SampleNoise: SampleNoise
    abstract Perturb: Perturb
    abstract GetPerturbHeight: GetPerturbHeight
    abstract GetHeight: GetHeight

type InitializeHashGrid = unit -> unit

[<Interface>]
type ICatlikeCodingNoiseCommand =
    abstract InitializeHashGrid: InitializeHashGrid
