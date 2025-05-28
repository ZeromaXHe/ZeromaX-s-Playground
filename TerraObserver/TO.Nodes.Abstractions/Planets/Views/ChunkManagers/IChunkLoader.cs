using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Planets.Views.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:26:15
public interface IChunkLoader : INode3D
{
    HashSet<int> InsightChunkIdsNow { get; }
    HashSet<int> InsightChunkIdsNext { get; }
    Queue<int> ChunkQueryQueue { get; }
    HashSet<int> VisitedChunkIds { get; }
    HashSet<int> RimChunkIds { get; }
    HashSet<int> UnloadSet { get; }
    HashSet<int> RefreshSet { get; }
    HashSet<int> LoadSet { get; }

    IHexGridChunk InstantiateHexGridChunk();
    void ReSetInsightSetIdx();
    void UpdateInSightSetNextIdx();
    void ClearOldData();
    void OnProcessed(double delta, Action<int> showChunk, Action<int> hideChunk);
}