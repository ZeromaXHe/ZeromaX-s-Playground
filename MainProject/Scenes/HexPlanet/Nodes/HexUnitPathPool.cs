using System.Collections.Generic;
using Domains.Models.Entities.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-02 12:48
public partial class HexUnitPathPool : Node3D, IHexUnitPathPool
{
    public NodeEvent? NodeEvent => null;
    [Export] private PackedScene? _pathScene;

    private readonly List<HexUnitPath> _paths = [];

    public void NewTask(HexUnit unit, List<Tile> pathTiles)
    {
        var path = FetchPath();
        path.TaskStart(pathTiles);
        unit.Travel(path);
    }

    private HexUnitPath FetchPath()
    {
        var path = _paths.Find(p => !p.Working);
        if (path == null)
        {
            path = _pathScene!.Instantiate<HexUnitPath>();
            AddChild(path);
            _paths.Add(path);
        }

        path.Working = true;
        return path;
    }
}