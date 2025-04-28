using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Nodes.Abstractions.ChunkManagers;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:33:17
public interface IHexGridChunk : IChunk
{
    int Id { get; set; }
    Dictionary<int, IHexTileLabel> UsingTileUis { get; }
    Queue<IHexTileLabel> UnusedTileUis { get; }

    IHexTileLabel InstantiateHexTileLabel();
    void RefreshTileLabel(int tileId, string text);
}