using System.Collections.Generic;
using Godot;
using TerraObserver.Scenes.Planets.Models;
using TerraObserver.Scenes.Planets.Views;
using TO.IocContainers.Planets;

namespace TerraObserver.Scenes.Planets.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 19:48
[Tool]
public partial class PlanetContext : Node, ISerializationListener
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
            RefreshContainer();
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
            RefreshContainer();
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
            RefreshContainer();
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

    #region 容器

    private PlanetContainer? _planetContainer;

    private void InitContainer()
    {
        if (Planet != null && HexSphereConfigs != null)
            _planetContainer = new PlanetContainer(Planet, HexSphereConfigs);
        UpdateConfigurationWarnings();
    }

    private void DestroyContainer()
    {
        _planetContainer?.Dispose();
        _planetContainer = null;
    }

    private void RefreshContainer()
    {
        DestroyContainer();
        InitContainer();
    }

    public void OnBeforeSerialize() => DestroyContainer();
    public void OnAfterDeserialize() => InitContainer();
    public override void _Ready() => InitContainer();

    public override void _Notification(int what)
    {
        switch (what)
        {
            case (int)NotificationPredelete:
                DestroyContainer();
                break;
        }
    }

    private void DrawHexSphereMesh() => _planetContainer?.PlanetCommander.DrawHexSphereMesh();

    #endregion
}