using Domains.Models.Entities.PlanetGenerates;
using Nodes.Abstractions.ChunkManagers;

namespace Domains.Services.Abstractions.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 09:31:22
public interface IChunkTriangulationService
{
    void Triangulate(Tile tile, IChunk chunk);
}