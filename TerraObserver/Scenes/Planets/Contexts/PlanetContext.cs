using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Friflo.Engine.ECS;
using Godot;
using TerraObserver.Scenes.Cameras.Views;
using TerraObserver.Scenes.Geos.Views;
using TerraObserver.Scenes.Planets.Views;
using TerraObserver.Scenes.Uis.Views;
using TO.FSharp.Apps.Planets;
using TO.FSharp.Commons.DataStructures;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node
{
    #region 依赖

    private readonly PlanetApp _planetApp;

    public PlanetContext()
    {
        var store = new EntityStore();
        var chunkVpTree = new VpTree<Vector3>();
        var tileVpTree = new VpTree<Vector3>();
        _planetApp = new PlanetApp(store, chunkVpTree, tileVpTree);
    }

    #endregion

    #region 内部变量、属性

    private Planet _planet = null!;
    private OrbitCameraRig _orbitCameraRig = null!;
    private LonLatGrid _lonLatGrid = null!;
    private CelestialMotion _celestialMotion = null!;
    private PlanetHud _planetHud = null!;

    #endregion

    #region 生命周期

    private bool NodeReady { get; set; }

    public override void _Ready()
    {
        var inEditor = Engine.IsEditorHint();
        _planet = GetNode<Planet>("%Planet");
        _orbitCameraRig = GetNode<OrbitCameraRig>("%OrbitCameraRig");
        _lonLatGrid = GetNode<LonLatGrid>("%LonLatGrid");
        _celestialMotion = GetNode<CelestialMotion>("%CelestialMotion");
        if (!inEditor)
            _planetHud = GetNode<PlanetHud>("%PlanetHud");
        NodeReady = true;

        DrawHexSphereMesh();
        _planet.ParamsChanged += DrawHexSphereMesh;

        // 轨道相机架
        _orbitCameraRig.Planet = _planet;
        // 经纬网
        _lonLatGrid.Planet = _planet;
        _lonLatGrid.OrbitCameraRig = _orbitCameraRig;
        // 天体运动
        _celestialMotion.Planet = _planet;
        // HUD
        if (!inEditor)
        {
            _planetHud.Planet = _planet;
            _planetHud.OrbitCameraRig = _orbitCameraRig;
            _planetHud.LonLatGrid = _lonLatGrid;
            _planetHud.CelestialMotion = _celestialMotion;
        }
    }

    #endregion

    private void DrawHexSphereMesh()
    {
        if (!NodeReady)
            return;
        _planetApp.DrawHexSphereMesh(_planet);
    }
}