using Godot;
using TO.GodotNodes.Abstractions;

namespace TO.Nodes.Abstractions.Planets.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-10 13:28:10
public interface ICatlikeCodingNoise: IResource
{
    Image? NoiseSourceImage { get; }
}