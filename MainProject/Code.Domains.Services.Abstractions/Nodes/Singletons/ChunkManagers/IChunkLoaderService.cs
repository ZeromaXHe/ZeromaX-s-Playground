using Domains.Models.Entities.PlanetGenerates;
using Godot;

namespace Domains.Services.Abstractions.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:05:18
public interface IChunkLoaderService
{
    void UpdateInsightChunks(Transform3D transform, float delta);
    void InitChunkNodes();
}