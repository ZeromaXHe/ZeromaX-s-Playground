using Godot;

namespace TerraObserver.Scenes.Planets.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-10 08:17:16
[GlobalClass]
[Tool]
public partial class CelestialMotionConfigs : Resource
{
    // 行星公转
    [Export] public bool PlanetRevolution { get; set; } = true;

    // 行星自转
    [Export] public bool PlanetRotation { get; set; } = true;

    // 卫星公转
    [Export] public bool SatelliteRevolution { get; set; } = true;

    // 卫星自转
    [Export] public bool SatelliteRotation { get; set; } = true;
}