namespace FrontEndToolFS.SebastianPlanet

type INoiseLayer =
    abstract member Enabled: bool
    abstract member UseFirstLayerAsMask: bool
    abstract member noiseSettings: INoiseSettings

type IShapeSettings =
    abstract member PlanetRadius: float32
    abstract member noiseLayers: INoiseLayer array
