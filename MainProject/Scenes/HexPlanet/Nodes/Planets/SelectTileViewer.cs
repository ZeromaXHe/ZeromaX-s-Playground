using Contexts;
using Domains.Models.Entities.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions.Planets;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-27 11:26:48
public partial class SelectTileViewer : MeshInstance3D, ISelectTileViewer
{
    public SelectTileViewer()
    {
        Context.RegisterToHolder<ISelectTileViewer>(this);
    }

    public NodeEvent? NodeEvent => null;

    public int EditingTileId { get; private set; }
    public void SelectEditingTile(Tile tile) => EditingTileId = tile.Id;
    public void CleanEditingTile() => EditingTileId = 0;
}