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