namespace TO.Presenters.Models.Planets

open TO.Abstractions.Models.Planets
open TO.Domains.Structs.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 20:38:24
module CatlikeCodingNoiseCommand =
    let initializeHashGrid (noise: ICatlikeCodingNoise) =
        fun (seed: uint64) ->
            let rng = noise.Rng
            let initState = rng.State
            rng.Seed <- seed

            for i in 0 .. noise.HashGrid.Length - 1 do
                noise.HashGrid[i] <- HexHash.Create()

            rng.State <- initState
