using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Services.Abstractions.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:06:18
public interface IFeaturePreviewManagerService
{
    int OnShowFeature(Transform3D transform, FeatureType type);
}