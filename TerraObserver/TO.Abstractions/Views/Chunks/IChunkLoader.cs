using System.Diagnostics;
using Godot.Abstractions.Bases;

namespace TO.Abstractions.Views.Chunks;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 17:50:06
public interface IChunkLoader : INode3D
{
    #region 事件

    event Action Processed;
    event Action<IHexGridChunk>? HexGridChunkGenerated;

    #endregion 事件

    #region 普通属性

    HashSet<int> InsightChunkIdsNow { get; }
    HashSet<int> InsightChunkIdsNext { get; }
    Queue<int> ChunkQueryQueue { get; }
    HashSet<int> VisitedChunkIds { get; }
    HashSet<int> RimChunkIds { get; }
    HashSet<int> UnloadSet { get; }
    HashSet<int> RefreshSet { get; }
    HashSet<int> LoadSet { get; }
    Dictionary<int, IHexGridChunk>? UsingChunks { get; }
    Queue<IHexGridChunk>? UnusedChunks { get; }

    Stopwatch Stopwatch { get; }

    #endregion

    void ResetInsightSetIdx();
    void UpdateInsightSetNextIdx();
    IHexGridChunk GetUnusedChunk();
}