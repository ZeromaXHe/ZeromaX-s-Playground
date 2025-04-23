using Domains.Models.ValueObjects.PlanetGenerates;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 09:45:22
public interface IChunk: INode3D
{
    IHexMesh? GetTerrain();
    IHexMesh? GetRivers();
    IHexMesh? GetRoads();
    IHexMesh? GetWater();
    IHexMesh? GetWaterShore();
    IHexMesh? GetEstuary();
    IHexFeatureManager? GetFeatures();
    HexTileDataOverrider TileDataOverrider { get; set; }

    ChunkLod Lod { get; set; }
}