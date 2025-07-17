namespace TO.Domains.Types.Maps

type MapRegion() =
    member val IcosahedronIds: int array = null with get, set

[<Interface>]
type IErosionLandGenerator =
    inherit ILandGenerator
    // Catlike Coding 侵蚀算法设置
    // =====【Export】=====
    abstract LandPercentage: int
    abstract ChunkSizeMin: int
    abstract ChunkSizeMax: int
    abstract HighRiseProbability: float32
    abstract SinkProbability: float32
    abstract JitterProbability: float32
    abstract ErosionPercentage: int
    // =====【普通属性】=====
    abstract Regions: MapRegion ResizeArray
