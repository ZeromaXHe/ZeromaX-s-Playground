namespace TO.Domains.Functions.Maps

open System
open System.Diagnostics
open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexMetrics
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.Maps

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-07 09:48:07
module HexMapGeneratorCommand =
    let temperatureBands = [| 0.1f; 0.3f; 0.6f |]
    let moistureBands = [| 0.12f; 0.28f; 0.85f |]

    let biomes =
        [| [| Biome(0, 0); Biome(4, 0); Biome(4, 0); Biome(4, 0) |]
           [| Biome(0, 0); Biome(2, 0); Biome(2, 1); Biome(2, 2) |]
           [| Biome(0, 0); Biome(1, 0); Biome(1, 1); Biome(1, 2) |]
           [| Biome(0, 0); Biome(1, 1); Biome(1, 2); Biome(1, 3) |] |]

    let private setRngSeed (env: #IHexMapGeneratorQuery) =
        let generator = env.HexMapGenerator
        let initState = generator.Rng.State

        if not generator.UseFixedSeed then
            generator.Rng.Randomize()

            generator.Seed <-
                generator.Rng.RandiRange(0, Int32.MaxValue)
                ^^^ int DateTime.Now.Ticks
                ^^^ (int <| Time.GetTicksMsec())
                &&& Int32.MaxValue

        GD.Print $"Generating map with seed {generator.Seed}"
        generator.Rng.Seed <- uint64 generator.Seed
        initState

    let private evolveClimate
        (env:
            'E when 'E :> IHexMapGeneratorQuery and 'E :> IPlanetConfigQuery and 'E :> ITileQuery and 'E :> IPointQuery)
        (tile: Entity)
        =
        let this = env.HexMapGenerator
        let tileCountId = tile |> Tile.countId |> _.CountId
        let mutable tileClimate = this.Climate[tileCountId]

        if tile |> Tile.value |> TileValue.isUnderwater then
            tileClimate.Moisture <- 1f
            tileClimate.Clouds <- tileClimate.Clouds + this.EvaporationFactor
        else
            let evaporation = tileClimate.Moisture * this.EvaporationFactor
            tileClimate.Moisture <- tileClimate.Moisture - evaporation
            tileClimate.Clouds <- tileClimate.Clouds + evaporation

        let precipitation = tileClimate.Clouds * this.PrecipitationFactor
        tileClimate.Clouds <- tileClimate.Clouds - precipitation
        tileClimate.Moisture <- tileClimate.Moisture + precipitation

        let cloudMaximum =
            1f
            - (tile |> Tile.value |> TileValue.viewElevation |> float32)
              / (float32 env.PlanetConfig.ElevationStep + 1f)

        if tileClimate.Clouds > cloudMaximum then
            tileClimate.Moisture <- tileClimate.Moisture + (tileClimate.Clouds - cloudMaximum)
            tileClimate.Clouds <- cloudMaximum

        let edgeCount = tile |> Tile.hexFaceIds |> _.Length

        let mainDispersalDirection =
            edgeCount |> HexIndexUtil.oppositeIdx this.WindDirection

        let cloudDispersal =
            tileClimate.Clouds * (1f / (float32 edgeCount - 1f + this.WindStrength))

        let runoff = tileClimate.Moisture * this.RunoffFactor * (1f / float32 edgeCount)
        let seepage = tileClimate.Moisture * this.SeepageFactor * (1f / float32 edgeCount)

        for neighbor in env.GetNeighborTiles tile.Id do
            let neighborCountId = neighbor |> Tile.countId |> _.CountId
            let mutable neighborClimate = this.NextClimate[neighborCountId]

            if env.GetNeighborIdx tile.Id neighbor.Id = mainDispersalDirection then
                neighborClimate.Clouds <- neighborClimate.Clouds + cloudDispersal * this.WindStrength
            else
                neighborClimate.Clouds <- neighborClimate.Clouds + cloudDispersal

            let elevationDelta =
                (neighbor |> Tile.value |> TileValue.viewElevation)
                - (tile |> Tile.value |> TileValue.viewElevation)

            if elevationDelta < 0 then
                tileClimate.Moisture <- tileClimate.Moisture - runoff
                neighborClimate.Moisture <- neighborClimate.Moisture + runoff
            elif elevationDelta = 0 then
                tileClimate.Moisture <- tileClimate.Moisture - seepage
                neighborClimate.Moisture <- neighborClimate.Moisture + seepage

            this.NextClimate[neighborCountId] <- neighborClimate

        let mutable nextTileClimate = this.NextClimate[tileCountId]
        nextTileClimate.Moisture <- nextTileClimate.Moisture + tileClimate.Moisture

        if nextTileClimate.Moisture > 1f then
            nextTileClimate.Moisture <- 1f

        this.NextClimate[tileCountId] <- nextTileClimate
        this.Climate[tileCountId] <- ClimateData()

    let private createClimate (env: 'E when 'E :> IHexMapGeneratorQuery and 'E :> ITileQuery) tileCount =
        let this = env.HexMapGenerator
        this.Climate.Clear()
        this.NextClimate.Clear()
        let mutable initialData = ClimateData()
        initialData.Moisture <- this.StartingMoisture
        let clearData = ClimateData()

        for i in 0..tileCount do
            this.Climate.Add initialData
            this.NextClimate.Add clearData

        for cycle in 0..39 do
            for tile in env.GetAllTiles() do
                evolveClimate env tile

            let temp = this.Climate
            this.Climate <- this.NextClimate
            this.NextClimate <- temp

    let private createRiver
        (env: 'E when 'E :> ITileQuery and 'E :> IPointQuery and 'E :> ITileCommand and 'E :> IHexMapGeneratorQuery)
        (origin: Entity)
        =
        let this = env.HexMapGenerator
        let mutable length = 1
        let mutable tile = origin
        let mutable direction = 0
        let flowDirections = ResizeArray<int>()
        let mutable directReturn = false
        let mutable breakWhile = false

        while not breakWhile
              && not directReturn
              && tile |> Tile.value |> TileValue.isUnderwater |> not do
            let mutable minNeighborElevation = Int32.MaxValue
            flowDirections.Clear()
            let neighbors = env.GetNeighborTiles tile.Id |> Seq.toList

            for neighbor in neighbors do
                if not directReturn then
                    let neighborElevation = neighbor |> Tile.value |> TileValue.elevation

                    if neighborElevation < minNeighborElevation then
                        minNeighborElevation <- neighborElevation

                    if neighbor = origin || neighbor |> Tile.flag |> TileFlag.hasIncomingRiver then
                        () // continue
                    else
                        let delta = neighborElevation - (tile |> Tile.value |> TileValue.elevation)

                        if delta > 0 then
                            () // continue
                        else
                            if neighbor |> Tile.flag |> TileFlag.hasOutgoingRiver then
                                env.SetOutgoingRiver tile neighbor
                                directReturn <- true

                            if not directReturn then
                                let d = env.GetNeighborIdx tile.Id neighbor.Id

                                if delta < 0 then
                                    flowDirections.Add d
                                    flowDirections.Add d
                                    flowDirections.Add d

                                let tileEdgeCount = tile |> Tile.hexFaceIds |> _.Length

                                if
                                    length = 1
                                    || (d <> HexIndexUtil.next2Idx direction tileEdgeCount
                                        && d <> HexIndexUtil.previous2Idx direction tileEdgeCount)
                                then
                                    flowDirections.Add d

                                flowDirections.Add d

            if not directReturn then
                if flowDirections.Count = 0 then
                    if length = 1 then
                        directReturn <- true
                    elif minNeighborElevation >= (tile |> Tile.value |> TileValue.elevation) then
                        let mutable newTileValue =
                            tile |> Tile.value |> TileValue.withWaterLevel (minNeighborElevation + 1)

                        if minNeighborElevation = (newTileValue |> TileValue.elevation) then
                            newTileValue <- newTileValue |> TileValue.withElevation (minNeighborElevation - 1)

                        tile.AddComponent<TileValue>(&newTileValue) |> ignore

                    breakWhile <- true

                if not directReturn && not breakWhile then
                    direction <- flowDirections[this.Rng.RandiRange(0, flowDirections.Count - 1)]
                    let riverToTile = env.GetNeighborTileByIdx tile.Id direction
                    env.SetOutgoingRiver tile riverToTile
                    length <- length + 1

                    if
                        minNeighborElevation >= (tile |> Tile.value |> TileValue.elevation)
                        && this.Rng.Randf() < this.ExtraLakeProbability
                    then
                        // 湖泊
                        let mutable newTileValue =
                            tile
                            |> Tile.value
                            |> TileValue.withWaterLevel (tile |> Tile.value |> TileValue.elevation)

                        for neighbor in neighbors do
                            let newNeighborValue =
                                neighbor
                                |> Tile.value
                                |> TileValue.withWaterLevel (newTileValue |> TileValue.waterLevel)

                            neighbor.AddComponent<TileValue>(&newNeighborValue) |> ignore

                        newTileValue <- newTileValue |> TileValue.withElevation (TileValue.elevation newTileValue - 1)
                        tile.AddComponent<TileValue>(&newTileValue) |> ignore

                    tile <- riverToTile

        length

    let private createRivers
        (env: 'E when 'E :> IHexMapGeneratorQuery and 'E :> ITileQuery and 'E :> IPlanetConfigQuery)
        =
        let this = env.HexMapGenerator
        let riverOrigins = ResizeArray<Entity>()

        for tile in env.GetAllTiles() do
            if tile |> Tile.value |> TileValue.isUnderwater then
                ()
            else
                let tileCountId = tile |> Tile.countId |> _.CountId
                let data = this.Climate[tileCountId]

                let weight =
                    data.Moisture
                    * float32 ((tile |> Tile.value |> TileValue.elevation) - this.DefaultWaterLevel)
                    / float32 (env.PlanetConfig.ElevationStep - this.DefaultWaterLevel)

                if weight > 0.75f then
                    riverOrigins.Add tile
                    riverOrigins.Add tile

                if weight > 0.5f then
                    riverOrigins.Add tile

                if weight > 0.25f then
                    riverOrigins.Add tile

        let mutable riverBudget =
            Mathf.RoundToInt(float32 this.LandTileCount * this.RiverPercentage * 0.01f)

        GD.Print $"{riverOrigins.Count} river origins with river budget {riverBudget}"

        while riverBudget > 0 && riverOrigins.Count > 0 do
            let lastIndex = riverOrigins.Count - 1
            let index = this.Rng.RandiRange(0, lastIndex)
            let origin = riverOrigins[index]
            riverOrigins[index] <- riverOrigins[lastIndex]
            riverOrigins.RemoveAt lastIndex

            if origin |> Tile.flag |> TileFlag.hasRivers |> not then
                let isValidOrigin =
                    env.GetNeighborTiles origin.Id
                    |> Seq.forall (fun n ->
                        (n |> Tile.flag |> TileFlag.hasRivers |> not)
                        && (n |> Tile.value |> TileValue.isUnderwater |> not))

                if isValidOrigin then
                    riverBudget <- riverBudget - createRiver env origin

        if riverBudget > 0 then
            GD.PrintErr $"Failed to use up river budget {riverBudget}"

    let private determineTemperature
        (env:
            'E
                when 'E :> IHexMapGeneratorQuery
                and 'E :> IPlanetConfigQuery
                and 'E :> ITileQuery
                and 'E :> ICatlikeCodingNoiseQuery)
        (tile: Entity)
        =
        let this = env.HexMapGenerator
        let planet = env.PlanetConfig
        let sphereAxial = env.GetSphereAxial tile

        let mutable latitude =
            float32 (sphereAxial.Coords.R + planet.Divisions)
            / (3f * float32 planet.Divisions)
        // 具有南北半球
        latitude <- latitude * 2f

        if latitude > 1f then
            latitude <- 2f - latitude

        let temperature =
            Mathf.Lerp(this.LowTemperature, this.HighTemperature, latitude)
            * (1f
               - float32 ((tile |> Tile.value |> TileValue.viewElevation) - this.DefaultWaterLevel)
                 / float32 (planet.ElevationStep - this.DefaultWaterLevel + 1))

        let jitter =
            (tile
             |> Tile.unitCentroid
             |> TileUnitCentroid.scaled HexMetrics.StandardRadius
             |> env.SampleNoise)[this.TemperatureJitterChannel]

        temperature + (jitter * 2f - 1f) * this.TemperatureJitter

    let private setTerrainType
        (env:
            'E
                when 'E :> IHexMapGeneratorQuery
                and 'E :> IPlanetConfigQuery
                and 'E :> IEntityStoreCommand
                and 'E :> IEntityStoreQuery)
        =
        let this = env.HexMapGenerator
        let planetConfig = env.PlanetConfig
        this.TemperatureJitterChannel <- this.Rng.RandiRange(0, 3)

        let rockDesertElevation =
            planetConfig.ElevationStep
            - (planetConfig.ElevationStep - this.DefaultWaterLevel) / 2

        env.ExecuteInCommandBuffer(fun cb ->
            env
                .Query<TileValue>()
                .ForEachEntity(fun tileValue tile ->
                    let temperature = determineTemperature env tile
                    let tileCountId = tile |> Tile.countId |> _.CountId
                    let tileElevation = tileValue |> TileValue.elevation
                    let moisture = this.Climate[tileCountId].Moisture

                    if tileValue |> TileValue.isUnderwater |> not then
                        let mutable t = 0

                        while t < temperatureBands.Length && temperature >= temperatureBands[t] do
                            t <- t + 1

                        let mutable m = 0

                        while m < moistureBands.Length && moisture >= moistureBands[m] do
                            m <- m + 1

                        let mutable tileBiome = biomes[t][m]

                        if tileBiome.Terrain = 0 then
                            // 假设如果一个单元格的高度比水位更接近最高高度，沙子就会变成岩石。这是岩石沙漠高程线
                            if tileElevation >= rockDesertElevation then
                                tileBiome.Terrain <- 3
                        elif tileElevation = planetConfig.ElevationStep then
                            // 强制处于最高海拔的单元格变成雪盖，无论它们有多暖和，只要它们不太干燥
                            tileBiome.Terrain <- 4
                        // 确保植物不会出现在雪地上
                        if tileBiome.Terrain = 4 then
                            tileBiome.Plant <- 0
                        elif tileBiome.Plant < 3 && tile |> Tile.flag |> TileFlag.hasRivers then
                            // 如果等级还没有达到最高点，让我们也增加河流沿岸的植物等级
                            tileBiome.Plant <- tileBiome.Plant + 1

                        let newTileValue =
                            tileValue
                            |> TileValue.withTerrainTypeIndex tileBiome.Terrain
                            |> TileValue.withPlantLevel tileBiome.Plant

                        cb.AddComponent<TileValue>(tile.Id, &newTileValue)
                    else
                        let terrain =
                            if tileElevation = this.DefaultWaterLevel - 1 then
                                let mutable cliffs = 0
                                let mutable slopes = 0

                                for neighbor in env.GetNeighborTiles tile.Id do
                                    let delta = (neighbor |> Tile.value |> TileValue.elevation) - tileElevation

                                    if delta = 0 then
                                        slopes <- slopes + 1
                                    elif delta > 0 then
                                        cliffs <- cliffs + 1

                                if cliffs + slopes > 3 then 1
                                elif cliffs > 0 then 3
                                elif slopes > 0 then 0
                                else 1
                            elif tileElevation >= this.DefaultWaterLevel then
                                // 用草来建造比水位更高的单元格，这些是由河流形成的湖泊
                                1
                            elif tileElevation < 0 then
                                // 负海拔的单元格位于深处，让我们用岩石来做
                                3
                            else
                                2

                        let newTileValue =
                            tileValue
                            |> TileValue.withTerrainTypeIndex (
                                if terrain = 1 && temperature < temperatureBands[0] then
                                    2
                                else
                                    terrain
                            )

                        cb.AddComponent<TileValue>(tile.Id, &newTileValue)))

    let private resetRng (env: #IHexMapGeneratorQuery) (initState: uint64) =
        env.HexMapGenerator.Rng.State <- initState

    let generateMap
        (env: 'E when 'E :> IEntityStoreQuery and 'E :> IEntityStoreCommand and 'E :> IHexMapGeneratorQuery)
        : GenerateMap =
        fun () ->
            let time = Time.GetTicksMsec()
            let generator = env.HexMapGenerator
            let stopwatch = Stopwatch()
            stopwatch.Start()
            let initState = setRngSeed env

            env.ExecuteInCommandBuffer(fun cb ->
                env
                    .Query<TileValue>()
                    .ForEachEntity(fun tileValue tile ->
                        let newValue =
                            tileValue
                            |> TileValue.withWaterLevel generator.DefaultWaterLevel
                            |> TileValue.withElevation 0

                        cb.AddComponent<TileValue>(tile.Id, &newValue)))

            let tileCount = env.Query<TileValue>().Count

            match generator.GetLandGenerator with
            | :? IErosionLandGenerator as erosionLandGen ->
                ErosionLandGeneratorCommand.createRegions erosionLandGen generator.RegionBorder
                GD.Print $"--- CreatedRegions in {stopwatch.ElapsedMilliseconds} ms"
                stopwatch.Restart()
                generator.LandTileCount <- ErosionLandGeneratorCommand.createLand env tileCount erosionLandGen

                GD.Print
                    $"--- CreatedLand {generator.LandTileCount}/{tileCount} tiles in {stopwatch.ElapsedMilliseconds} ms"

                stopwatch.Restart()
                ErosionLandGeneratorCommand.erodingLand env erosionLandGen
                GD.Print $"--- ErodeLand in {stopwatch.ElapsedMilliseconds} ms"
                stopwatch.Restart()
            | :? IFractalNoiseLandGenerator as fractalNoiseLandGen ->
                generator.LandTileCount <- FractalNoiseLandGeneratorCommand.createLand env fractalNoiseLandGen

                GD.Print
                    $"--- CreatedLand {generator.LandTileCount}/{tileCount} tiles in {stopwatch.ElapsedMilliseconds} ms"

                stopwatch.Restart()
            | _ -> ()

            createClimate env tileCount
            GD.Print $"--- CreateClimate in {stopwatch.ElapsedMilliseconds} ms"
            stopwatch.Restart()

            createRivers env
            GD.Print $"--- CreateRivers in {stopwatch.ElapsedMilliseconds} ms"
            stopwatch.Restart()

            setTerrainType env
            resetRng env initState
            GD.Print $"--- SetTerrainType in {stopwatch.ElapsedMilliseconds} ms"
            stopwatch.Stop()
            GD.Print $"Generated map in {Time.GetTicksMsec() - time} ms"
