using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:26:17
public interface IFeatureMeshManager : INode3D
{
    MultiMeshInstance3D[]? MultiUrbans { get; }
    MultiMeshInstance3D[]? MultiFarms { get; }
    MultiMeshInstance3D[]? MultiPlants { get; }
    MultiMeshInstance3D? MultiTowers { get; }
    MultiMeshInstance3D? MultiBridges { get; }
    MultiMeshInstance3D[]? MultiSpecials { get; }

    Dictionary<FeatureType, HashSet<int>> HidingIds { get; }

    void InitMultiMeshInstances();
    void ClearOldData();
}