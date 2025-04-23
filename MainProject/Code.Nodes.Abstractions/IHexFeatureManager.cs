using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:32:17
public interface IHexFeatureManager : INode3D
{
    void Clear();
    void Apply();
    void ShowFeatures(bool onlyExplored);
    void HideFeatures(bool onlyUnexplored);
    void AddBridge(Tile tile, Vector3 roadCenter1, Vector3 roadCenter2);
    void AddSpecialFeature(Tile tile, Vector3 position, HexTileDataOverrider overrider);
    void AddFeature(Tile tile, Vector3 position, HexTileDataOverrider overrider);

    void AddWall(EdgeVertices near, Tile nearTile, EdgeVertices far, Tile farTile,
        bool hasRiver, bool hasRoad, HexTileDataOverrider overrider, ChunkLod lod);

    void AddWall(Vector3 c1, Tile tile1, Vector3 c2, Tile tile2, Vector3 c3,
        Tile tile3, HexTileDataOverrider overrider, ChunkLod lod);
}