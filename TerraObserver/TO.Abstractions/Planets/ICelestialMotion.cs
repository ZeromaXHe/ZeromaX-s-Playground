using Godot.Abstractions.Bases;

namespace TO.Abstractions.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-05 19:50:05
public interface ICelestialMotion : INode3D
{
    bool PlanetRevolution { get; set; }
    bool PlanetRotation { get; set; }
    bool SatelliteRevolution { get; set; }
    bool SatelliteRotation { get; set; }
}