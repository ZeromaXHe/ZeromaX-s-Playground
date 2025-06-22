using System.Collections.Generic;
using Godot;
using TerraObserver.Scenes.Cameras.Views;
using TerraObserver.Scenes.Chunks.Views;
using TerraObserver.Scenes.Geos.Views;
using TerraObserver.Scenes.Planets.Models;
using TerraObserver.Scenes.Planets.Views;
using TerraObserver.Scenes.Uis.Views;
using TO.Abstractions.Views.Chunks;
using TO.Controllers.Apps.Planets;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node
{
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
    private PlanetApp _planetApp = null!;

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

        // App
        _planetApp = new PlanetApp(planet, _catlikeCodingNoise, _orbitCameraRig, _lonLatGrid, _celestialMotion,
            _chunkLoader, _planetHud);
        _planetApp.DrawHexSphereMesh();
        Planet!.ParamsChanged += _planetApp.DrawHexSphereMesh;
        Planet.ParamsChanged += _planetApp.OnPlanetParamsChanged;
        _planetApp.UpdateInsightChunks();
        _planetApp.Init();
        _orbitCameraRig.Transformed += UpdateInsightChunks;
        _orbitCameraRig.Transformed += _planetApp.OnOrbitCameraRigTransformed;
        _orbitCameraRig.Processed += _planetApp.OnOrbitCameraRigProcessed;
        _orbitCameraRig.ZoomChanged += _planetApp.OnOrbitCameraRigZoomChanged;
        _orbitCameraRig.Moved += _lonLatGrid.OnCameraMoved;
        _lonLatGrid.FixFullVisibilityChanged += OnLonLatGridFixFullVisibilityChanged;
        _celestialMotion.SatelliteRadiusRatioChanged += _planetApp.OnCelestialMotionSatelliteRadiusRatioChanged;
        _celestialMotion.SatelliteDistRatioChanged += _planetApp.OnCelestialMotionSatelliteDistRatioChanged;
        _chunkLoader.Processed += _planetApp.OnChunkLoaderProcessed;
        _chunkLoader.HexGridChunkGenerated += OnHexGridChunkGenerated;
        // HUD
        if (!inEditor)
        {
            _planetHud.Planet = planet;
            _planetHud.OnOrbitCameraRigMoved(_orbitCameraRig.GetFocusBasePos(), 0f);
            _planetApp.OnOrbitCameraRigTransformed(_orbitCameraRig.GetViewport().GetCamera3D().GetGlobalTransform(), 0f);
            _orbitCameraRig.Moved += _planetHud.OnOrbitCameraRigMoved;
            _planetHud.LonLatGrid = _lonLatGrid;
            _planetHud.CelestialMotion = _celestialMotion;
        }
    }

    public override void _Notification(int what)
    {
        if (what == (int)NotificationPredelete)
        {
            _orbitCameraRig.Transformed -= UpdateInsightChunks;
            _orbitCameraRig.Moved -= _lonLatGrid.OnCameraMoved;
            if (_planetHud != null!) 
                _orbitCameraRig.Moved -= _planetHud.OnOrbitCameraRigMoved;
            _lonLatGrid.FixFullVisibilityChanged -= OnLonLatGridFixFullVisibilityChanged;
            _chunkLoader.HexGridChunkGenerated -= OnHexGridChunkGenerated;
            if (_planetApp != null!)
            {
                // 首次编译后重载场景时会为空…… 没理解原因
                // 而且绑定和解绑逻辑不能下沉到 F# 层，否则就解绑失败，同样没理解原因
                Planet!.ParamsChanged -= _planetApp.DrawHexSphereMesh;
                Planet.ParamsChanged -= _planetApp.OnPlanetParamsChanged;
                _orbitCameraRig.Transformed -= _planetApp.OnOrbitCameraRigTransformed;
                _orbitCameraRig.Processed -= _planetApp.OnOrbitCameraRigProcessed;
                _orbitCameraRig.ZoomChanged -= _planetApp.OnOrbitCameraRigZoomChanged;
                _celestialMotion.SatelliteRadiusRatioChanged -= _planetApp.OnCelestialMotionSatelliteRadiusRatioChanged;
                _celestialMotion.SatelliteDistRatioChanged -= _planetApp.OnCelestialMotionSatelliteDistRatioChanged;
                _chunkLoader.Processed -= _planetApp.OnChunkLoaderProcessed;
            }
        }
    }

    #endregion

    private void UpdateInsightChunks(Transform3D transform, float delta)
    {
        if (!NodeReady)
            return;
        _planetApp.UpdateInsightChunks();
    }

    private void OnHexGridChunkGenerated(IHexGridChunk chunk) =>
        chunk.Processed += _planetApp.OnHexGridChunkProcessed; // TODO：怎么解绑事件？

    private void OnLonLatGridFixFullVisibilityChanged(bool value)
    {
        if (value)
            _orbitCameraRig.Moved -= _lonLatGrid.OnCameraMoved;
        else
            _orbitCameraRig.Moved += _lonLatGrid.OnCameraMoved;
    }
}