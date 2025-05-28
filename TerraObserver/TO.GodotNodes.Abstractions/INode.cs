using Godot;

namespace TO.GodotNodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 16:49:16
public interface INode : IGodotObject
{
    event Action? Ready; // 2208
    event Action? TreeExiting; // 2252
    StringName Name { get; set; }
    NodePath GetPath();
    void SetProcess(bool enable);
    bool IsNodeReady();
    void AddChild(Node node, bool forceReadableName = false, Node.InternalMode @internal = (Node.InternalMode)(0));
    Node GetChild(int idx, bool includeInternal = false);
    Godot.Collections.Array<Node> GetChildren(bool includeInternal = false);
    Viewport GetViewport();
    void QueueFree();
}