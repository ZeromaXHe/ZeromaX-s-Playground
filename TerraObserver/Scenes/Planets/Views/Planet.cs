using System;
using Godot;
using Godot.Abstractions.Extensions.Planets;
using TO.FSharp.Commons.Constants.Shaders;
using TO.FSharp.Commons.Utils;

namespace TerraObserver.Scenes.Planets.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
[Tool]
public partial class Planet : Node3D, IPlanet
{
    #region 事件和 Export 属性

    public event Action? ParamsChanged;

    [ExportGroup("戈德堡多面体配置")]
    [Export(PropertyHint.Range, "5, 1000")]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            CalcUnitHeight();
            ParamsChanged?.Invoke();
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
            CalcUnitHeight();
            ParamsChanged?.Invoke();
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
            CalcUnitHeight();
            ParamsChanged?.Invoke();
        }
    }

    private int _chunkDivisions = 2;

    [ExportGroup("噪声配置")]
    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    [Export]
    public Texture2D? NoiseSource
    {
        get => _noiseSource;
        set
        {
            _noiseSource = value;
            NoiseSourceImage = value?.GetImage();
        }
    }

    private Texture2D? _noiseSource;

    public Image? NoiseSourceImage { get; private set; }

    [Export]
    public ulong Seed
    {
        get => _seed;
        set { _seed = value; }
    }

    private ulong _seed = 1234;

    [ExportGroup("天体运动设置")]
    // 行星公转
    [Export]
    public bool PlanetRevolution { get; set; } = true;

    // 行星自转
    [Export] public bool PlanetRotation { get; set; } = true;

    // 卫星公转
    [Export] public bool SatelliteRevolution { get; set; } = true;

    // 卫星自转
    [Export] public bool SatelliteRotation { get; set; } = true;

    #endregion

    #region 外部变量、属性

    // 单位高度
    public float UnitHeight { get; private set; } = 1.5f;
    public float MaxHeight { get; private set; } = 15f;
    public float MaxHeightRatio { get; private set; } = 0.1f;
    private const float MaxHeightRadiusRatio = 0.2f;

    // [Export(PropertyHint.Range, "10, 15")]
    public int ElevationStep { get; set; } = 10; // 这里对应含义是 Elevation 分为几级

    public float StandardScale => Radius / HexMetrics.standardRadius * HexMetrics.standardDivisions / Divisions;

    // 默认水面高度 [Export(PropertyHint.Range, "1, 5")]
    public int DefaultWaterLevel { get; set; } = 5;

    #endregion

    private void CalcUnitHeight()
    {
        MaxHeightRatio = StandardScale * MaxHeightRadiusRatio;
        MaxHeight = Radius * MaxHeightRatio;
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.maxHeight, MaxHeight);
        UnitHeight = MaxHeight / ElevationStep;
    }
}