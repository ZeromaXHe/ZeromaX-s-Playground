namespace BackEnd4IdleStrategyFS.Game

/// 领域层类型
module DomainT =

    /// 玩家 ID
    type PlayerId = PlayerId of int

    /// 玩家
    type Player = { Id: PlayerId }

    /// 玩家工厂
    type PlayerFactory<'s> = 's -> 's * Player
    /// 按 Id 查询玩家
    type PlayerQueryById<'r> = PlayerId -> 'r -> Player option

    /// 地块 ID
    type TileId = TileId of int

    /// 单位：人
    [<Measure>]
    type Pop

    /// 坐标
    type Coord = int * int

    /// 地块
    type Tile =
        { Id: TileId
          Coord: Coord
          Population: int<Pop>
          PlayerId: PlayerId option }

    /// 地块工厂
    type TileFactory<'s> = Coord -> int<Pop> -> PlayerId option -> 's -> 's * Tile
    /// 保存地块更新
    type TileUpdater<'s> = Tile -> 's -> 's
    /// 按 Id 查询地块
    type TileQueryById<'r> = TileId -> 'r -> Tile option
    /// 按坐标查询地块
    type TileQueryByCoord<'r> = Coord -> 'r -> Tile option
    /// 按玩家查询地块
    type TilesQueryByPlayer<'r> = PlayerId -> 'r -> Tile seq
    /// 查询所有地块
    type TilesQueryAll<'r> = 'r -> Tile seq

    /// 行军部队 ID
    type MarchingArmyId = MarchingArmyId of int

    /// 行军部队
    type MarchingArmy =
        { Id: MarchingArmyId
          Population: int<Pop>
          PlayerId: PlayerId
          FromTileId: TileId
          ToTileId: TileId }

    /// 行军部队工厂
    type MarchingArmyFactory<'s> = int<Pop> -> PlayerId -> TileId -> TileId -> 's -> 's * MarchingArmy
    /// 删除行军部队
    type MarchingArmyDeleter<'s> = MarchingArmyId -> 's -> 's
