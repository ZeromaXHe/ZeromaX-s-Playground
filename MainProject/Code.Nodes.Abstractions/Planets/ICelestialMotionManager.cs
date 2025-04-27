using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:23:17
public interface ICelestialMotionManager : INode3D
{
    event Action<float>? SatelliteDistRatioChanged;
    event Action<float>? SatelliteRadiusRatioChanged;
    event Action? StarMoveStatusToggled;
    event Action? PlanetMoveStatusToggled;
    event Action? SatelliteMoveStatusToggled;

    #region export 变量

    float RotationTimeFactor { get; }
    float PlanetRevolutionSpeed { get; }
    float PlanetRotationSpeed { get; }
    float SatelliteRadiusRatio { get; }
    float SatelliteDistRatio { get; }
    float SatelliteRevolutionSpeed { get; }
    float SatelliteRotationSpeed { get; }

    #endregion

    #region on-ready 节点

    Node3D? EclipticPlane { get; }
    Node3D? SunRevolution { get; }
    Node3D? PlanetAxis { get; }
    Node3D? LunarOrbitPlane { get; }
    Node3D? LunarRevolution { get; }
    Node3D? LunarDist { get; }
    Node3D? LunarObliquity { get; }
    Node3D? MoonAxis { get; }
    MeshInstance3D? MoonMesh { get; }
    MeshInstance3D? SunMesh { get; }

    #endregion
}