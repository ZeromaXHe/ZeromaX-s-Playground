namespace FrontEndToolFS.SebastianPlanet

open Godot

type IBiome =
    abstract member Gradient: Gradient
    abstract member Tint: Color
    // 0.0 ~ 1.0
    abstract member StartHeight: float32
    // 0.0 ~ 1.0
    abstract member TintPercent: float32

type IBiomeColorSettings =
    abstract member biomes: IBiome array
    abstract member noise: INoiseSettings
    abstract member NoiseOffset: float32
    abstract member NoiseStrength: float32
    // 0.0 ~ 1.0
    abstract member BlendAmount: float32

type IColorSettings =
    abstract member PlanetMaterial: ShaderMaterial
    abstract member biomeColorSettings: IBiomeColorSettings
    abstract member OceanColor: Gradient
