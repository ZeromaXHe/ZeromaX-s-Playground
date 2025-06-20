using System.Collections.Generic;
using Godot;
using TerraObserver.Scenes.Cameras.Views;
using TerraObserver.Scenes.Chunks.Views;
using TerraObserver.Scenes.Geos.Views;
using TerraObserver.Scenes.Planets.Models;
using TerraObserver.Scenes.Planets.Views;
using TerraObserver.Scenes.Uis.Views;
using TO.Abstractions.Chunks;
using TO.Controllers.Apps.Planets;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node
{
    #region 依赖

    private PlanetApp _planetApp = null!;

    #endregion

    #region 事件和 Export 属性

    [Export]
    public Planet? Planet
    {
        get => _planet;
        set
        {
            _planet = value;
            UpdateConfigurationWarnings();
        }
    }

    private Planet? _planet;

    [Export]
    public CatlikeCodingNoise? CatlikeCodingNoise
    {
        get => _catlikeCodingNoise;
        set
        {
            _catlikeCodingNoise = value;
            UpdateConfigurationWarnings();
        }
    }

    private CatlikeCodingNoise? _catlikeCodingNoise;


    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [];
        if (Planet == null)
            warnings.Add("模型层：Planet 不可为空;");
        if (CatlikeCodingNoise == null)
            warnings.Add("模型层: CatlikeCodingNoise 不可为空;");
        return warnings.ToArray();
    }

    #endregion

    #region 内部变量、属性

    private OrbitCameraRig _orbitCameraRig = null!;
    private LonLatGrid _lonLatGrid = null!;
    private CelestialMotion _celestialMotion = null!;
    private ChunkLoader _chunkLoader = null!;
    private PlanetHud _planetHud = null!;

    #endregion

    #region 生命周期

    private bool NodeReady { get; set; }

    public override void _Ready()
    {
        var inEditor = Engine.IsEditorHint();
        _orbitCameraRig = GetNode<OrbitCameraRig>("%OrbitCameraRig");
        _lonLatGrid = GetNode<LonLatGrid>("%LonLatGrid");
        _celestialMotion = GetNode<CelestialMotion>("%CelestialMotion");
        _chunkLoader = GetNode<ChunkLoader>("%ChunkLoader");
        if (!inEditor)
            _planetHud = GetNode<PlanetHud>("%PlanetHud");
        NodeReady = true;

        var planet = Planet!;
        // Catlike Coding 的噪声实现
        CatlikeCodingNoise!.Planet = planet;
        // 轨道相机架
        _orbitCameraRig.Planet = planet;
        // 经纬网
        _lonLatGrid.Planet = planet;
        _lonLatGrid.OrbitCameraRig = _orbitCameraRig;
        // 天体运动
        _celestialMotion.Planet = planet;
        // 分块加载
        _chunkLoader.Planet = planet;
        // HUD
        if (!inEditor)
        {
            _planetHud.Planet = planet;
            _planetHud.OrbitCameraRig = _orbitCameraRig;
            _planetHud.LonLatGrid = _lonLatGrid;
            _planetHud.CelestialMotion = _celestialMotion;
        }

        // App
        _planetApp = new PlanetApp(planet, _catlikeCodingNoise, _chunkLoader);

        _planetApp.DrawHexSphereMesh();
        Planet!.ParamsChanged += DrawHexSphereMesh;
        _planetApp.UpdateInsightChunks();
        _orbitCameraRig.Transformed += UpdateInsightChunks;
        _chunkLoader.Processed += OnChunkLoaderProcessed;
        _chunkLoader.HexGridChunkGenerated += OnHexGridChunkGenerated;
    }

    public override void _Notification(int what)
    {
        if (what == (int)NotificationPredelete)
        {
            Planet!.ParamsChanged -= DrawHexSphereMesh;
            _orbitCameraRig.Transformed -= UpdateInsightChunks;
            _chunkLoader.Processed -= OnChunkLoaderProcessed;
            _chunkLoader.HexGridChunkGenerated -= OnHexGridChunkGenerated;
            _orbitCameraRig.PreDelete();
            _lonLatGrid.PreDelete();
        }
    }

    #endregion

    private void DrawHexSphereMesh()
    {
        if (!NodeReady)
            return;
        _planetApp.DrawHexSphereMesh();
    }

    private void OnChunkLoaderProcessed()
    {
        if (!NodeReady)
            return;
        _planetApp.OnChunkLoaderProcessed();
    }

    private void UpdateInsightChunks(Transform3D transform, float delta)
    {
        if (!NodeReady)
            return;
        _planetApp.UpdateInsightChunks();
    }

    private void OnHexGridChunkGenerated(IHexGridChunk chunk) =>
        chunk.Processed += _planetApp.OnHexGridChunkProcessed; // TODO：怎么解绑事件？
}