using System.Collections.Generic;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;

namespace ZeromaXPlayground.game.inGame.map.scripts.domain;

public class MarchingArmy
{
    private static int _nextId = 1;
    private static readonly Dictionary<int, MarchingArmy> IdMap = new();

    public int Id { get; }

    public int Population { get; }
    
    public int PlayerId { get; }

    public int FromTileId { get; }

    public int ToTileId { get; }

    public MarchingArmy(int population, int playerId, int fromTileId, int toTileId)
    {
        Id = _nextId++;
        IdMap.Add(Id, this); // Add 在 Id 重复的时候会报错，符合我们的需要；索引方式添加会直接覆盖原值
        Population = population;
        PlayerId = playerId;
        FromTileId = fromTileId;
        ToTileId = toTileId;

        TileInfo.GetById(FromTileId).Population -= population;
    }

    public void ArriveDestination()
    {
        var destinationTile = TileInfo.GetById(ToTileId);
        if (destinationTile.PlayerId == Constants.NullId)
        {
            destinationTile.ConqueredBy(PlayerId);
        }
        if (PlayerId == destinationTile.PlayerId)
        {
            // 自己领土上移动部队
            destinationTile.Population += Population;
        }
        else if (destinationTile.Population >= Population)
        {
            destinationTile.Population -= Population;
        }
        else
        {
            destinationTile.ConqueredBy(PlayerId);
            destinationTile.Population = Population - destinationTile.Population;
        }
        // 清除掉本条数据，避免内存泄露
        IdMap.Remove(Id);
    }

    #region 查询条件

    public static MarchingArmy GetById(int id)
    {
        return IdMap[id];
    }

    #endregion
}