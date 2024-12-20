namespace FrontEndToolFS.SebastianPlanet

open Godot

type FilterType =
    | Simple = 0
    | Rigid = 1

type NoiseSettings() =
    member val filterType = FilterType.Simple with get, set
    member val strength = 1f with get, set
    member val numLayers = 1 with get, set
    member val baseRoughness = 1f with get, set
    member val roughness = 2f with get, set
    member val persistence = 0.5f with get, set
    member val center = Vector3.Zero with get, set
    member val minValue = 0f with get, set
    member val weightMultiplier = 0.8f with get, set
