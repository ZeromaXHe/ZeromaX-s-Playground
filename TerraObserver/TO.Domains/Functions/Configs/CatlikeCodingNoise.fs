namespace TO.Domains.Functions.Configs

open Godot
open TO.Domains.Functions.Hashes
open TO.Domains.Functions.HexMetrics
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Functions.Maths
open TO.Domains.Functions.Textures
open TO.Domains.Types.Configs
open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 21:08:29
module CatlikeCodingNoiseQuery =
    let sampleHashGrid (env: #ICatlikeCodingNoiseQuery) : SampleHashGrid =
        fun (position: Vector3) ->
            let noise = env.CatlikeCodingNoise
            let position = Math3dUtil.ProjectToSphere(position, HexMetrics.StandardRadius)

            let mutable x =
                int
                <| Mathf.PosMod(position.X - position.Y * 0.5f - position.Z * 0.5f, float32 noise.HashGridSize)

            if x = noise.HashGridSize then // 前面噪声扰动那里说过 PosMod 文档返回 [0, b), 结果取到了 b，所以怕了…… 加个防御性处理
                x <- 0

            let mutable z =
                int
                <| Mathf.PosMod((position.Y - position.Z) * HexMetrics.OuterToInner, float32 noise.HashGridSize)

            if z = noise.HashGridSize then
                z <- 0

            noise.HashGrid[x + z * noise.HashGridSize]

    let noiseScale = 0.003f

    let sampleNoise (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ICatlikeCodingNoiseQuery) : SampleNoise =
        fun (position: Vector3) ->
            let planetConfig = env.PlanetConfig
            let catlikeCodingNoise = env.CatlikeCodingNoise

            TextureUtil.GetPixelBilinear
            <| catlikeCodingNoise.NoiseSourceImage
            <| (position.X - position.Y * 0.5f - position.Z * 0.5f) * noiseScale
               / planetConfig.StandardScale
            <| (position.Y - position.Z) * HexMetrics.OuterToInner * noiseScale
               / planetConfig.StandardScale

    let cellPerturbStrength = 4f

    let perturb (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ICatlikeCodingNoiseQuery) : Perturb =
        fun (position: Vector3) ->
            let planetConfig = env.PlanetConfig

            let sample =
                env.SampleNoise
                <| Math3dUtil.ProjectToSphere(position, HexMetrics.StandardRadius)

            let vecX =
                if position.X = 0f && position.Z = 0f then
                    position.Cross(Vector3.Back).Normalized()
                else
                    Vector3.Up.Cross(position).Normalized()

            let vecZ = vecX.Cross(position).Normalized()

            let x =
                vecX
                * (sample.X * 2f - 1f)
                * cellPerturbStrength
                * planetConfig.StandardScale
                * position.Length()
                / HexMetrics.StandardRadius

            let z =
                vecZ
                * (sample.Z * 2f - 1f)
                * cellPerturbStrength
                * planetConfig.StandardScale
                * position.Length()
                / HexMetrics.StandardRadius

            position + x + z

    let elevationPerturbStrength = 0.5f

    let getHeight (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ICatlikeCodingNoiseQuery) : GetHeight =
        let planetConfig = env.PlanetConfig

        let getPerturbHeight (centroid: Vector3) =
            ((env.SampleNoise centroid).Y * 2f - 1f) * elevationPerturbStrength

        let getHeightInner (elevation: int) (centroid: Vector3) =
            (float32 elevation + getPerturbHeight centroid) * planetConfig.UnitHeight

        fun (tileValue: TileValue) (tileUnitCentroid: TileUnitCentroid) ->
            let centroid = tileUnitCentroid |> TileUnitCentroid.scaled HexMetrics.StandardRadius
            let elevation = tileValue |> TileValue.elevation
            getHeightInner elevation centroid

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 20:38:24
module CatlikeCodingNoiseCommand =
    let initializeHashGrid (env: #ICatlikeCodingNoiseQuery) =
        fun (seed: uint64) ->
            let noise = env.CatlikeCodingNoise
            let rng = noise.Rng
            let initState = rng.State
            rng.Seed <- seed

            for i in 0 .. noise.HashGrid.Length - 1 do
                noise.HashGrid[i] <- HexHash.create ()

            rng.State <- initState
