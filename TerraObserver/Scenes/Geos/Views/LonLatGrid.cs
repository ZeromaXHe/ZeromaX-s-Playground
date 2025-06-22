using System;
using Godot;
using TO.Abstractions.Views.Geos;
using TO.Presenters.Views.Geos;

namespace TerraObserver.Scenes.Geos.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-04 10:48:55
[Tool]
public partial class LonLatGrid : LonLatGridFS, ILonLatGrid
{
    #region 生命周期

    // 需要忽略 IDE 省略 partial、_Ready 等的提示，必须保留它们
    public override void _Ready() => base._Ready();
    public override void _Process(double delta) => base._Process(delta);

    #endregion

    #region 事件

    public event Action<bool>? FixFullVisibilityChanged;

    public override void EmitFixFullVisibilityChanged(bool fixFullVisibility) =>
        FixFullVisibilityChanged?.Invoke(fixFullVisibility);

    #endregion

    #region Export 属性

    [ExportToolButton("手动触发重绘经纬线", Icon = "WorldEnvironment")]
    public override Callable Redraw => Callable.From(DoDraw);

    [Export(PropertyHint.Range, "0, 180")] public override int LongitudeInterval { get; set; } = 15;
    [Export(PropertyHint.Range, "0, 90")] public override int LatitudeInterval { get; set; } = 15;
    [Export(PropertyHint.Range, "1, 100")] public override int Segments { get; set; } = 30; // 每个 90 度的弧线被划分多少段
    [Export] public override Material? LineMaterial { get; set; }

    [ExportGroup("颜色设置")]
    [Export]
    public override Color NormalLineColor { get; set; } = Colors.SkyBlue with { A8 = 40 };

    [Export] public override Color DeeperLineColor { get; set; } = Colors.DeepSkyBlue with { A8 = 40 };
    [Export] public override int DeeperLineInterval { get; set; } = 3; // 更深颜色的线多少条出现一次
    [Export] public override Color TropicColor { get; set; } = Colors.Green with { A8 = 40 }; // 南北回归线颜色
    [Export] public override Color CircleColor { get; set; } = Colors.Aqua with { A8 = 40 }; // 南北极圈颜色
    [Export] public override Color EquatorColor { get; set; } = Colors.Yellow with { A8 = 40 }; // 赤道颜色
    [Export] public override Color Degree90LongitudeColor { get; set; } = Colors.Orange with { A8 = 40 }; // 东西经 90 度颜色
    [Export] public override Color MeridianColor { get; set; } = Colors.Red with { A8 = 40 }; // 子午线颜色

    [ExportGroup("开关特定线显示")] [Export] public override bool DrawTropicOfCancer { get; set; } = true; // 是否绘制北回归线
    [Export] public override bool DrawTropicOfCapricorn { get; set; } = true; // 是否绘制南回归线
    [Export] public override bool DrawArcticCircle { get; set; } = true; // 是否绘制北极圈
    [Export] public override bool DrawAntarcticCircle { get; set; } = true; // 是否绘制南极圈

    [ExportGroup("透明度设置")]
    [Export(PropertyHint.Range, "0.01, 5")]
    public override float FullVisibilityTime { get; set; } = 1.2f;

    [Export(PropertyHint.Range, "0, 1")] public override float FullVisibility { get; set; } = 0.6f;
    private bool _fixFullVisibility;

    // 是否锁定开启完全显示
    [Export]
    public override bool FixFullVisibility
    {
        get => _fixFullVisibility;
        set
        {
            _fixFullVisibility = value;
            FixFullVisibilitySetter(value);
        }
    }

    #endregion
}