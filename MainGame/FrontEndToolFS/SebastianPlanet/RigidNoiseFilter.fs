namespace FrontEndToolFS.SebastianPlanet

open Godot

type RigidNoiseFilter(settings: NoiseSettings) =
    let noise = MyNoise()

    interface INoiseFilter with
        override this.Evaluate(point: Vector3) =
            let mutable noiseValue = 0f
            let mutable frequency = settings.baseRoughness
            let mutable amplitude = 1f
            let mutable weight = 1f

            for i in 0 .. settings.numLayers - 1 do
                let mutable v =
                    1f - Mathf.Abs(noise.Evaluate <| point * frequency + settings.center)

                v <- v * v * weight
                weight <- Mathf.Clamp(v * settings.weightMultiplier, 0f, 1f)
                noiseValue <- noiseValue + v * amplitude
                frequency <- frequency * settings.roughness
                amplitude <- amplitude * settings.persistence

            noiseValue <- noiseValue - settings.minValue
            noiseValue * settings.strength
