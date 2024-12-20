namespace FrontEndToolFS.SebastianPlanet

open Godot

type NoiseFilter(settings: NoiseSettings) =
    let noise = MyNoise()

    member this.Evaluate(point: Vector3) =
        let mutable noiseValue = 0f
        let mutable frequency = settings.baseRoughness
        let mutable amplitude = 1f

        for i in 0 .. settings.numLayers - 1 do
            let v = noise.Evaluate <| point * frequency + settings.center
            noiseValue <- noiseValue + (v + 1f) * 0.5f * amplitude
            frequency <- frequency * settings.roughness
            amplitude <- amplitude * settings.persistence

        noiseValue <- Mathf.Max(0f, noiseValue - settings.minValue)
        noiseValue * settings.strength
