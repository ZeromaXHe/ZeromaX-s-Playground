using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd4IdleStrategyFS.Godot;
using Godot;
using ZeromaXPlayground.game.Global.Common;

namespace ZeromaXPlayground.game.Global.Adapter;

public class TileMapLayerAdapter(TileMapLayer tileMapLayer): IAdapter.ITileMapLayer
{
    public IEnumerable<Tuple<int, int>> GetUsedCells()
    {
        // GD.Print("TileMapLayer.GetUsedCells");
        return tileMapLayer.GetUsedCells().Select(BackEndUtil.To);
    }

    public IEnumerable<Tuple<int, int>> GetSurroundingCells(Tuple<int, int> cell)
    {
        GD.Print($"TileMapLayer.GetSurroundingCells ({cell.Item1}, {cell.Item2})");
        return tileMapLayer.GetSurroundingCells(BackEndUtil.FromI(cell)).Select(BackEndUtil.To);
    }
}