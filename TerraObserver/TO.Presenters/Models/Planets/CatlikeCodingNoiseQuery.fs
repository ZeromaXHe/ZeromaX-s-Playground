namespace TO.Presenters.Models.Planets

open Godot
open TO.Abstractions.Models.Planets
open TO.Domains.Components.HexSpheres.Tiles
open TO.Domains.Structs.Tiles
open TO.Domains.Utils.Commons
open TO.Domains.Utils.HexSpheres

type SampleHashGrid = Vector3 -> HexHash
type SampleNoise = Vector3 -> Vector4
type Perturb = Vector3 -> Vector3
type GetHeight = TileValue -> TileUnitCentroid -> float32

[<Interface>]
type ICatlikeCodingNoiseQuery =
    abstract SampleHashGrid: SampleHashGrid
    abstract SampleNoise: SampleNoise
    abstract Perturb: Perturb
    abstract GetHeight: GetHeight

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-21 20:39:21
module CatlikeCodingNoiseQuery =
    let sampleHashGrid (noise: ICatlikeCodingNoise) : SampleHashGrid =
        fun (position: Vector3) ->
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

    let sampleNoise (planet: IPlanet) (catlikeCodingNoise: ICatlikeCodingNoise) : SampleNoise =
        fun (position: Vector3) ->
            TextureUtil.GetPixelBilinear
            <| catlikeCodingNoise.NoiseSourceImage
            <| (position.X - position.Y * 0.5f - position.Z * 0.5f) * noiseScale
               / planet.StandardScale
            <| (position.Y - position.Z) * HexMetrics.OuterToInner * noiseScale
               / planet.StandardScale

    let cellPerturbStrength = 4f

    let perturb (env: 'E when 'E :> IPlanetQuery and 'E :> ICatlikeCodingNoiseQuery) : Perturb =
        fun (position: Vector3) ->
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
                * env.GetStandardScale()
                * position.Length()
                / HexMetrics.StandardRadius

            let z =
                vecZ
                * (sample.Z * 2f - 1f)
                * cellPerturbStrength
                * env.GetStandardScale()
                * position.Length()
                / HexMetrics.StandardRadius

            position + x + z

    let elevationPerturbStrength = 0.5f

    let getHeight (env: 'E when 'E :> IPlanetQuery and 'E :> ICatlikeCodingNoiseQuery) : GetHeight =
        let getPerturbHeight (centroid: Vector3) =
            ((env.SampleNoise centroid).Y * 2f - 1f) * elevationPerturbStrength

        let getHeightInner (elevation: int) (centroid: Vector3) =
            (float32 elevation + getPerturbHeight centroid) * env.GetUnitHeight()

        fun (tileValue: TileValue) (tileUnitCentroid: TileUnitCentroid) ->
            let centroid = tileUnitCentroid.Scaled HexMetrics.StandardRadius
            getHeightInner tileValue.Elevation centroid
