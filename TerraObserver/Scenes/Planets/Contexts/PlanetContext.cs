using Godot;
using TerraObserver.Scenes.Planets.Views;
using TO.FSharp.Apps;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node
{
    [Export] public Planet? Planet { get; set; }

    private bool NodeReady { get; set; }
    private PlanetApp _planetApp = null!;

    public override void _Ready()
    {
        _planetApp = new PlanetApp(Planet);
        NodeReady = true;
        if (Planet != null)
        {
            DrawHexSphereMesh();
            Planet.ParamsChanged += DrawHexSphereMesh;
        }
    }

    private void DrawHexSphereMesh()
    {
        if (!NodeReady || Planet == null)
            return;
        _planetApp.DrawHexSphereMesh();
    }
}