namespace FrontEndToolFS.SebastianPlanet

open Godot

type RigidNoiseFilter(settings: INoiseSettings) =
    let noise = MyNoise()

    interface INoiseFilter with
        override this.Evaluate point =
            let mutable noiseValue = 0f
            let mutable frequency = settings.BaseRoughness
            let mutable amplitude = 1f
            let mutable weight = 1f

            for i in 0 .. settings.NumLayers - 1 do
                let mutable v =
                    1f - Mathf.Abs(noise.Evaluate <| point * frequency + settings.Center)

                v <- v * v * weight
                weight <- Mathf.Clamp(v * settings.WeightMultiplier, 0f, 1f)
                noiseValue <- noiseValue + v * amplitude
                frequency <- frequency * settings.Roughness
                amplitude <- amplitude * settings.Persistence

            noiseValue <- noiseValue - settings.MinValue
            noiseValue * settings.Strength
