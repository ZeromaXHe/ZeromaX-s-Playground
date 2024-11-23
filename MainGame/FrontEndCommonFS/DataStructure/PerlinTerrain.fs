namespace FrontEndCommonFS.DataStructure

open Godot

module PerlinTerrain =
    let perlinNoise =
        let noise = new FastNoiseLite()
        noise.NoiseType <- FastNoiseLite.NoiseTypeEnum.Perlin
        noise

    type ColorHeight = { Color: Color; MaxHeight: float32 }

    let colorHeights =
        [| { Color = Colors.Blue
             MaxHeight = 0.0f }
           { Color = Colors.DarkGreen
             MaxHeight = 2.0f }
           { Color = Colors.DarkOliveGreen
             MaxHeight = 4.0f }
           { Color = Colors.GreenYellow
             MaxHeight = 6.0f }
           { Color = Colors.YellowGreen
             MaxHeight = 8.0f }
           { Color = Colors.White
             MaxHeight = 10.0f } |]

    let perlinNoise3D (vec: Vector3) =
        let x = vec.X + 15.0f
        let y = vec.Y + 25.0f
        let z = vec.Z + 35.0f

        (perlinNoise.GetNoise2D(x, y)
         + perlinNoise.GetNoise2D(x, z)
         + perlinNoise.GetNoise2D(y, z)
         + perlinNoise.GetNoise2D(y, x)
         + perlinNoise.GetNoise2D(z, x)
         + perlinNoise.GetNoise2D(z, y))
        / 6.0f

    let getNoise (vec: Vector3) octaves (noiseScaling: float32) lacunarity persistence =
        [ 1..octaves ]
        |> List.fold
            (fun (value, scale, effect) i ->
                (value + effect * perlinNoise3D (vec * scale), scale * lacunarity, effect * (1.0f - persistence)))
            (0.0f, noiseScaling, 1.0f)
        |> fun (value, _, _) -> value

    let heightColor maxHeight minHeight octaves noiseScaling lacunarity persistence (center: Vector3) =
        let noise =
            getNoise (center.Normalized()) octaves noiseScaling lacunarity persistence

        let height =
            3.0f * ((maxHeight - minHeight) * noise + minHeight)
            |> Mathf.Floor
            |> fun x -> x / 3.0f

        let color =
            colorHeights
            |> Array.tryFind (fun c -> height < c.MaxHeight)
            |> function
                | Some c -> c.Color
                | None -> Colors.White

        height, color
