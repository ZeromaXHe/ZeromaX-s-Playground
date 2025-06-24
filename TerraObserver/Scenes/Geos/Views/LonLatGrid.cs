using System;
using Godot;
using TO.Abstractions.Views.Geos;
using TO.Presenters.Views.Geos;

namespace TerraObserver.Scenes.Geos.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-04 10:48:55
[Tool]
public partial class LonLatGrid : Node3D, ILonLatGrid
{
    #region 事件

    public event Action<bool>? FixFullVisibilityChanged;
    public event Action? DoDrawRequested;

    #endregion

    #region Export 属性

    [ExportToolButton("手动触发重绘经纬线", Icon = "WorldEnvironment")]
    public Callable Redraw => Callable.From(() => DoDrawRequested?.Invoke());

    [Export(PropertyHint.Range, "0, 180")] public int LongitudeInterval { get; set; } = 15;
    [Export(PropertyHint.Range, "0, 90")] public int LatitudeInterval { get; set; } = 15;
    [Export(PropertyHint.Range, "1, 100")] public int Segments { get; set; } = 30; // 每个 90 度的弧线被划分多少段
    [Export] public Material? LineMaterial { get; set; }

    [ExportGroup("颜色设置")] [Export] public Color NormalLineColor { get; set; } = Colors.SkyBlue with { A8 = 40 };

    [Export] public Color DeeperLineColor { get; set; } = Colors.DeepSkyBlue with { A8 = 40 };
    [Export] public int DeeperLineInterval { get; set; } = 3; // 更深颜色的线多少条出现一次
    [Export] public Color TropicColor { get; set; } = Colors.Green with { A8 = 40 }; // 南北回归线颜色
    [Export] public Color CircleColor { get; set; } = Colors.Aqua with { A8 = 40 }; // 南北极圈颜色
    [Export] public Color EquatorColor { get; set; } = Colors.Yellow with { A8 = 40 }; // 赤道颜色
    [Export] public Color Degree90LongitudeColor { get; set; } = Colors.Orange with { A8 = 40 }; // 东西经 90 度颜色
    [Export] public Color MeridianColor { get; set; } = Colors.Red with { A8 = 40 }; // 子午线颜色

    [ExportGroup("开关特定线显示")] [Export] public bool DrawTropicOfCancer { get; set; } = true; // 是否绘制北回归线
    [Export] public bool DrawTropicOfCapricorn { get; set; } = true; // 是否绘制南回归线
    [Export] public bool DrawArcticCircle { get; set; } = true; // 是否绘制北极圈
    [Export] public bool DrawAntarcticCircle { get; set; } = true; // 是否绘制南极圈

    [ExportGroup("透明度设置")]
    [Export(PropertyHint.Range, "0.01, 5")]
    public float FullVisibilityTime { get; set; } = 1.2f;

    [Export(PropertyHint.Range, "0, 1")] public float FullVisibility { get; set; } = 0.6f;
    private bool _fixFullVisibility;

    // 是否锁定开启完全显示
    [Export]
    public bool FixFullVisibility
    {
        get => _fixFullVisibility;
        set
        {
            _fixFullVisibility = value;
            if (value)
            {
                Visibility = FullVisibility;
                FadeVisibility = false;
                Show();
            }
            else
                SetProcess(true);

            if (_ready)
                FixFullVisibilityChanged?.Invoke(value);
        }
    }

    #endregion

    #region 普通属性

    // 对应着色器中的 alpha_factor
    private float _visibility = 1f;

    public float Visibility
    {
        get => _visibility;
        set
        {
            _visibility = Mathf.Clamp(value, 0f, FullVisibility);
            if (_ready && LineMaterial is ShaderMaterial shaderMaterial)
                shaderMaterial.SetShaderParameter("alpha_factor", _visibility);
        }
    }

    public bool FadeVisibility { get; set; }

    public float Radius { get; set; } = 110f;
    public MeshInstance3D? MeshIns { get; set; }

    #endregion

    #region 生命周期

    private bool _ready;

    public override void _Ready()
    {
        MeshIns = new MeshInstance3D();
        AddChild(MeshIns);
        _ready = true;
        // 在 _ready = true 后面，触发 setter 的着色器参数初始化
        Visibility = FullVisibility;
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint() || FixFullVisibility)
        {
            SetProcess(false); // 编辑器下以及锁定显示时，无需处理显隐
            return;
        }

        if (FadeVisibility)
            Visibility -= (float)delta / FullVisibilityTime;
        FadeVisibility = true;

        if (Visibility > 0) return;
        Hide();
        SetProcess(false);
    }

    #endregion
}