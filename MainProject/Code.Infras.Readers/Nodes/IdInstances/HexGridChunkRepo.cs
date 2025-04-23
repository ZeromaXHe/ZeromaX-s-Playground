using System.Diagnostics.CodeAnalysis;
using Infras.Readers.Abstractions.Nodes.IdInstances;
using Infras.Readers.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 14:13:08
public class HexGridChunkRepo : IdInstanceNodeRepo<IHexGridChunk>, IHexGridChunkRepo
{
    private Dictionary<int, IHexGridChunk>? UsingChunks { get; } = new();
    private Queue<IHexGridChunk>? UnusedChunks { get; } = new();

    public bool IsChunkUsing(int chunkId) => UsingChunks?.ContainsKey(chunkId) ?? false;

    public bool TryGetUsingChunk(int chunkId, [MaybeNullWhen(false)] out IHexGridChunk chunk)
    {
        if (UsingChunks?.TryGetValue(chunkId, out chunk) ?? false)
            return true;
        chunk = null;
        return false;
    }

    public IEnumerable<IHexGridChunk> GetAllUsingChunk() => UsingChunks?.Values ?? Enumerable.Empty<IHexGridChunk>();
    public void AddUsingChunk(int chunkId, IHexGridChunk chunk) => UsingChunks!.Add(chunkId, chunk);
    public void RemoveUsingChunk(int chunkId) => UsingChunks!.Remove(chunkId);

    public bool NoUnusedChunk() => UnusedChunks is null || UnusedChunks.Count == 0;

    public IHexGridChunk DequeUnusedChunk() => UnusedChunks!.Dequeue();
    public void EnqueueUnusedChunks(IHexGridChunk chunk) => UnusedChunks!.Enqueue(chunk);

#if !FEATURE_NEW
    public void ExploreChunkFeatures(int chunkId, int tileId)
    {
        if (TryGetUsingChunk(chunkId, out var chunk))
            chunk.ExploreFeatures(tileId);
    }
#endif

    public void OnChunkServiceRefreshChunkTileLabel(int chunkId, int tileId, string text)
    {
        if (TryGetUsingChunk(chunkId, out var chunk))
            chunk.RefreshTileLabel(tileId, text);
    }

    public void ClearOldData()
    {
        UsingChunks?.Clear();
        UnusedChunks?.Clear();
    }
}