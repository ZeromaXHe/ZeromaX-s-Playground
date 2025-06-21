using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Godot.Abstractions.Bases;
using TO.Domains.Enums.HexSpheres.Chunks;

namespace TO.Abstractions.Chunks;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 17:50:06
public interface IChunkLoader : INode3D
{
    event Action Processed;
    event Action<IHexGridChunk>? HexGridChunkGenerated;
    void UpdateInsightSetNextIdx();
    HashSet<int> InsightChunkIdsNow { get; }
    HashSet<int> InsightChunkIdsNext { get; }
    Queue<int> ChunkQueryQueue { get; }
    HashSet<int> VisitedChunkIds { get; }
    HashSet<int> RimChunkIds { get; }
    HashSet<int> UnloadSet { get; }
    HashSet<int> RefreshSet { get; }
    HashSet<int> LoadSet { get; }
    Dictionary<int, IHexGridChunk>? UsingChunks { get; }

    Stopwatch Stopwatch { get; }

    void AddUsingChunk(int chunkId, IHexGridChunk chunk);
    bool TryGetUsingChunk(int chunkId, [MaybeNullWhen(false)] out IHexGridChunk chunk);
    IHexGridChunk GetUnusedChunk();
    void HideChunk(int chunkId);
    ChunkLodEnum CalcLod(float distance);
    bool IsChunkInsight(Vector3 chunkPos, Camera3D camera);
}