using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Nodes.Abstractions.ChunkManagers;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:31:17
public interface IEditPreviewChunk : IChunk
{
    ShaderMaterial[]? TerrainMaterials { get; }
    void Refresh(HexTileDataOverrider tileDataOverrider, IEnumerable<Tile> tiles);
}