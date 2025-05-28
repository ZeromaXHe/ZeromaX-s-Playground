using System.Collections.Generic;
using Friflo.Engine.ECS;
using Godot;
using TerraObserver.Scenes.Planets.Models;
using TerraObserver.Scenes.Planets.Views;
using TO.Apps.Planets;
using TO.Apps.Planets.Services;
using TO.Infras.Abstractions.Planets.Repos;
using TO.Infras.Planets.Repos;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node
{
    #region 外部依赖

    [ExportGroup("Views 显示层")]
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

    [ExportGroup("Models 模型层")]
    [Export]
    public HexSphereConfigs? HexSphereConfigs
    {
        get => _hexSphereConfigs;
        set
        {
            if (_hexSphereConfigs != null)
                _hexSphereConfigs.ParamsChanged -= DrawHexSphereMesh;
            _hexSphereConfigs = value;
            if (_hexSphereConfigs != null)
                _hexSphereConfigs.ParamsChanged += DrawHexSphereMesh;
            UpdateConfigurationWarnings();
        }
    }

    private HexSphereConfigs? _hexSphereConfigs;

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
            warnings.Add("显示层：Planet 为空;");
        if (HexSphereConfigs == null)
            warnings.Add("模型层: HexSphereSettings 为空;");
        if (CatlikeCodingNoise == null)
            warnings.Add("模型层: CatlikeCodingNoise 为空;");
        return warnings.ToArray();
    }

    #endregion

    #region 内部依赖

    private readonly EntityStore _store = new();
    // Repos
    private IPointRepo _pointRepo = null!;
    private IFaceRepo _faceRepo = null!;
    private ITileRepo _tileRepo = null!;
    private IChunkRepo _chunkRepo = null!;
    // Services
    private HexSphereService _hexSphereService = null!;
    // Worlds
    private PlanetWorld _planetWorld = null!;

    private void ComposeRoot()
    {
        _pointRepo = new PointRepo(_store);
        _faceRepo = new FaceRepo(_store);
        _tileRepo = new TileRepo(_store);
        _chunkRepo = new ChunkRepo(_store);
        _hexSphereService = new HexSphereService(_pointRepo, _faceRepo, _tileRepo, _chunkRepo);
        _planetWorld = new PlanetWorld(_hexSphereService);
    }

    #endregion

    private bool NodeReady { get; set; }

    public override void _Ready()
    {
        NodeReady = true;
        ComposeRoot();
        DrawHexSphereMesh();
    }

    public override void _EnterTree()
    {
        if (HexSphereConfigs != null)
            HexSphereConfigs.ParamsChanged += DrawHexSphereMesh;
    }

    public override void _ExitTree()
    {
        if (HexSphereConfigs != null)
            HexSphereConfigs.ParamsChanged -= DrawHexSphereMesh;
    }

    private void DrawHexSphereMesh()
    {
        if (!NodeReady || HexSphereConfigs == null || Planet == null)
            return;
        _planetWorld.DrawHexSphereMesh(Planet, HexSphereConfigs);
    }
}