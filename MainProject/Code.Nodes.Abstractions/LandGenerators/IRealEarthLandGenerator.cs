using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:30:17
public interface IRealEarthLandGenerator : INode
{
    Texture2D? WorldMap { get; }
}