namespace FrontEndToolFS.SebastianPlanet

open Godot

type ColorGenerator(settings: ColorSettings) =
    let mutable settings = settings
    let mutable texture: GradientTexture2D = null

    member this.UpdateSettings(settingsIn: ColorSettings) =
        settings <- settingsIn
        if texture = null then
            texture <- new GradientTexture2D()

    member this.UpdateElevation(elevationMinMax: MinMax) =
        RenderingServer.GlobalShaderParameterSet("elevation_min_max", Vector2(elevationMinMax.Min, elevationMinMax.Max))

    member this.UpdateColors() =
        texture.Gradient <- settings.gradient
        settings.planetMaterial.SetShaderParameter("planet_texture", Variant.CreateFrom texture)
