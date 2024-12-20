using System;
using FrontEndToolFS.SebastianPlanet;
using Godot;

namespace ZeromaXPlayground.game.SebastianPlanet;

[Tool]
public partial class Planet : PlanetFS
{
    [Export]
    public bool Generate
    {
        get => generate;
        set
        {
            generate = value;
            GeneratePlanet();
        }
    }

    [Export]
    public bool AutoUpdate
    {
        get => autoUpdate;
        set => autoUpdate = value;
    }

    [Export]
    public int Resolution
    {
        get => resolution;
        set => resolution = value;
    }

    [Export]
    public FaceRenderMask FaceRenderMask
    {
        get => faceRenderMask;
        set => faceRenderMask = value;
    }

    [ExportGroup("Color")]
    [Export]
    public Gradient Gradient
    {
        get => colorSettings.gradient;
        set
        {
            colorSettings.gradient = value;
            OnColorSettingsUpdated();
        }
    }

    [Export]
    public ShaderMaterial PlanetMaterial
    {
        get => colorSettings.planetMaterial;
        set
        {
            colorSettings.planetMaterial = value;
            OnColorSettingsUpdated();
        }
    }

    [ExportGroup("Shape")]
    [Export]
    public float PlanetRadius
    {
        get => shapeSettings.planetRadius;
        set
        {
            shapeSettings.planetRadius = value;
            OnShapeSettingsUpdated();
        }
    }

    [ExportSubgroup("Noise")]
    [Export]
    public int LayerCount
    {
        get => shapeSettings.layerCount;
        set
        {
            shapeSettings.layerCount = value;
            RefreshNoiseLayers();
            OnShapeSettingsUpdated();
            NotifyPropertyListChanged();
        }
    }

    // 暂时没想到 Godot 里面怎么实现比较好，用这种方式来约定数组
    private void NoiseLayerSetter<T>(T value, int idx, Action<NoiseSettings, T> settingSetter = null,
        Action<NoiseLayer, T> layerSetter = null)
    {
        var layers = shapeSettings.noiseLayers;
        if (layers == null || layers.Length <= idx) return;
        settingSetter?.Invoke(layers[idx].noiseSettings, value);
        layerSetter?.Invoke(layers[idx], value);
        OnShapeSettingsUpdated();
    }

    private T NoiseLayerGetter<T>(int idx, Func<NoiseSettings, T> settingGetter = null,
        Func<NoiseLayer, T> layerGetter = null)
    {
        var layers = shapeSettings.noiseLayers;
        if (layers == null || layers.Length <= idx) return default;
        if (settingGetter != null) return settingGetter.Invoke(layers[idx].noiseSettings);
        return layerGetter != null ? layerGetter.Invoke(layers[idx]) : default;
    }

    [ExportSubgroup("Noise Layer 0")]
    [Export]
    public bool Enabled0
    {
        get => NoiseLayerGetter(0, null, ns => ns.enabled);
        set => NoiseLayerSetter(value, 0, null, (ns, v) => ns.enabled = v);
    }

    [Export]
    public bool UseFirstLayerAsMask0
    {
        get => NoiseLayerGetter(0, null, ns => ns.useFirstLayerAsMask);
        set => NoiseLayerSetter(value, 0, null, (ns, v) => ns.useFirstLayerAsMask = v);
    }

    [Export]
    public FilterType FilterType0
    {
        get => NoiseLayerGetter(0, ns => ns.filterType);
        set => NoiseLayerSetter(value, 0, (ns, v) => ns.filterType = v);
    }

