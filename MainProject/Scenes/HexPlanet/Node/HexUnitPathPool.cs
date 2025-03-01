using System;
using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class HexUnitPathPool : Node3D
{
    [Export] private PackedScene _pathScene;

    private readonly List<HexUnitPath> _paths = [];

    public void NewTask(HexUnit unit, Curve3D curve, Action onFinished)
    {
        var path = FetchPath();
        path.NewTask(unit, curve, onFinished);
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