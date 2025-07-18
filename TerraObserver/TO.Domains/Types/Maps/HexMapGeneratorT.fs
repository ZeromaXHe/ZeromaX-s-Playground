namespace TO.Domains.Types.Maps

open Godot
open TO.Domains.Types.Godots

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

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-07 10:15:07
[<Interface>]
type IHexMapGenerator =
    inherit IResource
    // =====【Export】=====
    abstract GetLandGenerator: ILandGenerator
    abstract UpdateLandGenerator: int64 -> unit
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
    abstract Climate: ClimateData array with get, set
    abstract NextClimate: ClimateData array with get, set
    abstract TemperatureJitterChannel: int with get, set

[<Interface>]
type IHexMapGeneratorQuery =
    abstract HexMapGenerator: IHexMapGenerator

type GenerateMap = unit -> unit
type ChangeLandGenerator = int64 -> unit

[<Interface>]
type IHexMapGeneratorCommand =
    abstract GenerateMap: GenerateMap
    abstract ChangeLandGenerator: ChangeLandGenerator
