namespace TO.FSharp.Apps.Envs

open TO.Abstractions.Chunks
open TO.Abstractions.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 19:06:19
type PlanetViewEnv(planet: IPlanet, catlikeCodingNoise: ICatlikeCodingNoise, chunkLoader: IChunkLoader) =
    interface IPlanetEnv with
        override _.Planet = planet

    interface ICatlikeCodingNoiseEnv with
        override _.CatlikeCodingNoise = catlikeCodingNoise

    interface IChunkLoaderEnv with
        override _.ChunkLoader = chunkLoader
