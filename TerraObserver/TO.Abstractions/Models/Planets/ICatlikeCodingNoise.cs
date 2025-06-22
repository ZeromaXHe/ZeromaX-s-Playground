using Godot;
using Godot.Abstractions.Bases;

namespace TO.Abstractions.Models.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 17:20:06
public interface ICatlikeCodingNoise : IResource
{
    Image? NoiseSourceImage { get; }
}