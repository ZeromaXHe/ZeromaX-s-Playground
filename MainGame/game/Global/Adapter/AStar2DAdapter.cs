using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd4IdleStrategyFS.Game;
using BackEnd4IdleStrategyFS.Godot;
using Godot;
using ZeromaXPlayground.game.Global.Common;

namespace ZeromaXPlayground.game.Global.Adapter;

public class AStar2DAdapter(AStar2D aStar2D) : IAdapter.IAStar2D<RepositoryT.GameState>
{
    public RepositoryT.GameState AddPoint(int id, Tuple<int, int> t, RepositoryT.GameState s)
    {
        // GD.Print("AStar2D.AddPoint");
        aStar2D.AddPoint(id, BackEndUtil.From(t));
        return s;
    }

    public RepositoryT.GameState ConnectPoints(int fromId, int toId, RepositoryT.GameState s)
    {
        // GD.Print("AStar2D.ConnectPoints");
        aStar2D.ConnectPoints(fromId, toId);
        return s;
    }

    public IEnumerable<int> GetPointConnections(int id, RepositoryT.GameState s)
    {
        var connectNavIdArr = aStar2D.GetPointConnections(id);
        return connectNavIdArr.Select(l => (int)l);
    }
}