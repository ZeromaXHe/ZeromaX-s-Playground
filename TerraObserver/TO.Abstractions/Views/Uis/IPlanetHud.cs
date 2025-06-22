using Godot;
using Godot.Abstractions.Bases;

namespace TO.Abstractions.Views.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 20:01:22
public interface IPlanetHud : IControl
{
    PanelContainer? CompassPanel { get; }
    TextureRect? RectMap { get; }
    VSlider? ElevationVSlider { get; }
    VSlider? WaterVSlider { get; }
}