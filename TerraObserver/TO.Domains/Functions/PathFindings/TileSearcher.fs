namespace TO.Domains.Functions.PathFindings

open Friflo.Engine.ECS
open TO.Domains.Functions.HexGridCoords
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexMetrics
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.PathFindings

module TileSearcherQuery =
    let getMoveCost: GetMoveCost =
        fun (fromTile: Entity) (toTile: Entity) ->
            let edgeType =
                fromTile |> Tile.value |> TileValue.getEdgeType (toTile |> Tile.value)

            if edgeType = HexEdgeType.Cliff then
                -1
            elif
                fromTile
                |> Tile.flag
                |> TileFlag.hasRoad (Tile.getNeighborTileIdx fromTile toTile)
            then
                1
            elif
                fromTile |> Tile.flag |> TileFlag.walled
                <> (toTile |> Tile.flag |> TileFlag.walled)
            then
                -1
            else
                let toTileValue = toTile |> Tile.value

                (if edgeType = HexEdgeType.Flat then 5 else 10)
                + (toTileValue |> TileValue.urbanLevel)
                + (toTileValue |> TileValue.farmLevel)
                + (toTileValue |> TileValue.plantLevel)

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 09:16:25
module TileSearcherCommand =
    let initSearchData (env: 'E when 'E :> IEntityStoreQuery and 'E :> ITileSearcherQuery) : InitSearchData =
        fun () ->
            let this = env.TileSearcher
            this.SearchData <- Array.zeroCreate <| env.Query<TileUnitCentroid>().Count + 1
            this.SearchFrontier <- None

    let refreshTileSearchData (env: #ITileSearcherQuery) : RefreshTileSearchData =
        fun tileCountId -> env.TileSearcher.SearchData[tileCountId.CountId].SearchPhase <- 0

    let private isValidDestination (tile: Entity) =
        tile |> Tile.flag |> TileFlag.isExplored
        && tile |> Tile.value |> TileValue.isUnderwater // && !tile.HasUnit

    let private heuristicCost (env: #ITileQuery) (fromTile: Entity) (toTile: Entity) =
        env.GetSphereAxial fromTile
        |> SphereAxial.distanceTo (env.GetSphereAxial toTile)
    // 寻路
    let private searchPath
        (env: 'E when 'E :> ITileSearcherQuery and 'E :> ITileQuery)
        (fromTile: Entity)
        (toTile: Entity)
        =
        let this = env.TileSearcher
        this.SearchFrontierPhase <- this.SearchFrontierPhase + 2

        if this.SearchFrontier.IsNone then
            this.SearchFrontier <- this.SearchData |> TilePriorityQueue |> Some

        let searchFrontier = this.SearchFrontier.Value
        searchFrontier |> TilePriorityQueue.clear
        let fromTileCountId = fromTile |> Tile.countId |> _.CountId
        this.SearchData[fromTileCountId] <- TileSearchData(SearchPhase = this.SearchFrontierPhase)
        searchFrontier |> TilePriorityQueue.enqueue fromTileCountId
        let mutable currentCountId = -1
        let mutable pathFound = false

        while not pathFound && TilePriorityQueue.tryDequeue &currentCountId searchFrontier do
            let current = env.GetTileByCountId currentCountId
            let currentDistance = this.SearchData[currentCountId].Distance
            this.SearchData[currentCountId].SearchPhase <- this.SearchData[currentCountId].SearchPhase + 1

            if current.Id = toTile.Id then
                pathFound <- true
            else
                for neighbor in env.GetNeighborTiles current do
                    let neighborCountId = neighbor |> Tile.countId |> _.CountId
                    let neighborData = this.SearchData[neighborCountId]

                    if
                        neighborData.SearchPhase > this.SearchFrontierPhase
                        || not <| isValidDestination neighbor
                    then
                        () // continue
                    else
                        let moveCost = env.GetMoveCost current neighbor

                        if moveCost < 0 then
                            ()
                        else
                            let distance = currentDistance + moveCost

                            if neighborData.SearchPhase < this.SearchFrontierPhase then
                                this.SearchData[neighborCountId] <-
                                    TileSearchData(
                                        Distance = distance,
                                        PathFrom = current.Id,
                                        Heuristic = heuristicCost env neighbor toTile,
                                        SearchPhase = this.SearchFrontierPhase
                                    )

                                searchFrontier |> TilePriorityQueue.enqueue neighborCountId
                            elif distance < neighborData.Distance then
                                this.SearchData[neighborCountId].Distance <- distance
                                this.SearchData[neighborCountId].PathFrom <- current.Id

                                searchFrontier
                                |> TilePriorityQueue.change neighborCountId neighborData.SearchPriority

        pathFound

    // 可视范围
    let getVisibleTiles (env: 'E when 'E :> ITileSearcherQuery and 'E :> ITileQuery) =
        fun (fromTile: Entity) (range: int) ->
            let this = env.TileSearcher
            let visibleTiles = ResizeArray<Entity>()
            this.SearchFrontierPhase <- this.SearchFrontierPhase + 2

            if this.SearchFrontier.IsNone then
                this.SearchFrontier <- this.SearchData |> TilePriorityQueue |> Some

            let searchFrontier = this.SearchFrontier.Value
            searchFrontier |> TilePriorityQueue.clear
            let fromTileCountId = fromTile |> Tile.countId |> _.CountId

            this.SearchData[fromTileCountId] <-
                TileSearchData(
                    SearchPhase = this.SearchFrontierPhase,
                    PathFrom = this.SearchData[fromTileCountId].PathFrom
                )

            searchFrontier |> TilePriorityQueue.enqueue fromTileCountId
            let fromTileSa = env.GetSphereAxial fromTile
            let mutable currentCountId = -1

            while TilePriorityQueue.tryDequeue &currentCountId searchFrontier do
                let current = env.GetTileByCountId currentCountId
                this.SearchData[currentCountId].SearchPhase <- this.SearchData[currentCountId].SearchPhase + 1
                visibleTiles.Add current

                for neighbor in env.GetNeighborTiles current do
                    let neighborCountId = neighbor |> Tile.countId |> _.CountId
                    let neighborData = this.SearchData[neighborCountId]

                    if
                        neighborData.SearchPhase > this.SearchFrontierPhase
                        || neighbor |> Tile.flag |> TileFlag.isExplorable |> not
                    then
                        () // continue
                    else
                        let distance = this.SearchData[currentCountId].Distance + 1

                        if
                            distance + (neighbor |> Tile.value |> TileValue.viewElevation) > range
                            || distance > (fromTileSa |> SphereAxial.distanceTo <| env.GetSphereAxial neighbor)
                        then
                            ()
                        elif neighborData.SearchPhase < this.SearchFrontierPhase then
                            this.SearchData[neighborCountId] <-
                                TileSearchData(
                                    Distance = distance,
                                    PathFrom = neighborData.PathFrom,
                                    SearchPhase = this.SearchFrontierPhase
                                )

                            searchFrontier |> TilePriorityQueue.enqueue neighborCountId
                        elif distance < this.SearchData[neighborCountId].Distance then
                            this.SearchData[neighborCountId].Distance <- distance

                            searchFrontier
                            |> TilePriorityQueue.change neighborCountId neighborData.SearchPriority

            visibleTiles

    let findPath (env: 'E when 'E :> ITileSearcherQuery and 'E :> ITileQuery) =
        fun (fromTile: Entity) (toTile: Entity) (useCache: bool) ->
            let this = env.TileSearcher

            if
                not useCache
                || fromTile.Id <> this.CurrentPathFromId
                || toTile.Id <> this.CurrentPathToId
            then
                this.HasPath <- isValidDestination toTile && searchPath env fromTile toTile

            this.CurrentPathFromId <- fromTile.Id
            this.CurrentPathToId <- toTile.Id

            if not this.HasPath then
                ResizeArray<Entity>()
            else
                let mutable currentId = this.CurrentPathToId
                let res = ResizeArray<Entity>()

                while currentId <> this.CurrentPathFromId do
                    let current = env.GetTile currentId
                    res.Add <| current
                    let currentCountId = current |> Tile.countId |> _.CountId
                    currentId <- this.SearchData[currentCountId].PathFrom

                res.Add fromTile
                res.Reverse()
                res
