namespace FrontEndToolFS.SebastianPlanet

type NoiseLayer() =
    member val enabled = true with get, set
    member val useFirstLayerAsMask = false with get, set
    member val noiseSettings = NoiseSettings()

type ShapeSettings() =
    member val planetRadius = 1f with get, set
    member val layerCount = 2 with get, set
    member val noiseLayers: NoiseLayer array = Array.init 2 (fun _ -> NoiseLayer()) with get, set
