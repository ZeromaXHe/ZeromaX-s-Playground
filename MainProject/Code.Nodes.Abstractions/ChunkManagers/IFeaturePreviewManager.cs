using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:27:17
public interface IFeaturePreviewManager : INode3D
{
    Material? UrbanPreviewOverrideMaterial { get; }
    Material? PlantPreviewOverrideMaterial { get; }
    Material? FarmPreviewOverrideMaterial { get; }
    int PreviewCount { get; set; }
    HashSet<int> EmptyPreviewIds { get; }
    void ClearForData();
}