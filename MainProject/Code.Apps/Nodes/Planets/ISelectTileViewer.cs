using Domains.Models.Entities.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions;

namespace Apps.Nodes.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:25:17
public interface ISelectTileViewer : IMeshInstance3D
{
    void Update(int pathFromTileId, Vector3 position);
    void CleanEditingTile();
    void SelectEditingTile(Tile chosenTile);
}