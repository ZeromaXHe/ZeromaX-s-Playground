namespace TO.Domains.Types.Maps

open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-05 10:18:05
[<Interface>]
type INoiseSetting =
    inherit IResource
    abstract Strength: float32
    abstract SampleRadius: float32
    abstract Noise: FastNoiseLite
    abstract Bias: float32
    abstract Enabled: bool
    abstract UseFirstLayerAsMask: bool

[<Interface>]
type ILayeredFastNoise =
    inherit IResource
    abstract GetNoiseLayerByIdx: int -> INoiseSetting
    abstract GetNoiseLayersLength: unit -> int

[<Interface>]
type IFractalNoiseLandGenerator =
    inherit ILandGenerator
    abstract GetLayeredNoises: unit -> ILayeredFastNoise
