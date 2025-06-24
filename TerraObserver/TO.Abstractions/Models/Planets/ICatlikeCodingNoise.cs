using Godot;
using Godot.Abstractions.Bases;
using TO.Domains.Structs.Tiles;

namespace TO.Abstractions.Models.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 17:20:06
public interface ICatlikeCodingNoise : IResource
{
    Image? NoiseSourceImage { get; }
    int HashGridSize { get; }
    HexHash[] HashGrid { get; }
    RandomNumberGenerator Rng { get; }
}