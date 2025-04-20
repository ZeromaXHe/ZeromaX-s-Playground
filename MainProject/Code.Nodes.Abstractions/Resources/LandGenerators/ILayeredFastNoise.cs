using Godot;

namespace Nodes.Abstractions.Resources.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-20 21:44:20
public interface ILayeredFastNoise
{
    float GetLayeredNoise3Dv(Vector3 v);
}