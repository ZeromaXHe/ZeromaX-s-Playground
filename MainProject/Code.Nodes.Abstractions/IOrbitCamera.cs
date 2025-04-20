using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:36:17
public interface IOrbitCamera : INode3D
{
    delegate void MovedEvent(Vector3 pos, float delta);

    event MovedEvent? Moved;

    delegate void TransformedEvent(Transform3D transform, float delta);

    event TransformedEvent? Transformed;
    Vector3 GetFocusBasePos();
}