    [Export]
    public float Strength0
    {
        get => NoiseLayerGetter(0, ns => ns.strength);
        set => NoiseLayerSetter(value, 0, (ns, v) => ns.strength = v);
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int NumLayers0
    {
        get => NoiseLayerGetter(0, ns => ns.numLayers);
        set => NoiseLayerSetter(value, 0, (ns, v) => ns.numLayers = v);
    }

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float Roughness0
    {
        get => NoiseLayerGetter(0, ns => ns.roughness);
        set => NoiseLayerSetter(value, 0, (ns, v) => ns.roughness = v);
    }

    [Export]
    public float BaseRoughness0
    {
        get => NoiseLayerGetter(0, ns => ns.baseRoughness);
        set => NoiseLayerSetter(value, 0, (ns, v) => ns.baseRoughness = v);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Persistence0
    {
        get => NoiseLayerGetter(0, ns => ns.persistence);
        set => NoiseLayerSetter(value, 0, (ns, v) => ns.persistence = v);
    }

    [Export]
    public Vector3 Center0
    {
        get => NoiseLayerGetter(0, ns => ns.center);
        set => NoiseLayerSetter(value, 0, (ns, v) => ns.center = v);
    }

    [Export]
    public float MinValue0
    {
        get => NoiseLayerGetter(0, ns => ns.minValue);
        set => NoiseLayerSetter(value, 0, (ns, v) => ns.minValue = v);
    }

    [Export]
    public float WeightMultiplier0
    {
        get => NoiseLayerGetter(0, ns => ns.weightMultiplier);
        set => NoiseLayerSetter(value, 0, (ns, v) => ns.weightMultiplier = v);
    }

    [ExportSubgroup("Noise Layer 1")]
    [Export]
    public bool Enabled1
    {
        get => NoiseLayerGetter(1, null, ns => ns.enabled);
        set => NoiseLayerSetter(value, 1, null, (ns, v) => ns.enabled = v);
    }

    [Export]
    public bool UseFirstLayerAsMask1
    {
        get => NoiseLayerGetter(1, null, ns => ns.useFirstLayerAsMask);
        set => NoiseLayerSetter(value, 1, null, (ns, v) => ns.useFirstLayerAsMask = v);
    }

    [Export]
    public FilterType FilterType1
    {
        get => NoiseLayerGetter(1, ns => ns.filterType);
        set => NoiseLayerSetter(value, 1, (ns, v) => ns.filterType = v);
    }

    [Export]
    public float Strength1
    {
        get => NoiseLayerGetter(1, ns => ns.strength);
        set => NoiseLayerSetter(value, 1, (ns, v) => ns.strength = v);
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int NumLayers1
    {
        get => NoiseLayerGetter(1, ns => ns.numLayers);
        set => NoiseLayerSetter(value, 1, (ns, v) => ns.numLayers = v);
    }

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float Roughness1
    {
        get => NoiseLayerGetter(1, ns => ns.roughness);
        set => NoiseLayerSetter(value, 1, (ns, v) => ns.roughness = v);
    }

    [Export]
    public float BaseRoughness1
    {
        get => NoiseLayerGetter(1, ns => ns.baseRoughness);
        set => NoiseLayerSetter(value, 1, (ns, v) => ns.baseRoughness = v);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Persistence1
    {
        get => NoiseLayerGetter(1, ns => ns.persistence);
        set => NoiseLayerSetter(value, 1, (ns, v) => ns.persistence = v);
    }

    [Export]
    public Vector3 Center1
    {
        get => NoiseLayerGetter(1, ns => ns.center);
        set => NoiseLayerSetter(value, 1, (ns, v) => ns.center = v);
    }

    [Export]
    public float MinValue1
    {
        get => NoiseLayerGetter(1, ns => ns.minValue);
        set => NoiseLayerSetter(value, 1, (ns, v) => ns.minValue = v);
    }

    [Export]
    public float WeightMultiplier1
    {
        get => NoiseLayerGetter(1, ns => ns.weightMultiplier);
        set => NoiseLayerSetter(value, 1, (ns, v) => ns.weightMultiplier = v);
    }


    [ExportSubgroup("Noise Layer 2")]
    [Export]
    public bool Enabled2
    {
        get => NoiseLayerGetter(2, null, ns => ns.enabled);
        set => NoiseLayerSetter(value, 2, null, (ns, v) => ns.enabled = v);
    }

    [Export]
    public bool UseFirstLayerAsMask2
    {
        get => NoiseLayerGetter(2, null, ns => ns.useFirstLayerAsMask);
        set => NoiseLayerSetter(value, 2, null, (ns, v) => ns.useFirstLayerAsMask = v);
    }

    [Export]
    public FilterType FilterType2
    {
        get => NoiseLayerGetter(2, ns => ns.filterType);
        set => NoiseLayerSetter(value, 2, (ns, v) => ns.filterType = v);
    }

    [Export]
    public float Strength2
    {
        get => NoiseLayerGetter(2, ns => ns.strength);
        set => NoiseLayerSetter(value, 2, (ns, v) => ns.strength = v);
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int NumLayers2
    {
        get => NoiseLayerGetter(2, ns => ns.numLayers);
        set => NoiseLayerSetter(value, 2, (ns, v) => ns.numLayers = v);
    }

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float Roughness2
    {
        get => NoiseLayerGetter(2, ns => ns.roughness);
        set => NoiseLayerSetter(value, 2, (ns, v) => ns.roughness = v);
    }

    [Export]
    public float BaseRoughness2
    {
        get => NoiseLayerGetter(2, ns => ns.baseRoughness);
        set => NoiseLayerSetter(value, 2, (ns, v) => ns.baseRoughness = v);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Persistence2
    {
        get => NoiseLayerGetter(2, ns => ns.persistence);
        set => NoiseLayerSetter(value, 2, (ns, v) => ns.persistence = v);
    }

    [Export]
    public Vector3 Center2
    {
        get => NoiseLayerGetter(2, ns => ns.center);
        set => NoiseLayerSetter(value, 2, (ns, v) => ns.center = v);
    }

    [Export]
    public float MinValue2
    {
        get => NoiseLayerGetter(2, ns => ns.minValue);
        set => NoiseLayerSetter(value, 2, (ns, v) => ns.minValue = v);
    }

    [Export]
    public float WeightMultiplier2
    {
        get => NoiseLayerGetter(2, ns => ns.weightMultiplier);
        set => NoiseLayerSetter(value, 2, (ns, v) => ns.weightMultiplier = v);
    }


    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}