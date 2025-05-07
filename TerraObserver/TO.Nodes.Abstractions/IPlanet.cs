using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 21:27:28
public interface IPlanet : INode3D
{
    event Action<double>? Processed;
    event Action<float, float>? RadiusChanged;
    float Radius { get; set; }
}