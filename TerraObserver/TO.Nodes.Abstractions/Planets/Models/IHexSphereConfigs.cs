using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Planets.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-10 07:11:10
public interface IHexSphereConfigs : IResource
{
    event Action? ParamsChanged;
    float Radius { get; set; }
    float StandardScale { get; }
}