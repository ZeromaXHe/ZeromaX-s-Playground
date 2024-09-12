using System.Collections.Generic;

namespace ZeromaXPlayground.game.inGame.map.scripts;

public class TileInfo
{
    private int _x;
    private int _y;
    private int _aStarId;
    private int _population = 0;
    private List<TileInfo> _connectedTiles = new();

    public int AStarId
    {
        get => _aStarId;
    }

    public TileInfo(int x, int y, int aStarId)
    {
        _x = x;
        _y = y;
        _aStarId = aStarId;
    }

    public void AddConnectedTile(TileInfo tileInfo)
    {
        _connectedTiles.Add(tileInfo);
    }
}