using System;
using Godot;
using TO.Domains.Functions.HexMetrics;
using TO.Domains.Functions.Shaders;
using TO.Domains.Types.Configs;
using TO.Domains.Types.HexGridCoords;

namespace TerraObserver.Scenes.Planets.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
[Tool]
[GlobalClass]
public partial class PlanetConfig : Resource, IPlanetConfig
{
    #region 事件

    public event Action? ParamsChanged;

    #endregion

    #region Export 属性

    [ExportGroup("戈德堡多面体配置")]
    [Export(PropertyHint.Range, "5, 1000")]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            OnParamsChanged();
        }
    }

    private float _radius = 100f;

    [Export(PropertyHint.Range, "1, 200")]
    public int Divisions
    {
        get => _divisions;
        set
        {
            _divisions = value;
            _chunkDivisions = Mathf.Min(Mathf.Max(1, _divisions / 10), _chunkDivisions);
            SphereAxial.Div = _divisions;
            OnParamsChanged();
        }
    }

    private int _divisions = 20;

    [Export(PropertyHint.Range, "1, 20")]
    public int ChunkDivisions
    {
        get => _chunkDivisions;
        set
        {
            _chunkDivisions = value;
            _divisions = Mathf.Max(Mathf.Min(200, _chunkDivisions * 10), _divisions);
            SphereAxial.Div = _divisions;
            OnParamsChanged();
        }
    }

    private int _chunkDivisions = 2;

    #endregion

    #region 普通属性

    // 单位高度
    public float UnitHeight { get; private set; } = 1.5f;
    public float MaxHeight { get; private set; } = 15f;
    public float MaxHeightRatio { get; private set; } = 0.1f;
    private const float MaxHeightRadiusRatio = 0.2f;

    // [Export(PropertyHint.Range, "10, 15")]
    public int ElevationStep { get; set; } = 10; // 这里对应含义是 Elevation 分为几级

    public float StandardScale => Radius / HexMetrics.StandardRadius * HexMetrics.StandardDivisions / Divisions;

    // 默认水面高度 [Export(PropertyHint.Range, "1, 5")]
    public int DefaultWaterLevel { get; set; } = 5;

    #endregion

    private void OnParamsChanged()
    {
        CalcUnitHeight();
        ParamsChanged?.Invoke();
    }

    private void CalcUnitHeight()
    {
        MaxHeightRatio = StandardScale * MaxHeightRadiusRatio;
        MaxHeight = Radius * MaxHeightRatio;
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.MaxHeight, MaxHeight);
        UnitHeight = MaxHeight / ElevationStep;
    }
}