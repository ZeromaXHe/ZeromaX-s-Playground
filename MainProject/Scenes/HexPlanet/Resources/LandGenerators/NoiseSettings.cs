using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Resources.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-20 20:13:02
[Tool]
[GlobalClass]
public partial class NoiseSettings : Resource
{
    [Export] public float Strength { get; set; } = 1f;
    [Export] public float SampleRadius { get; set; } = 1f;
    [Export] public FastNoiseLite? Noise { get; set; }

    [Export(PropertyHint.Range, "-1.0, 1.0, 0.01")]
    public float Bias;

    [Export] public bool Enabled { get; set; } = true;
    [Export] public bool UseFirstLayerAsMask { get; set; }

    public float GetNoise3Dv(Vector3 v)
    {
        if (Noise == null) return Bias;
        var noiseValue = Noise.GetNoise3Dv(v * SampleRadius);
        return (noiseValue + Bias) * Strength;
    }
}