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

    #region export 变量

    float SatelliteRadiusRatio { get; }
    float SatelliteDistRatio { get; }

    #endregion

    #region on-ready 节点

    Node3D? LunarDist { get; }
    MeshInstance3D? MoonMesh { get; }

    #endregion
}