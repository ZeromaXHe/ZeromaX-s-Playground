namespace TO.FSharp.Apps.Planets

open Godot
open TO.Abstractions.Chunks
open TO.Abstractions.Planets
open TO.FSharp.Apps.Envs
open TO.FSharp.Services.Actions

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 19:41:30
type PlanetApp(viewEnv: PlanetViewEnv) =
    let dataEnv = PlanetDataEnv()

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()
        let planet = (viewEnv :> IPlanetEnv).Planet
        GD.Print $"[===DrawHexSphereMesh===] radius {planet.Radius}, divisions {planet.Divisions}, start at: {time}"
        HexSphereService.clearOldData dataEnv
        HexSphereService.initHexSphere viewEnv dataEnv
        HexGridChunkService.initChunkNodes viewEnv dataEnv

    member this.OnChunkLoaderProcessed() =
        HexGridChunkService.onChunkLoaderProcessed viewEnv dataEnv

    member this.OnHexGridChunkProcessed(chunk: IHexGridChunk) =
        HexGridChunkService.onHexGridChunkProcessed viewEnv dataEnv chunk

    member this.UpdateInsightChunks() =
        HexGridChunkService.updateInsightChunks viewEnv dataEnv
