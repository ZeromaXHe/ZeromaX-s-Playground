namespace TO.Presenters.Queries.Planets

open Godot
open TO.Abstractions.Models.Planets
open TO.Domains.Components.HexSpheres.Tiles
open TO.Domains.Utils.Commons
open TO.Domains.Utils.HexSpheres

type CatlikeCodingNoisePerturb = Vector3 -> Vector3
type CatlikeCodingNoiseGetHeight = TileValue -> TileUnitCentroid -> float32

[<Interface>]
type ICatlikeCodingNoiseQuery =
    abstract Perturb: CatlikeCodingNoisePerturb
    abstract GetHeight: CatlikeCodingNoiseGetHeight

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-21 20:39:21
module CatlikeCodingNoiseQuery =
    let noiseScale = 0.003f

    let private sampleNoise (planet: IPlanet) (catlikeCodingNoise: ICatlikeCodingNoise) =
        fun (position: Vector3) ->
            TextureUtil.GetPixelBilinear
            <| catlikeCodingNoise.NoiseSourceImage
            <| (position.X - position.Y * 0.5f - position.Z * 0.5f) * noiseScale
               / planet.StandardScale
            <| (position.Y - position.Z) * HexMetrics.OuterToInner * noiseScale
               / planet.StandardScale

    let cellPerturbStrength = 4f

    let perturb (planet: IPlanet) (catlikeCodingNoise: ICatlikeCodingNoise) =
        fun (position: Vector3) ->
            let sample =
                sampleNoise planet catlikeCodingNoise
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
                * planet.StandardScale
                * position.Length()
                / HexMetrics.StandardRadius

            let z =
                vecZ
                * (sample.Z * 2f - 1f)
                * cellPerturbStrength
                * planet.StandardScale
                * position.Length()
                / HexMetrics.StandardRadius

            position + x + z

    let elevationPerturbStrength = 0.5f

    let getHeight (planet: IPlanet) (catlikeCodingNoise: ICatlikeCodingNoise) =
        let getPerturbHeight (centroid: Vector3) =
            ((sampleNoise planet catlikeCodingNoise centroid).Y * 2f - 1f)
            * elevationPerturbStrength

        let getHeightInner (elevation: int) (centroid: Vector3) =
            (float32 elevation + getPerturbHeight centroid) * planet.UnitHeight

        fun (tileValue: TileValue) (tileUnitCentroid: TileUnitCentroid) ->
            let centroid = tileUnitCentroid.Scaled HexMetrics.StandardRadius
            getHeightInner tileValue.Elevation centroid
