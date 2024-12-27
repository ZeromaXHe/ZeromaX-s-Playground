namespace FrontEndToolFS.SebastianPlanet

open Godot

type FilterType =
    | Simple = 0
    | Rigid = 1

type INoiseSettings =
    abstract member FilterType: FilterType
    abstract member Strength: float32
    // 1 ~ 8
    abstract member NumLayers: int
    abstract member BaseRoughness: float32
    // 1.0 ~ 10.0
    abstract member Roughness: float32
    // 0.0 ~ 1.0
    abstract member Persistence: float32
    abstract member Center: Vector3
    abstract member MinValue: float32
    abstract member WeightMultiplier: float32
