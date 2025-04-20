using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.ChunkManagers;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;

namespace Domains.Services.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:13:43
public class FeaturePreviewManagerService(
    IFeaturePreviewManagerRepo featurePreviewManagerRepo,
    IFeatureMeshManagerRepo featureMeshManagerRepo) : IFeaturePreviewManagerService
{
    public int OnShowFeature(Transform3D transform, FeatureType type) =>
        featureMeshManagerRepo.IsRegistered()
            ? featurePreviewManagerRepo.OnShowFeature(transform, type, featureMeshManagerRepo.GetMultiMesh(type).Mesh)
            : -1;
}