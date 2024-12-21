namespace FrontEndToolFS.SebastianPlanet

open Godot

type ShapeGenerator(settings: ShapeSettings) =
    let mutable settings = settings
    member val noiseFilters: INoiseFilter array = null with get, set

    member val elevationMinMax = MinMax()

    member this.UpdateSettings(settingsIn: ShapeSettings) =
        settings <- settingsIn

        this.noiseFilters <-
            Array.init settings.noiseLayers.Length (fun i ->
                NoiseFilterFactory.createNoiseFilter settings.noiseLayers[i].noiseSettings)

    member this.CalculateUnscaledElevation(pointOnUnitSphere: Vector3) =
        let mutable firstLayerValue = 0f
        let mutable elevation = 0f

        if this.noiseFilters.Length > 0 then
            firstLayerValue <- this.noiseFilters[0].Evaluate pointOnUnitSphere

            if settings.noiseLayers[0].enabled then
                elevation <- firstLayerValue

        for i in 1 .. this.noiseFilters.Length - 1 do
            if settings.noiseLayers[i].enabled then
                let mask =
                    if settings.noiseLayers[i].useFirstLayerAsMask then
                        firstLayerValue
                    else
                        1f

                elevation <- elevation + this.noiseFilters[i].Evaluate pointOnUnitSphere * mask

        this.elevationMinMax.AddValue elevation
        elevation

    member this.GetScaledElevation unscaledElevation =
        let elevation = Mathf.Max(0f, unscaledElevation)
        settings.planetRadius * (1f + elevation)
