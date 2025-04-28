using System.Diagnostics.CodeAnalysis;
using Domains.Models.Entities.PlanetGenerates;
using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Abstractions.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 11:08:22
public interface IHexGridChunkRepo : IIdInstanceNodeRepo<IHexGridChunk>
{
    bool IsChunkUsing(int chunkId);
    bool TryGetUsingChunk(int chunkId, [MaybeNullWhen(false)] out IHexGridChunk chunk);
    IEnumerable<IHexGridChunk> GetAllUsingChunk();
    void AddUsingChunk(int chunkId, IHexGridChunk chunk);
    void RemoveUsingChunk(int chunkId);
    bool NoUnusedChunk();
    IHexGridChunk DequeUnusedChunk();
    void EnqueueUnusedChunks(IHexGridChunk chunk);
    void OnChunkServiceRefreshChunkTileLabel(int chunkId, int tileId, string text);
    void ClearOldData();
}