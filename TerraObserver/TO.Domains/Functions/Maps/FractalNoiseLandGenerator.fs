namespace TO.Domains.Functions.Maps

open System
open Godot
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.Maps

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-05 10:25:05
module NoiseSettings =
    let getNoise3Dv (v: Vector3) (this: INoiseSetting) =
        if this.Noise = null then
            this.Bias
        else
            let noiseValue = this.Noise.GetNoise3Dv <| v * this.SampleRadius
            (noiseValue + this.Bias) * this.Strength

module LayeredFastNoise =
    let private getLayeredNoise (getNoise3Dv: INoiseSetting -> float32) (this: ILayeredFastNoise) =
        let mutable noiseSum = 0f
        let mutable firstLayerValue = 0f

        if this.GetNoiseLayersLength() > 1 && this.GetNoiseLayerByIdx 0 |> _.Enabled then
            firstLayerValue <- getNoise3Dv <| this.GetNoiseLayerByIdx 0
            noiseSum <- firstLayerValue

        for i in 1 .. this.GetNoiseLayersLength() - 1 do
            let noiseLayer = this.GetNoiseLayerByIdx i

            if noiseLayer.Enabled then
                let mask =
                    if noiseLayer.UseFirstLayerAsMask then
                        firstLayerValue / (this.GetNoiseLayerByIdx 0).Strength
                    else
                        1f

                noiseSum <- noiseSum + getNoise3Dv noiseLayer * mask

        noiseSum

    let getLayeredNoise3Dv (v: Vector3) (this: ILayeredFastNoise) =
        getLayeredNoise <| NoiseSettings.getNoise3Dv v <| this

module FractalNoiseLandGeneratorCommand =
    let createLand
        (env:
            'E
                when 'E :> IHexMapGeneratorQuery
                and 'E :> IPlanetConfigQuery
                and 'E :> ITileQuery
                and 'E :> IEntityStoreCommand)
        (landGen: IFractalNoiseLandGenerator)
        =
        let radius = env.PlanetConfig.Radius
        let elevationStep = env.PlanetConfig.ElevationStep
        let rng = env.HexMapGenerator.Rng
        let defaultWaterLevel = env.HexMapGenerator.DefaultWaterLevel
        let origin = Vector3(rng.Randf(), rng.Randf(), rng.Randf()) * radius
        let mutable minNoise = Single.MaxValue
        let mutable maxNoise = Single.MinValue

        for tile in env.GetAllTiles() do
            let noise =
                landGen.GetLayeredNoises()
                |> LayeredFastNoise.getLayeredNoise3Dv ((tile |> Tile.unitCentroid |> _.UnitCentroid) * radius + origin)

            if noise > maxNoise then
                maxNoise <- noise

            if noise < minNoise then
                minNoise <- noise

        let mutable landCount = 0

        env.ExecuteInCommandBuffer(fun cb ->
            for tile in env.GetAllTiles() do
                let noise =
                    landGen.GetLayeredNoises()
                    |> LayeredFastNoise.getLayeredNoise3Dv (
                        (tile |> Tile.unitCentroid |> _.UnitCentroid) * radius + origin
                    )

                let noiseDiff = if noise > 0f then noise else noise - minNoise

                let elevation =
                    Mathf.RoundToInt(
                        if noise > 0f then
                            float32 defaultWaterLevel
                            + float32 (elevationStep - defaultWaterLevel) * noiseDiff / maxNoise
                        else
                            float32 defaultWaterLevel * noiseDiff / -minNoise
                    )

                let newTileValue = tile |> Tile.value |> TileValue.withElevation elevation
                cb.AddComponent<TileValue>(tile.Id, &newTileValue)

                if tile |> Tile.value |> TileValue.isUnderwater |> not then
                    landCount <- landCount + 1)

        GD.Print $"------ land tiles {landCount}, min noise: {minNoise}, max noise: {maxNoise} ------"
        landCount
