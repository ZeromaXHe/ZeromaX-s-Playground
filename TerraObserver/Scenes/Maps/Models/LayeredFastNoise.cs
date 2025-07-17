using Godot;
using TO.Domains.Types.Maps;

namespace TerraObserver.Scenes.Maps.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-05 10:14:34
[Tool]
[GlobalClass]
public partial class LayeredFastNoise : Resource, ILayeredFastNoise
{
    [Export] public NoiseSettings[]? NoiseLayers { get; set; }
    public INoiseSetting? GetNoiseLayerByIdx(int idx) => NoiseLayers?[idx];
    public int GetNoiseLayersLength() => NoiseLayers?.Length ?? 0;
}