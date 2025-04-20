using Domains.Models.Entities.PlanetGenerates;
using Godot;

namespace Domains.Services.Abstractions.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:05:18
public interface IChunkLoaderService
{
    void OnProcessed(double delta);
#if !FEATURE_NEW
    void ExploreFeatures(Tile tile);
#endif
    void UpdateInsightChunks(Transform3D transform, float delta);
    void InitChunkNodes();
}