namespace BackEnd4IdleStrategyFS.Game

/// 领域层逻辑
module private DomainF =
    open System
    open System.Reactive.Subjects
    open DomainT
    open EventT

    // 增加人口
    let increaseTilePopulation
        (tilePopulationChangedEventSubject: Subject<TilePopulationChangedEvent>)
        increment
        (tile: Tile)
        =

        tilePopulationChangedEventSubject.OnNext(
            { id = tile.id
              beforePopulation = tile.population
              afterPopulation = tile.population + increment }
        )

        { tile with
            population = tile.population + increment }

    // 增加玩家领土人口
    let addPopulationToPlayerTiles
        (tilePopulationChangedEventSubject: Subject<TilePopulationChangedEvent>)
        (tiles: seq<Tile>)
        incr
        =

        tiles
        |> Seq.filter (fun tile -> tile.playerId.IsSome && tile.population < 1000<Pop>)
        |> Seq.map (increaseTilePopulation tilePopulationChangedEventSubject incr)

    /// 占领地块
    let conquerTile (tileConqueredEventSubject: Subject<TileConqueredEvent>) (tile: Tile) (conqueror: Player) =

        tileConqueredEventSubject.OnNext(
            { id = tile.id
              conquerorId = conqueror.id
              loserId = tile.playerId }
        )

        { tile with
            playerId = Some conqueror.id }

    /// 出兵
    let marchArmy (fromTile: Tile) (toTile: Tile) =
        match fromTile.playerId with
        | Some playerId when fromTile.population > 0<Pop> ->
            // TODO：副作用
            let population = Random().Next(1, fromTile.population / 1<Pop>) * 1<Pop>
            let playerId = playerId
            let fromTileId = fromTile.id
            let toTileId = toTile.id
            playerId, population, fromTileId, toTileId
        | None -> failwith "按道理不应该有这种从不是自己领土出兵的情况"
        | Some value -> failwith $"出兵 fromFile {fromTile.id} 人口为零"

    /// 将玩家分配在地块上
    let playersFirstConquerTiles
        (tileConqueredEventSubject: Subject<TileConqueredEvent>)
        (tiles: seq<Tile>)
        (players: seq<Player>)
        =

        players
        |> Seq.zip tiles
        |> Seq.map (fun (tile, player) ->
            tileConqueredEventSubject.OnNext(
                { id = tile.id
                  conquerorId = player.id
                  loserId = tile.playerId }
            )

            { tile with playerId = Some player.id })
