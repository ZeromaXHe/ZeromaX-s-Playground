using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;
using ZeromaXPlayground.game.inGame.map.scripts.service;

namespace ZeromaXPlayground.game.inGame.map.scripts.domain;

public partial class TileInfo : GodotObject
{
    private static int _nextId = 1;
    private static readonly Dictionary<int, TileInfo> IdMap = new();
    private static readonly Dictionary<Vector2I, TileInfo> CoordMap = new();
    private static readonly Dictionary<int, List<TileInfo>> PlayerIdMap = new();

    public int Id { get; }

    /**
     * 坐标
     */
    public readonly Vector2I Coord;

    /**
     * 人口
     */
    public int Population
    {
        get => _population;
        set
        {
            _population = value;
            EventBus.Instance.EmitSignal(EventBus.SignalName.TilePopulationChanged, Id);
        }
    }

    private int _population;

    /**
     * 所属的玩家
     */
    public int PlayerId
    {
        get => _playerId;
        private set
        {
            if (_playerId != Constants.NullId)
            {
                PlayerIdMap[_playerId].Remove(this);
                if (PlayerIdMap[_playerId].Count == 0)
                {
                    PlayerIdMap.Remove(_playerId);
                }
            }

            _playerId = value;
            if (_playerId != Constants.NullId)
            {
                if (PlayerIdMap.TryGetValue(_playerId, out var getVal))
                {
                    getVal.Add(this);
                }
                else
                {
                    PlayerIdMap.Add(_playerId, new List<TileInfo> { this });
                }
            }
        }
    }

    private int _playerId = Constants.NullId;

    private TileInfo(Vector2I coord)
    {
        Id = _nextId++;
        Coord = coord;
        NavigationService.Instance.AddPoint(Id, coord);
        IdMap.Add(Id, this); // Add 在 Id 重复的时候会报错，符合我们的需要；索引方式添加会直接覆盖原值
        CoordMap.Add(Coord, this);
    }

    public static void InitMapTile(TileMapLayer baseTerrain)
    {
        // 根据 BaseTerrain 层的内容预生成地图
        var usedCells = baseTerrain.GetUsedCells();
        foreach (var cell in usedCells)
        {
            var cellTileInfo = new TileInfo(cell);
            // 完成连接图
            var surroundings = baseTerrain.GetSurroundingCells(cell);
            foreach (var surrounding in surroundings)
            {
                GetByCoord(surrounding)?.ConnectTile(cellTileInfo.Id);
            }
        }
    }

    public static void AllPlayerTilesAddPopulation(int incr)
    {
        // 目前超过 1000 人口的地块不再增长
        foreach (var tile in IdMap.Values
                     .Where(tile => tile._playerId != Constants.NullId && tile.Population < 1000))
        {
            tile.Population += incr;
        }
    }

    private void ConnectTile(int tileInfoId)
    {
        NavigationService.Instance.ConnectPoints(tileInfoId, Id);
    }

    public void ConqueredBy(int conquerorId)
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.TileConquered, Id, conquerorId, PlayerId);
        PlayerId = conquerorId;
    }

    #region 查询方法

    public static TileInfo GetById(int id)
    {
        return IdMap.TryGetValue(id, out var result) ? result : null;
    }

    public static TileInfo GetByCoord(Vector2I coord)
    {
        return CoordMap.TryGetValue(coord, out var result) ? result : null;
    }

    public static List<TileInfo> GetByPlayerId(int playerId)
    {
        return PlayerIdMap.TryGetValue(playerId, out var result) ? result : null;
    }

    #endregion
}