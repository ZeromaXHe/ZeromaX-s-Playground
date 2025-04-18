using Godot;

namespace GodotNodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 16:49:16
public interface INode
{
    event Action Ready;
    Viewport GetViewport();
}