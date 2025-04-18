namespace FrontEndToolFS.SebastianPlanet

open Godot
open Microsoft.FSharp.Core

type ColorGenerator(settings: IColorSettings) =
    let mutable settings = settings
    let mutable texture: Image = null
    let textureResolution = 50

    let mutable biomeNoiseFilter =
        NoiseFilterFactory.createNoiseFilter settings.biomeColorSettings.noise

    member this.UpdateSettings(settingsIn: IColorSettings) =
        settings <- settingsIn

        if
            texture = null
            || texture.GetWidth() <> textureResolution * 2
            || texture.GetHeight() <> settings.biomeColorSettings.biomes.Length
        then
            texture <-
                Image.CreateEmpty(
                    textureResolution * 2,
                    settings.biomeColorSettings.biomes.Length,
                    false,
                    Image.Format.Rgba8
                )

        biomeNoiseFilter <- NoiseFilterFactory.createNoiseFilter settings.biomeColorSettings.noise

    member this.UpdateElevation(elevationMinMax: MinMax) =
        RenderingServer.GlobalShaderParameterSet("elevation_min_max", Vector2(elevationMinMax.Min, elevationMinMax.Max))

    member this.BiomePercentFromPoint(pointOnUnitSphere: Vector3) =
        let mutable heightPercent = (pointOnUnitSphere.Y + 1f) / 2f

        heightPercent <-
            heightPercent
            + (biomeNoiseFilter.Evaluate pointOnUnitSphere
               - settings.biomeColorSettings.NoiseOffset)
              * settings.biomeColorSettings.NoiseStrength

        let mutable biomeIndex = 0f
        let numBiome = settings.biomeColorSettings.biomes.Length
        let blendRange = settings.biomeColorSettings.BlendAmount / 2f + 0.001f

        for i in 0 .. numBiome - 1 do
            let dst = heightPercent - settings.biomeColorSettings.biomes[i].StartHeight
            // Unity 的 InverseLerp 是会自动 Clamp
            let weight = Mathf.Clamp(Mathf.InverseLerp(-blendRange, blendRange, dst), 0f, 1f)
            biomeIndex <- biomeIndex * (1f - weight)
            biomeIndex <- biomeIndex + float32 i * weight

        biomeIndex / float32 (Mathf.Max(1, numBiome - 1))

    member this.UpdateColors() =
        let mutable colorIndex = 0

        for biome in settings.biomeColorSettings.biomes do
            for i in 0 .. textureResolution * 2 - 1 do
                let gradientColor =
                    if i < textureResolution then
                        settings.OceanColor.Sample(float32 i / float32 (textureResolution - 1))
                    else
                        biome.Gradient.Sample(float32 (i - textureResolution) / float32 (textureResolution - 1))

                let tintColor = biome.Tint
                let y = colorIndex / textureResolution / 2
                texture.SetPixel(i, y, gradientColor * (1f - biome.TintPercent) + tintColor * biome.TintPercent)
                colorIndex <- colorIndex + 1
                    

        settings.PlanetMaterial.SetShaderParameter(
            "planet_texture",
            ImageTexture.CreateFromImage texture |> Variant.CreateFrom
        )
