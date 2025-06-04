using Godot.Abstractions.Bases;

namespace Godot.Abstractions.Extensions.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-03 17:38:03
public interface IPlanet : INode3D
{
    float Radius { get; }
    int Divisions { get; }
    int ChunkDivisions { get; }
    float MaxHeight { get; }
    float MaxHeightRatio { get; }
    float StandardScale { get; }
}