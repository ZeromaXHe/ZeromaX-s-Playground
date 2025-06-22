using Godot;
using Godot.Abstractions.Bases;

namespace TO.Abstractions.Views.Geos;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-04 10:53:04
public interface ILonLatGrid : INode3D
{
    bool FixFullVisibility { get; set; }
    void OnCameraMoved(Vector3 pos, float delta);
    void Draw(float radius);
}