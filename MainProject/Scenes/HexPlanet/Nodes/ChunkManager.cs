using System.Diagnostics.CodeAnalysis;
using Apps.Queries.Contexts;
using Contexts;
using Domains.Services.Abstractions.Nodes.ChunkManagers;
using Godot;
using GodotNodes.Abstractions.Addition;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
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
        NodeContext.Instance.RegisterSingleton<IChunkManager>(this);
        Context.RegisterToHolder<IChunkManager>(this);
    }

    private bool _ready;

    public NodeEvent? NodeEvent => null;

    public override void _Ready()
    {
        _ready = true;
    }

    public override void _ExitTree()
    {
        _ready = false;
        NodeContext.Instance.DestroySingleton<IChunkManager>();
    }
}