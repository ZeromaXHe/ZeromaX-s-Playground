namespace FrontEndToolFS.SebatianPlanet

open Godot

type ShapeGenerator(settings: ShapeSettings) =
    member this.CalculatePointOnPlanet(pointOnUnitSphere: Vector3) =
        pointOnUnitSphere * settings.planetRadius
