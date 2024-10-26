namespace BackEnd4IdleStrategyFS.Game

/// 领域层类型
module DomainT =

    /// 玩家 ID
    type PlayerId = PlayerId of int

    /// 玩家
    type Player = { id: PlayerId }

    /// 单位：人
    [<Measure>]
    type Pop

    /// 地块 ID
    type TileId = TileId of int

    /// 地块
    type Tile =
        { id: TileId
          coord: int * int
          population: int<Pop>
          playerId: PlayerId option }

    /// 行军部队 ID
    type MarchingArmyId = MarchingArmyId of int

    /// 行军部队
    type MarchingArmy =
        { id: MarchingArmyId
          population: int<Pop>
          playerId: PlayerId
          fromTileId: TileId
          toTileId: TileId }