namespace FrontEndToolFS.SebastianPlanet

open Godot

type INoiseFilter =
    abstract member Evaluate: Vector3 -> float32
