using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-28 16:30
[Tool]
public partial class HexTileLabel : Node3D, IHexTileLabel
{
    public NodeEvent? NodeEvent => null;
    public Label3D? Label { get; private set; }

    public override void _Ready()
    {
        Label = GetNode<Label3D>("%Label");
    }
}