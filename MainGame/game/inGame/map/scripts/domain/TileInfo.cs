using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;
using ZeromaXPlayground.game.inGame.map.scripts.service;

namespace ZeromaXPlayground.game.inGame.map.scripts.domain;

public partial class TileInfo: GodotObject
{
    /**
     * 地块人口增加事件
     */
    [Signal]
    public delegate void PopulationChangedEventHandler(int population);

    private static int _nextId = 1;
    private static readonly Dictionary<int, TileInfo> IdMap = new();
    private static readonly Dictionary<Vector2I, TileInfo> CoordMap = new();

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
        private set
        {
            EmitSignal(SignalName.PopulationChanged, value);
            _population = value;
        }
    }

    private int _population;

    /**
     * 所属的玩家
     */
    private int _playerId = Constants.NullId;

    public TileInfo(Vector2I coord)
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
        foreach (var tile in IdMap.Values.Where(tile => tile._playerId != Constants.NullId))
        {
            tile.Population += incr;
        }
    }

    public void ConnectTile(int tileInfoId)
    {
        NavigationService.Instance.ConnectPoints(tileInfoId, Id);
    }

    public void ConqueredBy(int conquerorId)
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.TileConquered, conquerorId, _playerId, Coord);
        _playerId = conquerorId;
    }

    #region 查询方法

    public static TileInfo GetById(int id)
    {
        if (IdMap.TryGetValue(id, out var result))
        {
            return result;
        }

        return null;
    }

    public static TileInfo GetByCoord(Vector2I coord)
    {
        if (CoordMap.TryGetValue(coord, out var result))
        {
            return result;
        }

        return null;
    }

    #endregion
}