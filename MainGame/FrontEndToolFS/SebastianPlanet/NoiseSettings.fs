namespace FrontEndToolFS.SebastianPlanet

open Godot

type NoiseSettings() =
    member val strength = 1f with get, set
    member val numLayers = 1 with get, set
    member val baseRoughness = 1f with get, set
    member val roughness = 2f with get, set
    member val persistence = 0.5f with get, set
    member val center = Vector3.Zero with get, set
    member val minValue = 0f with get, set
