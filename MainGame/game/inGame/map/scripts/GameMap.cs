using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts;

public class GameMap
{
    private readonly AStar2D _aStar2D = new();
    private readonly TileInfo[,] _tileInfos;
    private readonly int _fromX;
    private readonly int _fromY;

    public GameMap(TileMapLayer baseTerrain)
    {
        // 根据 BaseTerrain 层的最大范围确定数组大小
        var usedRect = baseTerrain.GetUsedRect();
        // GD.Print($"usedRect.Position: {usedRect.Position}, usedRect.End: {usedRect.End}");
        _fromX = usedRect.Position.X;
        _fromY = usedRect.Position.Y;
        var toX = usedRect.End.X;
        var toY = usedRect.End.Y;
        _tileInfos = new TileInfo[toX - _fromX, toY - _fromY];

        // 根据 BaseTerrain 层的内容预生成地图
        var usedCells = baseTerrain.GetUsedCells();
        int aStarId = 0;
        foreach (var cell in usedCells)
        {
            var cellTileInfo = new TileInfo(cell.X, cell.Y, aStarId);
            SetTileInfo(cell, cellTileInfo);
            _aStar2D.AddPoint(aStarId++, cell);

            // 完成连接图
            var surroundings = baseTerrain.GetSurroundingCells(cell);
            foreach (var surrounding in surroundings)
            {
                var surroundingTileInfo = GetTileInfo(surrounding);
                if (surroundingTileInfo == null)
                {
                    continue;
                }

                cellTileInfo.AddConnectedTile(surroundingTileInfo);
                surroundingTileInfo.AddConnectedTile(cellTileInfo);
                _aStar2D.ConnectPoints(cellTileInfo.AStarId, surroundingTileInfo.AStarId);
            }
        }
    }

    private TileInfo GetTileInfo(Vector2I mapVec)
    {
        if (IsOutOfTileInfoRect(mapVec))
        {
            return null;
        }

        return _tileInfos[ToTileInfoX(mapVec.X), ToTileInfoY(mapVec.Y)];
    }

    private void SetTileInfo(Vector2I mapVec, TileInfo tileInfo)
    {
        if (IsOutOfTileInfoRect(mapVec))
        {
            return;
        }

        _tileInfos[ToTileInfoX(mapVec.X), ToTileInfoY(mapVec.Y)] = tileInfo;
    }

    private bool IsOutOfTileInfoRect(Vector2I mapVec)
    {
        var toX = _fromX + _tileInfos.GetLength(0);
        var toY = _fromY + _tileInfos.GetLength(1);
        return mapVec.X < _fromX || mapVec.X >= toX || mapVec.Y < _fromY || mapVec.Y >= toY;
    }

    private int ToTileInfoX(int mapX)
    {
        return mapX - _fromX;
    }

    private int ToTileInfoY(int mapY)
    {
        return mapY - _fromY;
    }
}