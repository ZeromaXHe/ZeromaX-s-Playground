namespace TO.FSharp.Repos.Models.MapGenerators

open System
open System.Diagnostics
open Godot
open Godot.Abstractions.Extensions.Maps

type MapRegion() =
    member val IcosahedronIds: int array = null with get, set

[<Struct>]
type private ClimateData =
    val mutable Clouds: float32
    val mutable Moisture: float32

[<Struct>]
type private Biome =
    val Terrain: int
    val Plant: int
    new(terrain, plant) = { Terrain = terrain; Plant = plant }

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-07 09:48:07
type MapGenerator() =
    let rng = new RandomNumberGenerator()
    let mutable landTileCount = 0
    let regions = ResizeArray<MapRegion>()
    let mutable climate = ResizeArray<ClimateData>()
    let mutable nextClimate = ResizeArray<ClimateData>()
    let mutable temperatureJitterChannel = 0
    let temperatureBands = [| 0.1f; 0.3f; 0.6f |]
    let moistureBands = [| 0.12f; 0.28f; 0.85f |]

    let biomes =
        [| [| Biome(0, 0); Biome(4, 0); Biome(4, 0); Biome(4, 0) |]
           [| Biome(0, 0); Biome(2, 0); Biome(2, 1); Biome(2, 2) |]
           [| Biome(0, 0); Biome(1, 0); Biome(1, 1); Biome(1, 2) |]
           [| Biome(0, 0); Biome(1, 1); Biome(1, 2); Biome(1, 3) |] |]

    let setRngSeed (hexMapGenerator: IHexMapGenerator) =
        let initState = rng.State

        if not hexMapGenerator.UseFixedSeed then
            rng.Randomize()

            hexMapGenerator.Seed <-
                rng.RandiRange(0, Int32.MaxValue)
                ^^^ int DateTime.Now.Ticks
                ^^^ (int <| Time.GetTicksMsec())
                &&& Int32.MaxValue

        GD.Print $"Generating map with seed {hexMapGenerator.Seed}"
        rng.Seed <- uint64 hexMapGenerator.Seed
        initState

    let createRegions (regionBorder: int) =
        regions.Clear()
        let borderX = regionBorder
        let region = MapRegion()
        regions.Add region

    let getRandomCellIndex tileCount = GD.RandRange(1, tileCount)
    
    // let sinkTerrain (highRiseProbability: float32) chunkSize budget tileCount =
    //     let firstTileId = getRandomCellIndex tileCount
    //     let sink = if rng.Randf() < highRiseProbability then 2 else 1
    //     
    //
    // let createLand (tileCount: int) (hexMapGenerator: IHexMapGenerator)=
    //     let mutable landTileCount = Mathf.RoundToInt(float32 tileCount * hexMapGenerator.LandPercentage * 0.01f)
    //     let mutable landBudget = landTileCount
    //     // 根据地图尺寸来设置对应循环次数上限，保证大地图也能尽量用完 landBudget
    //     let mutable guard = 0 // 防止无限循环的守卫值
    //     let mutable returnNow = false
    //     while guard < landTileCount && not returnNow do
    //         let sink = rng.Randf() < hexMapGenerator.SinkProbability
    //         for region in regions do
    //             let chunkSize = rng.RandiRange(hexMapGenerator.ChunkSizeMin, hexMapGenerator.ChunkSizeMax)
    //             if sink then
    //                 landBudget <- 
    //
    // member this.GenerateMap(hexMapGenerator: IHexMapGenerator) =
    //     let time = Time.GetTicksMsec()
    //     let stopwatch = Stopwatch()
    //     stopwatch.Start()
    //     let initState = setRngSeed hexMapGenerator
    //     createRegions hexMapGenerator.RegionBorder
    //     GD.Print $"--- CreatedRegions in {stopwatch.ElapsedMilliseconds} ms"
    //     stopwatch.Restart()
    //     landTileCount <- 
