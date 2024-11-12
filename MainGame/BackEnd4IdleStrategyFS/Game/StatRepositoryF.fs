namespace BackEnd4IdleStrategyFS.Game

open FSharpPlus
open FSharpPlus.Data
open DomainT
open EventT
open RepositoryT

module StatRepositoryF =
    /// 根据玩家新建事件，初始化玩家统计
    let updatePlayerStatByPlayerAddedEvent (e: PlayerAddedEvent) =
        monad {
            let! gameStat = State.get

            let playerStat =
                { Territory = 0
                  TilePopulation = 0<Pop>
                  ArmyPopulation = 0<Pop> }

            let gameStat' =
                { gameStat with
                    PlayerStat = gameStat.PlayerStat.Add(e.PlayerId, playerStat) }

            do! State.put gameStat'
        }

    /// 根据地块占领事件，增量更新玩家统计
    let updatePlayerStatByTileConqueredEvent (e: TileConqueredEvent) =
        monad {
            if e.ConquerorId.IsSome then
                let! gameStat = State.get
                let conquerorStat = gameStat.PlayerStat[e.ConquerorId.Value]

                let conquerorStat' =
                    { conquerorStat with
                        Territory = conquerorStat.Territory + 1
                        TilePopulation = conquerorStat.TilePopulation + e.AfterPopulation }

                let gameStat' =
                    { gameStat with
                        PlayerStat = gameStat.PlayerStat.Add(e.ConquerorId.Value, conquerorStat') }

                do! State.put gameStat'

            if e.LoserId.IsSome then
                let! gameStat = State.get
                let loserStat = gameStat.PlayerStat[e.LoserId.Value]

                let loserStat' =
                    { loserStat with
                        Territory = loserStat.Territory - 1
                        TilePopulation = loserStat.TilePopulation - e.BeforePopulation }

                let gameStat' =
                    { gameStat with
                        PlayerStat = gameStat.PlayerStat.Add(e.LoserId.Value, loserStat') }

                do! State.put gameStat'
        }

    /// 根据地块人口增加事件，增量更新玩家统计
    let updatePlayerStatByTilePopulationChangedEvent (e: TilePopulationChangedEvent) =
        monad {
            if e.PlayerId.IsSome then
                let! gameStat = State.get
                let playerStat = gameStat.PlayerStat[e.PlayerId.Value]

                let playerStat' =
                    { playerStat with
                        TilePopulation = playerStat.TilePopulation + e.AfterPopulation - e.BeforePopulation }

                let gameStat' =
                    { gameStat with
                        PlayerStat = gameStat.PlayerStat.Add(e.PlayerId.Value, playerStat') }

                do! State.put gameStat'
        }

    /// 根据部队新建事件，增量更新玩家统计
    let updatePlayerStatByMarchingArmyAddedEvent (e: MarchingArmyAddedEvent) =
        monad {
            let! gameStat = State.get
            let playerStat = gameStat.PlayerStat[e.PlayerId]

            let playerStat' =
                { playerStat with
                    TilePopulation = playerStat.TilePopulation - e.Population
                    ArmyPopulation = playerStat.ArmyPopulation + e.Population }

            let gameStat' =
                { gameStat with
                    PlayerStat = gameStat.PlayerStat.Add(e.PlayerId, playerStat') }

            do! State.put gameStat'
        }

    /// 根据部队抵达事件，增量更新玩家统计
    let updatePlayerStatByMarchingArmyArrivedEvent (e: MarchingArmyArrivedEvent) =
        monad {
            let! gameStat = State.get
            let playerStat = gameStat.PlayerStat[e.PlayerId]

            let playerStat' =
                { playerStat with
                    ArmyPopulation = playerStat.ArmyPopulation - e.Population }

            let gameStat' =
                { gameStat with
                    PlayerStat = gameStat.PlayerStat.Add(e.PlayerId, playerStat') }

            do! State.put gameStat'
        }
