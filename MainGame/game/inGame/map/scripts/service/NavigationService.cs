using System;
using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts.service;

public class NavigationService
{
    public static NavigationService Instance => Lazy.Value;
    private static readonly Lazy<NavigationService> Lazy = new(() => new NavigationService());

    private NavigationService()
    {
    }

    private readonly AStar2D _aStar2D = new();

    public void AddPoint(int id, Vector2 vec)
    {
        _aStar2D.AddPoint(id, vec);
    }

    public void ConnectPoints(int fromId, int toId)
    {
        _aStar2D.ConnectPoints(fromId, toId);
    }

    public long[] GetPointConnections(int id)
    {
        return _aStar2D.GetPointConnections(id);
    }
}