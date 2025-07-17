using Godot;
using TO.Domains.Types.Maps;

namespace TerraObserver.Scenes.Maps.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-05 10:13:42
[Tool]
[GlobalClass]
public partial class NoiseSettings: Resource, INoiseSetting
{
    [Export] public float Strength { get; set; } = 1f;
    [Export] public float SampleRadius { get; set; } = 1f;
    [Export] public FastNoiseLite? Noise { get; set; }

    [Export(PropertyHint.Range, "-1.0, 1.0, 0.01")]
    public float Bias { get; set; }

    [Export] public bool Enabled { get; set; } = true;
    [Export] public bool UseFirstLayerAsMask { get; set; }
}