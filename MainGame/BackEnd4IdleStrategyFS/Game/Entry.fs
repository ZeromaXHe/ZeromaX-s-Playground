namespace BackEnd4IdleStrategyFS.Game

open BackEnd4IdleStrategyFS.Game.RepositoryT
open FSharp.Control.Reactive
open FSharpPlus
open FSharpPlus.Data
open System
open System.Reactive.Subjects
open BackEnd4IdleStrategyFS.Common.MonadHelper
open BackEnd4IdleStrategyFS.Game.EventT
open BackEnd4IdleStrategyFS.Game.DomainT
open BackEnd4IdleStrategyFS.Game.Dependency
open BackEnd4IdleStrategyFS.Godot.IAdapter

module Entry =

    type Implement =
        | Original
        | TwoMonad
        | MonadTransformer

    let implement = MonadTransformer

    type Container(aStar: IAStar2D, terrainLayer: ITileMapLayer, playerCount: int) =
        let mutable gameState = emptyGameState

        let random = Random()
        let tileAdded = new Subject<TileAddedEvent>()
        let tileConquered = new Subject<TileConqueredEvent>()
        let tilePopulationChanged = new Subject<TilePopulationChangedEvent>()
        let gameTicked = TimeSpan.FromSeconds 0.5 |> Observable.interval
        let gameFirstArmyGenerated = TimeSpan.FromSeconds 3 |> Observable.timerSpan
        let marchingArmyAdded = new Subject<MarchingArmyAddedEvent>()
        let marchingArmyArrived = new Subject<MarchingArmyArrivedEvent>()

        let onNextWrapper onNext e s =
            onNext e
            s

        let onNextWrapperM onNext e =
            monad {
                let! s = State.get
                onNext e
                do! State.put s
                return ()
            }

        let injector =
            {
              // Godot
              AStar = aStar
              TerrainLayer = terrainLayer
              // 随机
              Random = random
              // 仓储
              PlayerFactory = CommandRepositoryFM.insertPlayer
              PlayerQueryById = QueryRepositoryFM.getPlayer
              TileFactory = CommandRepositoryFM.insertTile
              TileUpdater = CommandRepositoryFM.updateTile
              TileQueryById = QueryRepositoryFM.getTile
              TileQueryByCoord = QueryRepositoryFM.getTileByCoord
              TilesQueryByPlayer = QueryRepositoryFM.getTilesByPlayer
              TilesQueryAll = QueryRepositoryFM.getAllTiles
              MarchingArmyFactory = CommandRepositoryFM.insertMarchingArmy
              MarchingArmyDeleter = CommandRepositoryFM.deleteMarchingArmy
              // 事件
              TileConquered = tileConquered.OnNext
              TilePopulationChanged = tilePopulationChanged.OnNext
              TileAdded = tileAdded.OnNext
              MarchingArmyAdded = marchingArmyAdded.OnNext
              MarchingArmyArrived = marchingArmyArrived.OnNext }

        let tileAddedSubscription =
            tileAdded
            |> Observable.subscribe (fun e ->
                let (TileId tileId) = e.TileId
                aStar.AddPoint tileId e.Coord)

        let marchingArmyAddedSubscription =
            marchingArmyAdded
            |> Observable.delayMap (fun e ->
                // 计算延迟到达时间
                let speed = DomainF.marchSpeed e.Population
                let delay = 100.0 / float speed
                TimeSpan.FromSeconds delay |> Observable.timerSpan)
            |> Observable.subscribe (fun e ->
                // 转发抵达事件
                marchingArmyArrived.OnNext
                    { MarchingArmyId = e.MarchingArmyId
                      Population = e.Population
                      DestinationTileId = e.ToTileId
                      PlayerId = e.PlayerId })

        let marchingArmyArrivedSubscription =
            let marchingArmyArrivedOnNext_Original e =
                let marchingArmy = QueryRepositoryF.getMarchingArmy e.MarchingArmyId gameState
                // 处理抵达部队
                if marchingArmy.IsSome then
                    gameState <-
                        DomainF.arriveArmy
                            QueryRepositoryF.getTile
                            QueryRepositoryF.getPlayer
                            CommandRepositoryF.deleteMarchingArmy
                            CommandRepositoryF.updateTile
                            (onNextWrapper tileConquered.OnNext)
                            marchingArmy.Value
                            gameState

                let player = QueryRepositoryF.getPlayer e.PlayerId gameState
                // 派出新的部队
                if player.IsSome then
                    let gameState', _ =
                        DomainF.marchArmy
                            CommandRepositoryF.insertMarchingArmy
                            (onNextWrapper marchingArmyAdded.OnNext)
                            QueryRepositoryF.getTilesByPlayer
                            CommandRepositoryF.updateTile
                            aStar
                            random
                            player.Value
                            gameState

                    gameState <- gameState'

            let marchingArmyArrivedOnNext_2Monad e =
                let arrive =
                    monad {
                        // 处理抵达部队
                        let! marchingArmy = QueryRepositoryFM.getMarchingArmy e.MarchingArmyId |> OptionT

                        return! DomainFMM.arriveArmy marchingArmy |> Reader.run <| injector |> OptionT
                    }

                let send =
                    monad {
                        // 派出新的部队
                        let! player = QueryRepositoryFM.getPlayer e.PlayerId |> OptionT

                        return! DomainFMM.marchArmy player |> Reader.run <| injector |> OptionT.lift
                    }

                let boolOpt, gameState' = arrive |> OptionT.run |> State.run <| gameState

                if boolOpt.IsSome && boolOpt.Value then
                    gameState <- gameState'

                let armyOpt, gameState' = send |> OptionT.run |> State.run <| gameState

                if armyOpt.IsSome then
                    gameState <- gameState'

            let marchingArmyArrivedOnNext_MonadTransformer e =
                let arrive =
                    monad {
                        // 处理抵达部队
                        let! marchingArmy =
                            QueryRepositoryFM.getMarchingArmy e.MarchingArmyId |> StateT.hoist |> OptionT

                        return! DomainFM.arriveArmy marchingArmy
                    }

                let send =
                    monad {
                        // 派出新的部队
                        let! player = QueryRepositoryFM.getPlayer e.PlayerId |> StateT.hoist |> OptionT

                        return! DomainFM.marchArmy player |> OptionT.lift
                    }

                let boolOpt, gameState' =
                    arrive |> OptionT.run |> StateT.run <| gameState |> Reader.run <| injector

                if boolOpt.IsSome && boolOpt.Value then
                    gameState <- gameState'

                let armyOpt, gameState' =
                    send |> OptionT.run |> StateT.run <| gameState |> Reader.run <| injector

                if armyOpt.IsSome then
                    gameState <- gameState'

            marchingArmyArrived
            |> Observable.subscribe (
                match implement with
                | Original -> marchingArmyArrivedOnNext_Original
                | TwoMonad -> marchingArmyArrivedOnNext_2Monad
                | MonadTransformer -> marchingArmyArrivedOnNext_MonadTransformer
            )

        let gameTickedSubscription =
            let gameTickedOnNext_Original _ =
                gameState <-
                    DomainF.increaseAllPlayerTilesPopulation
                        QueryRepositoryF.getAllTiles
                        CommandRepositoryF.updateTile
                        (onNextWrapper tilePopulationChanged.OnNext)
                        1<Pop>
                        gameState

            let gameTickedOnNext_2Monad _ =
                let _, gameState' =
                    DomainFMM.increaseAllPlayerTilesPopulation 1<Pop> |> Reader.run <| injector
                    |> State.run
                    <| gameState

                gameState <- gameState'

            let gameTickedOnNext_MonadTransformer _ =
                let _, gameState' =
                    DomainFM.increaseAllPlayerTilesPopulation 1<Pop> |> StateT.run <| gameState
                    |> Reader.run
                    <| injector

                gameState <- gameState'

            gameTicked
            |> Observable.subscribe (
                match implement with
                | Original -> gameTickedOnNext_Original
                | TwoMonad -> gameTickedOnNext_2Monad
                | MonadTransformer -> gameTickedOnNext_MonadTransformer
            )

        let gameFirstArmyGeneratedSubscription =
            let gameFirstArmyGeneratedOnNext_Original _ =
                gameState <-
                    QueryRepositoryF.getAllPlayers gameState
                    |> Seq.fold
                        (fun s p ->
                            let s', _ =
                                DomainF.marchArmy
                                    CommandRepositoryF.insertMarchingArmy
                                    (onNextWrapper marchingArmyAdded.OnNext)
                                    QueryRepositoryF.getTilesByPlayer
                                    CommandRepositoryF.updateTile
                                    aStar
                                    random
                                    p
                                    s

                            s')
                        gameState

            let gameFirstArmyGeneratedOnNext_2Monad _ =
                let playerSeq, _ = State.run QueryRepositoryFM.getAllPlayers gameState

                playerSeq
                |> Seq.iter (fun p ->
                    let _, s = DomainFMM.marchArmy p |> Reader.run <| injector |> State.run <| gameState

                    gameState <- s)

            let gameFirstArmyGeneratedOnNext_MonadTransformer _ =
                let playerSeq, _ = State.run QueryRepositoryFM.getAllPlayers gameState

                playerSeq
                |> Seq.iter (fun p ->
                    let _, s = DomainFM.marchArmy p |> StateT.run <| gameState |> Reader.run <| injector

                    gameState <- s)

            gameFirstArmyGenerated
            |> Observable.subscribe (
                match implement with
                | Original -> gameFirstArmyGeneratedOnNext_Original
                | TwoMonad -> gameFirstArmyGeneratedOnNext_2Monad
                | MonadTransformer -> gameFirstArmyGeneratedOnNext_MonadTransformer
            )

        member this.GameTicked = gameTicked |> Observable.asObservable

        member this.GameFirstArmyGenerated = gameFirstArmyGenerated |> Observable.asObservable

        member this.TileAdded = tileAdded |> Observable.asObservable

        member this.TileConquered = tileConquered |> Observable.asObservable

        member this.TilePopulationChanged = tilePopulationChanged |> Observable.asObservable

        member this.MarchingArmyAdded = marchingArmyAdded |> Observable.asObservable

        member this.MarchingArmyArrived = marchingArmyArrived |> Observable.asObservable

        member this.QueryAllPlayers() = AppService.queryAllPlayers gameState

        member this.QueryTileById tileIdInt =
            AppService.queryTileById gameState tileIdInt

        member this.QueryTilesByPlayerId playerIdInt =
            AppService.queryTilesByPlayerId gameState playerIdInt

        member this.Init() =
            match implement with
            | Original -> this.Init_Original()
            | TwoMonad -> this.Init_2Monad()
            | MonadTransformer -> this.Init_MonadTransformer()

        member private this.Init_Original() =
            let usedCells = terrainLayer.GetUsedCells()

            let combineF =
                DomainF.createTiles
                    (fun c _ _ -> CommandRepositoryF.insertTile c)
                    (onNextWrapper tileAdded.OnNext)
                    usedCells
                >> (DomainF.initTilesConnections QueryRepositoryF.getTileByCoord terrainLayer aStar)
                >> (DomainF.spawnPlayers
                        QueryRepositoryF.getAllTiles
                        CommandRepositoryF.updateTile
                        CommandRepositoryF.insertPlayer
                        (onNextWrapper tileConquered.OnNext)
                        random
                        playerCount)

            do gameState <- combineF gameState

        member private this.Init_2Monad() =
            let usedCells = terrainLayer.GetUsedCells()

            let _, gameState' =
                DomainFMM.createTiles usedCells |> Reader.run <| injector |> State.run
                <| gameState

            gameState <- gameState'

            let gameState' =
                DomainFMM.initTilesConnections |> Reader.run <| injector
                |> Seq.fold
                    (fun s t ->
                        let _, s' = t |> State.run <| s
                        s')
                    gameState

            gameState <- gameState'

            let _, gameState' =
                DomainFMM.spawnPlayers playerCount |> Reader.run <| injector |> State.run
                <| gameState

            gameState <- gameState'

        member private this.Init_MonadTransformer() =
            let usedCells = terrainLayer.GetUsedCells()

            let _, gameState' =
                DomainFM.createTiles usedCells |> StateT.run <| gameState |> Reader.run
                <| injector

            gameState <- gameState'

            let _, gameState' =
                DomainFM.initTilesConnections |> StateT.run <| gameState |> Reader.run
                <| injector

            gameState <- gameState'

            let _, gameState' =
                DomainFM.spawnPlayers playerCount |> StateT.run <| gameState |> Reader.run
                <| injector

            gameState <- gameState'
