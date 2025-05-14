using System.Collections.Generic;
using Godot;
using TerraObserver.Scenes.Planets.Models;
using TerraObserver.Scenes.Planets.Views;

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
        set
        {
            _planet = value;
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
            _hexSphereConfigs = value;
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
}