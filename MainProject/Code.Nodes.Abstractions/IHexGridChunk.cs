using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:33:17
public interface IHexGridChunk : INode3D
{
    void Refresh();
    void ExploreFeatures(int tileId);
    void RefreshTileLabel(int tileId, string text);
    void UpdateLod(ChunkLod lod, bool idChanged = true);
    void UsedBy(Chunk chunk);
    void HideOutOfSight();
}