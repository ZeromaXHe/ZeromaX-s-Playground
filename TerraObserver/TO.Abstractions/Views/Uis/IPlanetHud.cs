using Godot;
using Godot.Abstractions.Bases;

namespace TO.Abstractions.Views.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 20:01:22
public interface IPlanetHud : IControl
{
    #region on-ready

    Label CamLonLatLabel { get; }
    PanelContainer CompassPanel { get; }
    TextureRect RectMap { get; }
    LineEdit RadiusLineEdit { get; }
    LineEdit DivisionLineEdit { get; }

    LineEdit ChunkDivisionLineEdit { get; }
    VSlider ElevationVSlider { get; }
    VSlider WaterVSlider { get; }

    #endregion
}