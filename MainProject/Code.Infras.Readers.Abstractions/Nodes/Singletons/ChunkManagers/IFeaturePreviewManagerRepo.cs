using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions.ChunkManagers;

namespace Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:28:18
public interface IFeaturePreviewManagerRepo : ISingletonNodeRepo<IFeaturePreviewManager>
{
    void OnHideFeature(int id, FeatureType type);
    int OnShowFeature(Transform3D transform, FeatureType type, Mesh mesh);
}