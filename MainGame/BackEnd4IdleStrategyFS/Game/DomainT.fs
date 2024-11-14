namespace BackEnd4IdleStrategyFS.Game

open FSharpPlus.Data

/// 领域层类型
module DomainT =
    /// 查询游戏速度
    type SpeedMultiplierQuery<'r> = unit -> State<'r, float>

    /// 玩家 ID
    type PlayerId = PlayerId of int

    /// 玩家
    type Player = { Id: PlayerId }

    /// 玩家工厂
    type PlayerFactory<'s> = State<'s, Player>
    /// 按 Id 查询玩家
    type PlayerQueryById<'r> = PlayerId -> State<'r, Player option>
    /// 查询所有玩家
    type PlayersQueryAll<'r> = State<'r, Player seq>

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
    type TileFactory<'s> = Coord -> int<Pop> -> PlayerId option -> State<'s, Tile>
    /// 保存地块更新
    type TileUpdater<'s> = Tile -> State<'s, Tile>
    /// 按 Id 查询地块
    type TileQueryById<'r> = TileId -> State<'r, Tile option>
    /// 按坐标查询地块
    type TileQueryByCoord<'r> = Coord -> State<'r, Tile option>
    /// 按玩家查询地块
    type TilesQueryByPlayer<'r> = PlayerId -> State<'r, Tile seq>
    /// 查询所有地块
    type TilesQueryAll<'r> = State<'r, Tile seq>

    /// 行军部队 ID
    type MarchingArmyId = MarchingArmyId of int

    /// 行军部队
    type MarchingArmy =
        { Id: MarchingArmyId
          Population: int<Pop>
          PlayerId: PlayerId
          FromTileId: TileId
          ToTileId: TileId
          Progress: float }

    /// 行军部队工厂
    type MarchingArmyFactory<'s> = int<Pop> -> PlayerId -> TileId -> TileId -> State<'s, MarchingArmy>
    /// 保存行军部队更新
    type MarchingArmyUpdater<'s> = MarchingArmy -> State<'s, MarchingArmy>
    /// 删除行军部队
    type MarchingArmyDeleter<'s> = MarchingArmyId -> State<'s, bool>
    /// 按 Id 查询部队
    type MarchingArmyQueryById<'r> = MarchingArmyId -> State<'r, MarchingArmy option>
    /// 查询所有部队
    type MarchingArmiesQueryAll<'r> = State<'r, MarchingArmy seq>
