using System;
using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class HexUnitPathPool : Node3D
{
    [Export] private PackedScene _pathScene;

    private readonly List<HexUnitPath> _paths = [];

    public void NewTask(HexUnit unit, Curve3D curve, int toTileId)
    {
        var path = FetchPath();
        path.TaskStart(curve);
        unit.StartPath(path, toTileId);
    }

    private HexUnitPath FetchPath()
    {
        var path = _paths.Find(p => !p.Working);
        if (path == null)
        {
            path = _pathScene.Instantiate<HexUnitPath>();
            AddChild(path);
            _paths.Add(path);
        }

        path.Working = true;
        return path;
    }
}