namespace TO.Presenters.Abstractions.Models.Planets

open Godot
open Godot.Abstractions.Bases
open TO.Domains.Structs.Tiles

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

type GetNoiseSourceImage = unit -> Image
type GetHashGridSize = unit -> int
type GetHashGrid = unit -> HexHash array
type GetRng = unit -> RandomNumberGenerator

[<Interface>]
type ICatlikeCodingNoiseQuery =
    abstract GetNoiseSourceImage: GetNoiseSourceImage
    abstract GetHashGridSize: GetHashGridSize
    abstract GetHashGrid: GetHashGrid
    abstract GetRng: GetRng

module CatlikeCodingNoiseQuery =
    let getNoiseSourceImage (this: ICatlikeCodingNoise) = fun () -> this.NoiseSourceImage
    let getHashGridSize (this: ICatlikeCodingNoise) = fun () -> this.HashGridSize
    let getHashGrid (this: ICatlikeCodingNoise) = fun () -> this.HashGrid
    let getRng (this: ICatlikeCodingNoise) = fun () -> this.Rng
