using System.Collections.Generic;
using Godot;
using ZeromaXPlayground.game.inGame.map.scripts.utils.mySimSql;

namespace ZeromaXPlayground.game.inGame.map.scripts.entity.dataObj;

public class TileInfoDO: BaseDataObj
{
    public Vector2I Coord { get; set; }
    public int Population { get; set; } = 0;
    public List<TileInfoDO> ConnectedTiles { get; set; } = new();
    
    public TileInfoDO(Vector2I coord)
    {
        Coord = coord;
    }
}