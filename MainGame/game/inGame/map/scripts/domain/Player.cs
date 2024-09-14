using System.Collections.Generic;
using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts.domain;

public class Player
{
    private static int _nextId = 1;
    private static readonly Dictionary<int, Player> IdMap = new();
    public int Id { get; }

    public Player()
    {
        Id = _nextId++;
        IdMap.Add(Id, this); // Add 在 Id 重复的时候会报错，符合我们的需要；索引方式添加会直接覆盖原值
    }
    
    public static void InitAndSpawnOnTile(TileMapLayer baseTerrain, int playerCount)
    {
        var usedCells = baseTerrain.GetUsedCells();
        usedCells.Shuffle();
        for (int i = 0; i < playerCount; i++)
        {
            var player = new Player();
            TileInfo.GetByCoord(usedCells[i]).ConqueredBy(player.Id);
        }
    }

    #region 查询方法

    public static Player GetById(int id)
    {
        if (IdMap.TryGetValue(id, out var result))
        {
            return result;
        }

        return null;
    }

    #endregion
}