namespace BackEnd4IdleStrategyFS.Game

/// 数据存储库类型
module RepositoryT =
    open DomainT

    /// 游戏状态
    type GameState =
        { PlayerRepo: Map<PlayerId, Player>
          PlayerNextId: int
          TileRepo: Map<TileId, Tile>
          TileCoordIndex: Map<int * int, TileId>
          TilePlayerIndex: Map<PlayerId, TileId list>
          TileNextId: int
          MarchingArmyRepo: Map<MarchingArmyId, MarchingArmy>
          MarchingArmyNextId: int }

    let emptyGameState =
        { PlayerRepo = Map.empty
          PlayerNextId = 1
          TileRepo = Map.empty
          TileCoordIndex = Map.empty
          TilePlayerIndex = Map.empty
          TileNextId = 1
          MarchingArmyRepo = Map.empty
          MarchingArmyNextId = 1 }

    /// 玩家 id、所有地块数、人口总数统计
    type PlayerStat =
        { Territory: int
          TilePopulation: int<Pop>
          ArmyPopulation: int<Pop> }

    /// 游戏统计
    type GameStat =
        { PlayerStat: Map<PlayerId, PlayerStat> }

    let emptyGameStat = { PlayerStat = Map.empty }
