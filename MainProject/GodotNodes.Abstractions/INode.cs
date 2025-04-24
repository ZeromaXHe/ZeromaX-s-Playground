using Godot;
using GodotNodes.Abstractions.Addition;

namespace GodotNodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 16:49:16
public interface INode
{
    // 为了事件驱动而加的自定义节点事件，Godot 本身 Node 类里面其实没有这个，需要自己实现
    // 没有用到相关事件时，为 null
    NodeEvent? NodeEvent { get; }
    event Action? Ready;
    event Action? TreeExiting;
    StringName Name { get; set; }
    NodePath GetPath();
    void SetProcess(bool enable);
    bool IsNodeReady();
    void AddChild(Node node, bool forceReadableName = false, Node.InternalMode @internal = (Node.InternalMode)(0));
    Node GetChild(int idx, bool includeInternal = false);
    Godot.Collections.Array<Node> GetChildren(bool includeInternal = false);
    Viewport GetViewport();
}