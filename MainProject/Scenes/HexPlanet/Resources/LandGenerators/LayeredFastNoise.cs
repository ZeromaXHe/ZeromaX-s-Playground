using System;
using System.Linq;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Resources.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-20 20:03:35
[Tool]
[GlobalClass]
public partial class LayeredFastNoise : Resource
{
    [Export] public NoiseSettings[] NoiseLayers { get; set; }

    public float GetLayeredNoise3Dv(Vector3 v) => GetLayeredNoise(x => x.GetNoise3Dv(v));
    public float GetMax() => GetLayeredNoise(x => (1 + x.Bias) * x.Strength);
    public float GetMin() => GetLayeredNoise(x => (-1 + x.Bias) * x.Strength);

    private float GetLayeredNoise(Func<NoiseSettings, float> getNoise3Dv)
    {
        var noiseSum = 0f;
        var firstLayerValue = 0f;
        if (NoiseLayers.Length > 1 && NoiseLayers[0].Enabled)
        {
            firstLayerValue = getNoise3Dv.Invoke(NoiseLayers[0]);
            noiseSum += firstLayerValue;
        }

        for (var i = 1; i < NoiseLayers.Length; i++)
        {
            var noiseLayer = NoiseLayers[i];
            if (NoiseLayers == null || !noiseLayer.Enabled)
                continue;
            var mask = noiseLayer.UseFirstLayerAsMask ? firstLayerValue / NoiseLayers[0].Strength : 1f;
            noiseSum += getNoise3Dv.Invoke(noiseLayer) * mask;
        }

        return noiseSum;
    }
}