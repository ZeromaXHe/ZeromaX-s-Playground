using System.Collections.Generic;
using Godot;
using TerraObserver.Scenes.Cameras.Views;
using TerraObserver.Scenes.Chunks.Views;
using TerraObserver.Scenes.Features.Views;
using TerraObserver.Scenes.Geos.Views;
using TerraObserver.Scenes.Maps.Models;
using TerraObserver.Scenes.Maps.Views;
using TerraObserver.Scenes.Planets.Models;
using TerraObserver.Scenes.Planets.Views;
using TerraObserver.Scenes.Uis.Views;
using TerraObserver.Scenes.Units.Views;
using TO.Domains.Apps;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node
{
    #region Export 属性

    [Export]
    public PlanetConfig? PlanetConfig
    {
        get => _planetConfig;
        set
        {
            _planetConfig = value;
            UpdateConfigurationWarnings();
        }
    }

    private PlanetConfig? _planetConfig;

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

    [Export]
    public HexMapGenerator? HexMapGenerator
    {
        get => _hexMapGenerator;
        set
        {
            // if (_hexMapGenerator != null)
            //     _hexMapGenerator.Changed -= UpdateConfigurationWarnings;
            _hexMapGenerator = value;
            UpdateConfigurationWarnings();
            // if (_hexMapGenerator != null)
            //     _hexMapGenerator.Changed += UpdateConfigurationWarnings;
        }
    }

    private HexMapGenerator? _hexMapGenerator;

    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [];
        if (PlanetConfig == null)
            warnings.Add("模型层：Planet 不可为空;");
        if (CatlikeCodingNoise == null)
            warnings.Add("模型层: CatlikeCodingNoise 不可为空;");
        if (HexMapGenerator == null)
            warnings.Add("模型层: HexMapGenerator 不可为空;");
        // else if (HexMapGenerator.LandGenerator == null)
        //     warnings.Add("模型层: HexMapGenerator.LandGenerator 不可为空;");
        return warnings.ToArray();
    }

    #endregion

    #region 私有字段

    private OrbitCameraRig _orbitCameraRig = null!;
    private LonLatGrid _lonLatGrid = null!;
    private CelestialMotion _celestialMotion = null!;
    private UnitManager _unitManager = null!;
    private HexUnitPathPool _hexUnitPathPool = null!;
    private FeatureMeshManager _featureMeshManager = null!;
    private FeaturePreviewManager _featurePreviewManager = null!;
    private ChunkLoader _chunkLoader = null!;
    private SelectTileViewer _selectTileViewer = null!;
    private EditPreviewChunk _editPreviewChunk = null!;
    private MiniMapManager _miniMapManager = null!;
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
        _featureMeshManager = GetNode<FeatureMeshManager>("%FeatureMeshManager");
        _featurePreviewManager = GetNode<FeaturePreviewManager>("%FeaturePreviewManager");
        _chunkLoader = GetNode<ChunkLoader>("%ChunkLoader");
        if (!inEditor)
        {
            _unitManager = GetNode<UnitManager>("%UnitManager");
            _hexUnitPathPool = GetNode<HexUnitPathPool>("%HexUnitPathPool");
            _selectTileViewer = GetNode<SelectTileViewer>("%SelectTileViewer");
            _editPreviewChunk = GetNode<EditPreviewChunk>("%EditPreviewChunk");
            _planetHud = GetNode<PlanetHud>("%PlanetHud");
            _miniMapManager = _planetHud.MiniMapManager;
        }

        NodeReady = true;
        // App
        _planetApp = new PlanetApp(PlanetConfig, CatlikeCodingNoise, _orbitCameraRig, _lonLatGrid, _celestialMotion,
            _unitManager, _hexUnitPathPool, _featureMeshManager, _featurePreviewManager, _chunkLoader,
            _selectTileViewer, _editPreviewChunk, _miniMapManager, _hexMapGenerator, _planetHud);
        PlanetConfig!.ParamsChanged += _planetApp.DrawHexSphereMesh;
        PlanetConfig.ParamsChanged += _planetApp.OnPlanetConfigParamsChanged;
        _orbitCameraRig.Transformed += UpdateInsightChunks;
        _orbitCameraRig.Processed += _planetApp.OnOrbitCameraRigProcessed;
        _orbitCameraRig.ZoomChanged += _planetApp.OnOrbitCameraRigZoomChanged;
        _orbitCameraRig.Moved += _planetApp.OnLonLatGridCameraMoved;
        _lonLatGrid.FixFullVisibilityChanged += OnLonLatGridFixFullVisibilityChanged;
        _lonLatGrid.DoDrawRequested += _planetApp.OnLonLatGridDoDrawRequested;
        _celestialMotion.SatelliteRadiusRatioChanged += _planetApp.OnCelestialMotionSatelliteRadiusRatioChanged;
        _celestialMotion.SatelliteDistRatioChanged += _planetApp.OnCelestialMotionSatelliteDistRatioChanged;
        _chunkLoader.Processed += _planetApp.OnChunkLoaderProcessed;
        _chunkLoader.HexGridChunkGenerated += OnHexGridChunkGenerated;
        // HUD
        if (!inEditor)
        {
            _unitManager.UnitInstantiated += OnHexUnitInstantiated;
            _orbitCameraRig.Moved += _planetApp.OnPlanetHudOrbitCameraRigMoved;
            _orbitCameraRig.Transformed += _planetApp.OnPlanetHudOrbitCameraRigTransformed;
            _miniMapManager.Clicked += _planetApp.OnMiniMapClicked;
            _planetHud.ChosenTileIdChanged += _planetApp.OnPlanetHudChosenTileIdChanged;
            _planetHud.LonLatFixCheckButtonToggled += _planetApp.LonLatGridToggleFixFullVisibility;
            _planetHud.CelestialMotionCheckButtonToggled += _planetApp.CelestialMotionToggleAllMotions;
            _planetHud.RadiusLineEditTextSubmitted += _planetApp.OnPlanetHudRadiusLineEditTextSubmitted;
            _planetHud.DivisionLineEditTextSubmitted += _planetApp.OnPlanetHudDivisionLineEditTextSubmitted;
            _planetHud.ChunkDivisionLineEditTextSubmitted += _planetApp.OnPlanetHudChunkDivisionLineEditTextSubmitted;
            _planetHud.LandGenOptionButton.ItemSelected += _planetApp.OnPlanetHudLandGenOptionButtonItemSelected;
        }

        _planetApp.DrawHexSphereMesh();
        _planetApp.UpdateInsightChunks();
        _planetApp.Init();
        if (!inEditor)
        {
            _planetApp.OnPlanetHudOrbitCameraRigTransformed(
                _orbitCameraRig.GetViewport().GetCamera3D().GetGlobalTransform(), 0f);
        }
    }

    public override void _Notification(int what)
    {
        // 根节点的 pre-delete 会在所有子节点的 pre-delete 前执行
        if (what == (int)NotificationPredelete)
        {
            NodeReady = false;

            _orbitCameraRig.Transformed -= UpdateInsightChunks;
            _lonLatGrid.FixFullVisibilityChanged -= OnLonLatGridFixFullVisibilityChanged;
            _chunkLoader.HexGridChunkGenerated -= OnHexGridChunkGenerated;
            // if (HexMapGenerator != null)
            //     HexMapGenerator.Changed -= UpdateConfigurationWarnings;
            if (_planetApp != null!)
            {
                // 首次编译后重载场景时会为空…… 没理解原因
                // 而且绑定和解绑逻辑不能下沉到 F# 层，否则就解绑失败，同样没理解原因
                PlanetConfig!.ParamsChanged -= _planetApp.DrawHexSphereMesh;
                PlanetConfig.ParamsChanged -= _planetApp.OnPlanetConfigParamsChanged;
                _orbitCameraRig.Processed -= _planetApp.OnOrbitCameraRigProcessed;
                _orbitCameraRig.ZoomChanged -= _planetApp.OnOrbitCameraRigZoomChanged;
                _orbitCameraRig.Moved -= _planetApp.OnLonLatGridCameraMoved;
                _lonLatGrid.DoDrawRequested -= _planetApp.OnLonLatGridDoDrawRequested;
                _celestialMotion.SatelliteRadiusRatioChanged -= _planetApp.OnCelestialMotionSatelliteRadiusRatioChanged;
                _celestialMotion.SatelliteDistRatioChanged -= _planetApp.OnCelestialMotionSatelliteDistRatioChanged;
                _chunkLoader.Processed -= _planetApp.OnChunkLoaderProcessed;
                if (_planetHud != null!)
                {
                    _unitManager.UnitInstantiated -= OnHexUnitInstantiated;
                    _orbitCameraRig.Moved -= _planetApp.OnPlanetHudOrbitCameraRigMoved;
                    _orbitCameraRig.Transformed -= _planetApp.OnPlanetHudOrbitCameraRigTransformed;
                    _planetHud.LonLatFixCheckButtonToggled -= _planetApp.LonLatGridToggleFixFullVisibility;
                    _planetHud.CelestialMotionCheckButtonToggled -= _planetApp.CelestialMotionToggleAllMotions;
                    _planetHud.RadiusLineEditTextSubmitted -= _planetApp.OnPlanetHudRadiusLineEditTextSubmitted;
                    _planetHud.DivisionLineEditTextSubmitted -= _planetApp.OnPlanetHudDivisionLineEditTextSubmitted;
                    _planetHud.ChunkDivisionLineEditTextSubmitted -=
                        _planetApp.OnPlanetHudChunkDivisionLineEditTextSubmitted;
                    _planetHud.LandGenOptionButton.ItemSelected -=
                        _planetApp.OnPlanetHudLandGenOptionButtonItemSelected;
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        if (!NodeReady || _planetApp == null!) return; // _planetApp 在重载场景时为空
        _planetApp.OnProcessed((float)delta);
    }

    #endregion

    private void UpdateInsightChunks(Transform3D transform, float delta)
    {
        if (!NodeReady)
            return;
        _planetApp.UpdateInsightChunks();
    }

    private void OnHexGridChunkGenerated(HexGridChunk chunk) =>
        chunk.Processed += _planetApp.OnHexGridChunkProcessed; // TODO：怎么解绑事件？

    private void OnHexUnitInstantiated(HexUnit unit) =>
        unit.Processed += _planetApp.OnHexUnitProcessed;

    private void OnLonLatGridFixFullVisibilityChanged(bool value)
    {
        if (value)
            _orbitCameraRig.Moved -= _planetApp.OnLonLatGridCameraMoved;
        else
            _orbitCameraRig.Moved += _planetApp.OnLonLatGridCameraMoved;
    }
}