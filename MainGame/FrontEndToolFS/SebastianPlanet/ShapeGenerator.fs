namespace FrontEndToolFS.SebastianPlanet

open Godot

type ShapeGenerator(settings: ShapeSettings) =
    member val noiseFilters: NoiseFilter array =
        Array.init settings.noiseLayers.Length (fun i -> NoiseFilter(settings.noiseLayers[i].noiseSettings)) with get, set

    member this.CalculatePointOnPlanet(pointOnUnitSphere: Vector3) =
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

        pointOnUnitSphere * settings.planetRadius * (1f + elevation)
