using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:34:17
public interface IHexTileLabel : INode3D
{
    Label3D? Label { get;}
}