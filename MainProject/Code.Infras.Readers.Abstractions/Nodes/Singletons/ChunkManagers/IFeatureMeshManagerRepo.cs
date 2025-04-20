using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions.ChunkManagers;

namespace Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:26:18
public interface IFeatureMeshManagerRepo : ISingletonNodeRepo<IFeatureMeshManager>
{
    void OnHideFeature(int id, FeatureType type);
    int OnShowFeature(Transform3D transform, FeatureType type);
    MultiMesh GetMultiMesh(FeatureType type);
}