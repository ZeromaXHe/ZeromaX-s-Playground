namespace TO.FSharp.Apps.Planets

open Godot
open Godot.Abstractions.Extensions.Chunks
open Godot.Abstractions.Extensions.Planets
open TO.FSharp.Services.Actions

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 19:41:30
type PlanetApp(planet: IPlanet, catlikeCodingNoise: ICatlikeCodingNoise, chunkLoader: IChunkLoader) =
    let env = PlanetEnv()

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()
        GD.Print $"[===DrawHexSphereMesh===] radius {planet.Radius}, divisions {planet.Divisions}, start at: {time}"
        HexSphereService.clearOldData env
        HexSphereService.initHexSphere planet env
        HexGridChunkService.initChunkNodes chunkLoader env

    member this.OnChunkLoaderProcessed() =
        HexGridChunkService.onChunkLoaderProcessed chunkLoader env

    member this.OnHexGridChunkProcessed(chunk: IHexGridChunk) =
        HexGridChunkService.onHexGridChunkProcessed planet catlikeCodingNoise env chunk

    member this.UpdateInsightChunks() =
        HexGridChunkService.updateInsightChunks chunkLoader env
