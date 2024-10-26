namespace BackEnd4IdleStrategyFS.Game

/// 数据存储库类型
module RepositoryT =
    open DomainT

    /// 游戏状态
    type GameState =
        { playerRepo: Map<PlayerId, Player>
          playerNextId: int
          tileRepo: Map<TileId, Tile>
          tileCoordIndex: Map<int * int, TileId>
          tilePlayerIndex: Map<PlayerId, TileId list>
          tileNextId: int
          marchingArmyRepo: Map<MarchingArmyId, MarchingArmy>
          marchingArmyNextId: int }
