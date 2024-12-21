namespace FrontEndToolFS.SebastianPlanet

open Godot

type Biome() =
    member val gradient: Gradient = null with get, set
    member val tint = Colors.White with get, set
    // 0.0 ~ 1.0
    member val startHeight = 0f with get, set
    // 0.0 ~ 1.0
    member val tintPercent = 0f with get, set

type BiomeColorSettings() =
    member val biomes = Array.init 3 (fun _ -> Biome()) with get, set
    member val noise = NoiseSettings() with get, set
    member val noiseOffset = 0f with get, set
    member val noiseStrength = 0f with get, set
    // 0.0 ~ 1.0
    member val blendAmount = 0f with get, set

type ColorSettings() =
    member val planetMaterial: ShaderMaterial = null with get, set
    member val biomeColorSettings = BiomeColorSettings() with get, set
