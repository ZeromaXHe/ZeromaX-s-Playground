using Domains.Models.Entities.PlanetGenerates;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:26:15
public interface IChunkLoader : INode3D
{
    Dictionary<int, IHexGridChunk> UsingChunks { get; }
    Queue<IHexGridChunk> UnusedChunks { get; }
    HashSet<int> InsightChunkIdsNow { get; }
    HashSet<int> InsightChunkIdsNext { get; }
    Queue<int> ChunkQueryQueue { get; }
    HashSet<int> VisitedChunkIds { get; }
    HashSet<int> RimChunkIds { get; }
    HashSet<int> UnloadSet { get; }
    HashSet<int> RefreshSet { get; }
    HashSet<int> LoadSet { get; }
    void OnChunkServiceRefreshChunk(int id);
    void OnChunkServiceRefreshChunkTileLabel(int chunkId, int tileId, string text);
    void ReSetInsightSetIdx();
    void UpdateInSightSetNextIdx();
    void ShowChunk(Chunk chunk);
    void HideChunk(int chunkId);
    void ClearOldData();

    // TODO: 提取成多例 Repo 自己的方法
    void ExploreChunkFeatures(int chunkId, int tileId);
}