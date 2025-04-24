using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:37:17
public interface ILongitudeLatitude : INode3D
{
    event Action<bool>? FixFullVisibilityChanged;
    void OnCameraMoved(Vector3 pos, float delta);
}