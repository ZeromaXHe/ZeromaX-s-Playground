using System.Collections.Generic;
using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions.ChunkManagers;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-26 21:28:04
[Tool]
public partial class FeaturePreviewManager : Node3D, IFeaturePreviewManager
{
    public FeaturePreviewManager()
    {
        Context.RegisterToHolder<IFeaturePreviewManager>(this);
    }

    public NodeEvent? NodeEvent => null;

    #region export 变量

    [Export] public Material? UrbanPreviewOverrideMaterial { get; private set; }
    [Export] public Material? PlantPreviewOverrideMaterial { get; private set; }
    [Export] public Material? FarmPreviewOverrideMaterial { get; private set; }

    #endregion

    public void ClearForData()
    {
        PreviewCount = 0;
        EmptyPreviewIds.Clear();
        foreach (var child in GetChildren())
            child.QueueFree();
    }

    public int PreviewCount { get; set; }
    public HashSet<int> EmptyPreviewIds { get; } = [];
}