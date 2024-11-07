using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd4IdleStrategyFS.Godot;
using Godot;
using ZeromaXPlayground.game.Global.Common;

namespace ZeromaXPlayground.game.Global.Adapter;

public class AStar2DAdapter(AStar2D aStar2D) : IAdapter.IAStar2D
{
    public void AddPoint(int id, Tuple<int, int> t)
    {
        // GD.Print("AStar2D.AddPoint");
        aStar2D.AddPoint(id, BackEndUtil.From(t));
    }

    public void ConnectPoints(int fromId, int toId)
    {
        GD.Print($"AStar2D.ConnectPoints ({fromId}, {toId})");
        aStar2D.ConnectPoints(fromId, toId);
    }

    public IEnumerable<int> GetPointConnections(int id)
    {
        GD.Print($"AStar2D.GetPointConnections {id}");
        var connectNavIdArr = aStar2D.GetPointConnections(id);
        return connectNavIdArr.Select(l => (int)l);
    }
}