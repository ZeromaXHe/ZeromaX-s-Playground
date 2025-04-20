using Godot;

namespace GodotNodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 16:51:16
public interface INode3D: INode
{
    World3D GetWorld3D();
    Vector3 ToLocal(Vector3 globalPoint);
    Vector3 ToGlobal(Vector3 localPoint);
}