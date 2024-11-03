using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd4IdleStrategyFS.Game;
using BackEnd4IdleStrategyFS.Godot;
using Godot;
using ZeromaXPlayground.game.Global.Common;

namespace ZeromaXPlayground.game.Global.Adapter;

public class TileMapLayerAdapter(TileMapLayer tileMapLayer): IAdapter.ITileMapLayer<RepositoryT.GameState>
{
    public IEnumerable<Tuple<int, int>> GetUsedCells(RepositoryT.GameState s)
    {
        // GD.Print("TileMapLayer.GetUsedCells");
        return tileMapLayer.GetUsedCells().Select(BackEndUtil.To);
    }

    public IEnumerable<Tuple<int, int>> GetSurroundingCells(Tuple<int, int> cell, RepositoryT.GameState s)
    {
        // GD.Print("TileMapLayer.GetSurroundingCells");
        return tileMapLayer.GetSurroundingCells(BackEndUtil.FromI(cell)).Select(BackEndUtil.To);
    }
}