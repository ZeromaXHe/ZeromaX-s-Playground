using Domains.Models.Entities.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:25:17
public interface ISelectTileViewer : IMeshInstance3D
{
    int EditingTileId { get; }
    void CleanEditingTile();
    void SelectEditingTile(Tile chosenTile);
}