using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-12 12:46
[Tool]
public partial class ChunkManager : Node3D, IChunkManager
{
    public ChunkManager()
    {
        Context.RegisterToHolder<IChunkManager>(this);
    }

    public NodeEvent? NodeEvent => null;
}