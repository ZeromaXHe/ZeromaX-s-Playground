using Godot;
using TO.Domains.Types.PlanetHuds;

namespace TerraObserver.Scenes.Uis.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-01 09:30:51
public partial class SelectTileViewer : MeshInstance3D, ISelectTileViewer
{
    public int? HoverTileId { get; set; }
    public int? SelectedTileId { get; set; }
}