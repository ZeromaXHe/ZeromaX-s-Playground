namespace TO.Domains.Functions.Maps

open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexGridCoords
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Functions.PathFindings
open TO.Domains.Types.Configs
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.Maps
open TO.Domains.Types.PathFindings

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-04 10:01:04
module ErosionLandGeneratorCommand =
    let createRegions (landGen: IErosionLandGenerator) (regionBorder: int) =
        landGen.Regions.Clear()
        let borderX = regionBorder
        let region = MapRegion()
        landGen.Regions.Add region

    let private getRandomCellIndex tileCount = GD.RandRange(1, tileCount)

    let private sinkTerrain
        (env:
            'E
                when 'E :> IHexMapGeneratorQuery
                and 'E :> ITileSearcherQuery
                and 'E :> ITileSearcherCommand
                and 'E :> ITileQuery)
        (landGen: IErosionLandGenerator)
        chunkSize
        budget
        tileCount
        =
        let mapGen = env.HexMapGenerator
        let tileSearcher = env.TileSearcher
        let firstTileCountId = getRandomCellIndex tileCount

        let sink =
            if mapGen.Rng.Randf() < landGen.HighRiseProbability then
                2
            else
                1

        let mutable budget = budget
        tileSearcher.SearchFrontierPhase <- tileSearcher.SearchFrontierPhase + 1

        if tileSearcher.SearchFrontier.IsNone then
            tileSearcher.SearchFrontier <- tileSearcher.SearchData |> TilePriorityQueue |> Some

        let searchFrontier = tileSearcher.SearchFrontier.Value
        searchFrontier |> TilePriorityQueue.clear
        let firstTile = env.GetTileByCountId firstTileCountId
        tileSearcher.SearchData[firstTileCountId] <- TileSearchData(SearchPhase = tileSearcher.SearchFrontierPhase)
        searchFrontier |> TilePriorityQueue.enqueue firstTileCountId
        let center = env.GetSphereAxial firstTile
        let mutable size = 0
        let mutable currentCountId = -1

        while size < chunkSize && TilePriorityQueue.tryDequeue &currentCountId searchFrontier do
            let current = env.GetTileByCountId currentCountId
            let originalElevation = current |> Tile.value |> TileValue.elevation
            let newElevation = originalElevation - sink

            if newElevation < 0 then
                ()
            else
                let newCurrentValue = current |> Tile.value |> TileValue.withElevation newElevation
                current.AddComponent<TileValue>(&newCurrentValue) |> ignore

                if
                    originalElevation >= mapGen.DefaultWaterLevel
                    && newElevation < mapGen.DefaultWaterLevel
                then
                    budget <- budget + 1

                size <- size + 1

                for neighbor in env.GetNeighborTiles current do
                    let neighborCountId = neighbor |> Tile.countId |> _.CountId

                    if tileSearcher.SearchData[neighborCountId].SearchPhase < tileSearcher.SearchFrontierPhase then
                        tileSearcher.SearchData[neighborCountId] <-
                            TileSearchData(
                                SearchPhase = tileSearcher.SearchFrontierPhase,
                                Distance = SphereAxial.distanceTo center (env.GetSphereAxial neighbor), // 不能用管道符……
                                Heuristic =
                                    (if mapGen.Rng.Randf() < landGen.JitterProbability then
                                         1
                                     else
                                         0)
                            )

                        searchFrontier |> TilePriorityQueue.enqueue neighborCountId

        searchFrontier |> TilePriorityQueue.clear
        budget

    let private raiseTerrain
        (env:
            'E
                when 'E :> IHexMapGeneratorQuery
                and 'E :> ITileSearcherQuery
                and 'E :> ITileSearcherCommand
                and 'E :> ITileQuery
                and 'E :> IPlanetConfigQuery)
        (landGen: IErosionLandGenerator)
        chunkSize
        budget
        tileCount
        =
        let elevationStep = env.PlanetConfig.ElevationStep
        let mapGen = env.HexMapGenerator
        let tileSearcher = env.TileSearcher
        let firstTileCountId = getRandomCellIndex tileCount

        let rise =
            if mapGen.Rng.Randf() < landGen.HighRiseProbability then
                2
            else
                1

        let mutable budget = budget
        tileSearcher.SearchFrontierPhase <- tileSearcher.SearchFrontierPhase + 1

        if tileSearcher.SearchFrontier.IsNone then
            tileSearcher.SearchFrontier <- tileSearcher.SearchData |> TilePriorityQueue |> Some

        let searchFrontier = tileSearcher.SearchFrontier.Value
        searchFrontier |> TilePriorityQueue.clear
        let firstTile = env.GetTileByCountId firstTileCountId
        let firstTileCountId = firstTile |> Tile.countId |> _.CountId
        tileSearcher.SearchData[firstTileCountId] <- TileSearchData(SearchPhase = tileSearcher.SearchFrontierPhase)
        searchFrontier |> TilePriorityQueue.enqueue firstTileCountId
        let center = env.GetSphereAxial firstTile
        let mutable size = 0
        let mutable currentCountId = -1
        let mutable breakWhile = false

        while not breakWhile
              && size < chunkSize
              && TilePriorityQueue.tryDequeue &currentCountId searchFrontier do
            let current = env.GetTileByCountId currentCountId
            let originalElevation = current |> Tile.value |> TileValue.elevation
            let newElevation = originalElevation + rise

            if newElevation > elevationStep then
                ()
            else
                let newCurrentValue = current |> Tile.value |> TileValue.withElevation newElevation
                current.AddComponent<TileValue>(&newCurrentValue) |> ignore

                if
                    originalElevation < mapGen.DefaultWaterLevel
                    && newElevation >= mapGen.DefaultWaterLevel
                then
                    budget <- budget - 1

                    if budget = 0 then
                        breakWhile <- true

                if not breakWhile then
                    size <- size + 1

                    for neighbor in env.GetNeighborTiles current do
                        let neighborCountId = neighbor |> Tile.countId |> _.CountId

                        if tileSearcher.SearchData[neighborCountId].SearchPhase < tileSearcher.SearchFrontierPhase then
                            tileSearcher.SearchData[neighborCountId] <-
                                TileSearchData(
                                    SearchPhase = tileSearcher.SearchFrontierPhase,
                                    Distance = SphereAxial.distanceTo center (env.GetSphereAxial neighbor),
                                    Heuristic =
                                        (if mapGen.Rng.Randf() < landGen.JitterProbability then
                                             1
                                         else
                                             0)
                                )

                            searchFrontier |> TilePriorityQueue.enqueue neighborCountId

        searchFrontier |> TilePriorityQueue.clear
        budget

    let createLand (env: #IHexMapGeneratorQuery) (tileCount: int) (landGen: IErosionLandGenerator) =
        let mapGen = env.HexMapGenerator

        let landTileCount =
            Mathf.RoundToInt(float32 tileCount * float32 landGen.LandPercentage * 0.01f)

        let mutable landBudget = landTileCount
        // 根据地图尺寸来设置对应循环次数上限，保证大地图也能尽量用完 landBudget
        let mutable guard = 0 // 防止无限循环的守卫值
        let mutable returnNow = false

        while guard < landTileCount && not returnNow do
            let sink = mapGen.Rng.Randf() < landGen.SinkProbability

            for region in landGen.Regions do
                if not returnNow then
                    let chunkSize = mapGen.Rng.RandiRange(landGen.ChunkSizeMin, landGen.ChunkSizeMax)

                    if sink then
                        landBudget <- sinkTerrain env landGen chunkSize landBudget tileCount
                    else
                        landBudget <- raiseTerrain env landGen chunkSize landBudget tileCount

                        if landBudget <= 0 then
                            returnNow <- true

            guard <- guard + 1

        if returnNow then
            landTileCount
        elif landBudget <= 0 then
            0
        else
            GD.PrintErr $"Failed to use up {landBudget} land budget."
            landTileCount - landBudget

    let private isErodible (env: #ITileQuery) (tile: Entity) =
        let erodibleElevation = (tile |> Tile.value |> TileValue.elevation) - 2

        env.GetNeighborTiles tile
        |> Seq.exists (fun neighbor -> neighbor |> Tile.value |> TileValue.elevation <= erodibleElevation)

    let private getErosionTarget (env: 'E when 'E :> ITileQuery and 'E :> IHexMapGeneratorQuery) (tile: Entity) =
        let erodibleElevation = (tile |> Tile.value |> TileValue.elevation) - 2

        let candidates =
            env.GetNeighborTiles tile
            |> Seq.filter (fun neighbor -> neighbor |> Tile.value |> TileValue.elevation <= erodibleElevation)
            |> Seq.toArray

        candidates[env.HexMapGenerator.Rng.RandiRange(0, candidates.Length - 1)]

    let erodingLand (env: 'E when 'E :> ITileQuery and 'E :> IHexMapGeneratorQuery) (landGen: IErosionLandGenerator) =
        let erodibleTiles = env.GetAllTiles() |> Seq.filter (isErodible env) |> ResizeArray

        let targetErodibleCount =
            int
            <| float32 erodibleTiles.Count * float32 (100 - landGen.ErosionPercentage) * 0.01f

        while erodibleTiles.Count > targetErodibleCount do
            let index = env.HexMapGenerator.Rng.RandiRange(0, erodibleTiles.Count - 1)
            let tile = erodibleTiles[index]
            let targetTile = getErosionTarget env tile

            let newTileValue =
                tile
                |> Tile.value
                |> TileValue.withElevation ((tile |> Tile.value |> TileValue.elevation) - 1)

            tile.AddComponent<TileValue>(&newTileValue) |> ignore

            let newTargetValue =
                targetTile
                |> Tile.value
                |> TileValue.withElevation ((targetTile |> Tile.value |> TileValue.elevation) + 1)

            targetTile.AddComponent<TileValue>(&newTargetValue) |> ignore

            if not <| isErodible env tile then
                let lastIndex = erodibleTiles.Count - 1
                erodibleTiles[index] <- erodibleTiles[lastIndex]
                erodibleTiles.RemoveAt lastIndex

            for neighbor in env.GetNeighborTiles tile do
                if
                    neighbor |> Tile.value |> TileValue.elevation = (tile |> Tile.value |> TileValue.elevation) + 2
                    && not <| erodibleTiles.Contains neighbor
                then
                    erodibleTiles.Add neighbor

            if isErodible env targetTile && not <| erodibleTiles.Contains targetTile then
                erodibleTiles.Add targetTile

            for neighbor in env.GetNeighborTiles targetTile do
                // 有一个台阶上去就不是悬崖孤台了
                if
                    neighbor |> Tile.value |> TileValue.elevation = (targetTile |> Tile.value |> TileValue.elevation)
                                                                    + 1
                    && not <| isErodible env neighbor
                then
                    erodibleTiles.Remove neighbor |> ignore
