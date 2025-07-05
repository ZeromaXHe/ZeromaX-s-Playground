namespace TO.Domains.Types.Maps

open Godot
open TO.Domains.Types.Godots

type MapRegion() =
    member val IcosahedronIds: int array = null with get, set

[<Struct>]
type ClimateData =
    val mutable Clouds: float32
    val mutable Moisture: float32

[<Struct>]
type Biome =
    val mutable Terrain: int
    val mutable Plant: int
    new(terrain, plant) = { Terrain = terrain; Plant = plant }

[<Interface>]
type ILandGenerator =
    inherit IResource

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

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-07 10:15:07
[<Interface>]
type IHexMapGenerator =
    inherit IResource
    // =====【Export】=====
    abstract GetLandGenerator: ILandGenerator
    abstract DefaultWaterLevel: int
    abstract MapBoardX: int
    abstract MapBoardZ: int
    abstract RegionBorder: int
    abstract RegionCount: int

    abstract EvaporationFactor: float32
    abstract PrecipitationFactor: float32
    abstract RunoffFactor: float32
    abstract SeepageFactor: float32
    abstract WindDirection: int
    abstract WindStrength: float32
    abstract StartingMoisture: float32
    abstract RiverPercentage: float32
    abstract ExtraLakeProbability: float32
    abstract LowTemperature: float32
    abstract HighTemperature: float32
    abstract TemperatureJitter: float32
    abstract UseFixedSeed: bool
    abstract Seed: int with get, set
    // =====【普通属性】=====
    abstract Rng: RandomNumberGenerator
    abstract LandTileCount: int with get, set
    abstract Climate: ClimateData ResizeArray with get, set
    abstract NextClimate: ClimateData ResizeArray with get, set
    abstract TemperatureJitterChannel: int with get, set

[<Interface>]
type IHexMapGeneratorQuery =
    abstract HexMapGenerator: IHexMapGenerator

type GenerateMap = unit -> unit

[<Interface>]
type IHexMapGeneratorCommand =
    abstract GenerateMap: GenerateMap
