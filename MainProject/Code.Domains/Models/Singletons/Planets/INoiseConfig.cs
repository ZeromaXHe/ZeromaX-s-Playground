using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Models.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-12 07:52
public interface INoiseConfig
{
    Image? NoiseSource { get; set; }

    float ElevationPerturbStrength { get; }

    // 球面噪声采样逻辑
    Vector4 SampleNoise(Vector3 position);

    // 球面的扰动逻辑
    Vector3 Perturb(Vector3 position);
    void InitializeHashGrid(ulong seed);
    HexHash SampleHashGrid(Vector3 position);
}