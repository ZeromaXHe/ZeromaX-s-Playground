using System.Collections.Generic;
using Godot;
using TerraObserver.Scenes.Planets.Models;
using TerraObserver.Scenes.Planets.Views;
using TO.Infras.Planets;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node
{
    #region export 变量

    [ExportGroup("Views 显示层")]
    [Export]
    public Planet? Planet
    {
        get => _planet;
        set { _planet = value; }
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
        }
    }

    private HexSphereConfigs? _hexSphereConfigs;

    [Export]
    public CatlikeCodingNoise? CatlikeCodingNoise
    {
        get => _catlikeCodingNoise;
        set { _catlikeCodingNoise = value; }
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

    private PlanetWorld _planetWorld = new();

    private bool NodeReady { get; set; }

    public override void _Ready()
    {
        NodeReady = true;
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
        if (!NodeReady)
            return;
        var time = Time.GetTicksMsec();
        GD.Print($"[===DrawHexSphereMesh===] radius {HexSphereConfigs?.Radius}, divisions {
            HexSphereConfigs?.Divisions}, start at: {time}");
        _planetWorld.ClearOldData();
        _planetWorld.InitHexSphere(HexSphereConfigs);
    }
}