namespace Godot.Abstractions.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 16:49:16
public interface INode
{
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
    void QueueFree();
}