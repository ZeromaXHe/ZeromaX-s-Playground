namespace FrontEndToolFS.SebastianPlanet

open Godot

type SimpleNoiseFilter(settings: INoiseSettings) =
    let noise = MyNoise()

    interface INoiseFilter with
        override this.Evaluate point =
            let mutable noiseValue = 0f
            let mutable frequency = settings.BaseRoughness
            let mutable amplitude = 1f

            for i in 0 .. settings.NumLayers - 1 do
                let v = noise.Evaluate <| point * frequency + settings.Center
                noiseValue <- noiseValue + (v + 1f) * 0.5f * amplitude
                frequency <- frequency * settings.Roughness
                amplitude <- amplitude * settings.Persistence

            noiseValue <- noiseValue - settings.MinValue
            noiseValue * settings.Strength
