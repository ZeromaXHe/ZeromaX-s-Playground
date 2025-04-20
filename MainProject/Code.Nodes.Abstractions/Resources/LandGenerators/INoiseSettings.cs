using Godot;

namespace Nodes.Abstractions.Resources.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-20 21:44:20
public interface INoiseSettings
{
    float Strength { get; }
    float SampleRadius { get; }
    FastNoiseLite? Noise { get; }
    float Bias { get; }
    bool Enabled { get; }
    bool UseFirstLayerAsMask { get; }
}