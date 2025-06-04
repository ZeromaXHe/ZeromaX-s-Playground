namespace Godot.Abstractions.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 14:15:17
public interface IMeshInstance3D : IGeometryInstance3D
{
    Mesh? Mesh { get; set; }
}