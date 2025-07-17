using Godot;
using TO.Domains.Types.Maps;

namespace TerraObserver.Scenes.Maps.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-05 10:12:15
[Tool]
[GlobalClass]
public partial class FractalNoiseLandGenerator : LandGenerator, IFractalNoiseLandGenerator
{
    [Export] public LayeredFastNoise LayeredNoises { get; set; } = new();
    public ILayeredFastNoise GetLayeredNoises() => LayeredNoises;
}