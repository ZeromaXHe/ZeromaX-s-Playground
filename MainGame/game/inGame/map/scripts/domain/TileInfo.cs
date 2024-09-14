using System.Collections.Generic;
using Godot;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;
using ZeromaXPlayground.game.inGame.map.scripts.service;

namespace ZeromaXPlayground.game.inGame.map.scripts.domain;

public class TileInfo
{
    private static int _nextId = 1;
    private static readonly Dictionary<int, TileInfo> IdMap = new();
    private static readonly Dictionary<Vector2I, TileInfo> CoordMap = new();

    public int Id { get; }

    /**
     * 坐标
     */
    private readonly Vector2I _coord;

    /**
     * 人口
     */
    private int _population = 0;

    /**
     * 所属的玩家
     */
    private int _playerId = Constants.NullId;

    public TileInfo(Vector2I coord)
    {
        Id = _nextId++;
        _coord = coord;
        NavigationService.Instance.AddPoint(Id, coord);
        IdMap.Add(Id, this); // Add 在 Id 重复的时候会报错，符合我们的需要；索引方式添加会直接覆盖原值
        CoordMap.Add(_coord, this);
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

    public void ConnectTile(int tileInfoId)
    {
        NavigationService.Instance.ConnectPoints(tileInfoId, Id);
    }

    public void ConqueredBy(int conquerorId)
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.TerritoryConquered, conquerorId, _playerId, _coord);
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