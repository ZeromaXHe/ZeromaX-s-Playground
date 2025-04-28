using Domains.Models.Entities.PlanetGenerates;

namespace Domains.Services.Abstractions.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 17:36:22
public interface IHexGridChunkService
{
    void RefreshChunk(int id);
    void OnEditorEditModeChanged(bool mode);
    void ShowFeatures(Tile tile, bool onlyExplored, bool preview);
    void HideFeatures(Tile tile, bool preview);
    void ClearFeatures(Tile tile, bool preview);
    void ExploreFeatures(Tile tile);
    void RefreshTilesLabelMode(int mode);
    void ShowChunk(int chunkId);
    void HideChunk(int chunkId);
    bool IsHandlingLodGaps(Chunk chunk);
}