using Friflo.Engine.ECS;
using Godot;
using TerraObserver.Scenes.Cameras.Views;
using TerraObserver.Scenes.Planets.Views;
using TO.FSharp.Apps.Cameras;
using TO.FSharp.Apps.Planets;
using TO.FSharp.Commons.DataStructures;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node
{
    [Export]
    public Planet? Planet
    {
        get => _planet;
        set
        {
            if (NodeReady && _planet != null)
                _planet.ParamsChanged -= DrawHexSphereMesh;
            _planet = value;
            if (NodeReady && _planet != null)
                _planet.ParamsChanged += DrawHexSphereMesh;
        }
    }

    private Planet? _planet;

    [Export]
    public OrbitCameraRig? OrbitCameraRig
    {
        get => _orbitCameraRig;
        set
        {
            if (NodeReady && _orbitCameraRig != null)
            {
                _orbitCameraRig.RadiusChanged -= OnOrbitCameraRadiusChanged;
                _orbitCameraRig.ZoomChanged -= OnOrbitCameraZoomChanged;
            }

            _orbitCameraRig = value;
            if (NodeReady && _orbitCameraRig != null)
            {
                _orbitCameraRig.RadiusChanged += OnOrbitCameraRadiusChanged;
                _orbitCameraRig.ZoomChanged += OnOrbitCameraZoomChanged;
            }
        }
    }

    private OrbitCameraRig? _orbitCameraRig;

    private bool NodeReady { get; set; }
    private PlanetApp _planetApp = null!;
    private OrbitCameraApp _orbitCameraApp = null!;

    public override void _Ready()
    {
        var store = new EntityStore();
        var chunkVpTree = new VpTree<Vector3>();
        var tileVpTree = new VpTree<Vector3>();
        _planetApp = new PlanetApp(store, chunkVpTree, tileVpTree);
        _orbitCameraApp = new OrbitCameraApp();
        NodeReady = true;

        DrawHexSphereMesh();
        if (Planet != null)
            Planet.ParamsChanged += DrawHexSphereMesh;
        if (OrbitCameraRig != null)
        {
            OrbitCameraRig.RadiusChanged += OnOrbitCameraRadiusChanged;
            OrbitCameraRig.ZoomChanged += OnOrbitCameraZoomChanged;
        }
    }

    private void DrawHexSphereMesh()
    {
        if (!NodeReady || Planet == null)
            return;
        _planetApp.DrawHexSphereMesh(Planet);
    }

    private void OnOrbitCameraRadiusChanged(float radius)
    {
        if (!NodeReady || Planet == null || OrbitCameraRig == null)
            return;
        OrbitCameraRig.SetRadius(radius, Planet.MaxHeightRatio, Planet.StandardScale);
    }

    private void OnOrbitCameraZoomChanged(float radius)
    {
        if (!NodeReady || Planet == null || OrbitCameraRig == null)
            return;
        OrbitCameraRig.SetZoom(radius, Planet.StandardScale);
    }
}