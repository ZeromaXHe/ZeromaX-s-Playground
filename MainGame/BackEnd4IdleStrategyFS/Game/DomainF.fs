namespace BackEnd4IdleStrategyFS.Game

/// 领域层逻辑
module private DomainF =
    open System
    open DomainT
    open EventT

    // 增加人口
    let increaseTilePopulation (eventSubject: EventSubject) increment (tile: Tile) =

        eventSubject.tilePopulationChanged.OnNext(
            { id = tile.id
              beforePopulation = tile.population
              afterPopulation = tile.population + increment }
        )

        { tile with
            population = tile.population + increment }

    // 增加玩家领土人口
    let addPopulationToPlayerTiles (eventSubject: EventSubject) (tiles: seq<Tile>) incr =

        tiles
        |> Seq.filter (fun tile -> tile.playerId.IsSome && tile.population < 1000<Pop>)
        |> Seq.map (increaseTilePopulation eventSubject incr)

    /// 占领地块
    let conquerTile (eventSubject: EventSubject) (tile: Tile) (conqueror: Player) =

        eventSubject.tileConquered.OnNext(
            { id = tile.id
              coord = tile.coord
              population = tile.population 
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
        (eventSubject: EventSubject)
        (tiles: seq<Tile>)
        (players: seq<Player>)
        =

        players
        |> Seq.zip tiles
        |> Seq.map (fun (tile, player) ->
            eventSubject.tileConquered.OnNext(
                { id = tile.id
                  coord = tile.coord
                  population = tile.population 
                  conquerorId = player.id
                  loserId = tile.playerId }
            )

            { tile with playerId = Some player.id })