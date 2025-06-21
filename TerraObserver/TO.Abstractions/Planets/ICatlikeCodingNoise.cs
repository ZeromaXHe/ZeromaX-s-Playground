using Godot;
using Godot.Abstractions.Bases;
using TO.Domains.Components.HexSpheres.Tiles;

namespace TO.Abstractions.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 17:20:06
public interface ICatlikeCodingNoise : IResource
{
    Vector3 Perturb(Vector3 position);
    float GetHeight(TileValue tileValue, TileUnitCentroid tileUnitCentroid);
}