using System;
using TO.Abstractions.Views.Uis;
using TO.Presenters.Views.Uis;

namespace TerraObserver.Scenes.Uis.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-05 19:08:02
public partial class PlanetHud : PlanetHudFS, IPlanetHud
{
    #region 生命周期

    // 需要忽略 IDE 省略 partial、_Ready 等的提示，必须保留它们
    public override void _Ready() => base._Ready();

    #endregion

    #region 事件

    public event Action<int?>? ChosenTileIdChanged;
    public override void EmitChosenTileIdChanged(int? v) => ChosenTileIdChanged?.Invoke(v);
    public event Action<bool>? CelestialMotionCheckButtonToggled;
    public override void EmitCelestialMotionCheckButtonToggled(bool v) => CelestialMotionCheckButtonToggled?.Invoke(v);
    public event Action<bool>? LonLatFixCheckButtonToggled;
    public override void EmitLonLatFixCheckButtonToggled(bool v) => LonLatFixCheckButtonToggled?.Invoke(v);
    public event Action<string>? RadiusLineEditTextSubmitted;
    public override void EmitRadiusLineEditTextSubmitted(string v) => RadiusLineEditTextSubmitted?.Invoke(v);
    public event Action<string>? DivisionLineEditTextSubmitted;
    public override void EmitDivisionLineEditTextSubmitted(string v) => DivisionLineEditTextSubmitted?.Invoke(v);
    public event Action<string>? ChunkDivisionLineEditTextSubmitted;

    public override void EmitChunkDivisionLineEditTextSubmitted(string v) =>
        ChunkDivisionLineEditTextSubmitted?.Invoke(v);

    #endregion
}