using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd4IdleStrategyFS.Godot;
using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts.service;

public class NavigationService: Adapter.IAStar2D
{
    public static NavigationService Instance => Lazy.Value;
    private static readonly Lazy<NavigationService> Lazy = new(() => new NavigationService());

    private NavigationService()
    {
    }

    private readonly AStar2D _aStar2D = new();

    public void AddPoint(int id, int x, int y)
    {
        _aStar2D.AddPoint(id, new Vector2(x, y));
    }

    public void ConnectPoints(int fromId, int toId)
    {
        _aStar2D.ConnectPoints(fromId, toId);
    }

    public IEnumerable<int> GetPointConnections(int id)
    {
        var connectNavIdArr = _aStar2D.GetPointConnections(id);
        return connectNavIdArr.Select(l => (int)l);
    }
}