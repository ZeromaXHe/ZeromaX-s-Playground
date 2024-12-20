namespace FrontEndToolFS.SebastianPlanet

open Godot

type ColorSettings() =
    member val gradient: Gradient = null with get, set
    member val planetMaterial: ShaderMaterial = null with get, set
