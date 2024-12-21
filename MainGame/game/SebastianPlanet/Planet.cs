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
    public ShaderMaterial PlanetMaterial
    {
        get => colorSettings.planetMaterial;
        set
        {
            colorSettings.planetMaterial = value;
            OnColorSettingsUpdated();
        }
    }

    [Export]
    public float CbNoiseOffset
    {
        get => colorSettings.biomeColorSettings.noiseOffset;
        set
        {
            colorSettings.biomeColorSettings.noiseOffset = value;
            OnColorSettingsUpdated();
        }
    }

    [Export]
    public float CbNoiseStrength
    {
        get => colorSettings.biomeColorSettings.noiseStrength;
        set
        {
            colorSettings.biomeColorSettings.noiseStrength = value;
            OnColorSettingsUpdated();
        }
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float CbBlendAmount
    {
        get => colorSettings.biomeColorSettings.blendAmount;
        set
        {
            colorSettings.biomeColorSettings.blendAmount = value;
            OnColorSettingsUpdated();
        }
    }

    private NoiseSettings GetBiomeColorNoise() => colorSettings.biomeColorSettings.noise;

    [ExportSubgroup("Biome Color Noise")]
    [Export]
    public FilterType BcnFilterType
    {
        get => GetBiomeColorNoise().filterType;
        set
        {
            GetBiomeColorNoise().filterType = value;
            OnColorSettingsUpdated();
        }
    }

    [Export]
    public float BcnStrength
    {
        get => GetBiomeColorNoise().strength;
        set
        {
            GetBiomeColorNoise().strength = value;
            OnColorSettingsUpdated();
        }
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int BcnNumLayers
    {
        get => GetBiomeColorNoise().numLayers;
        set
        {
            GetBiomeColorNoise().numLayers = value;
            OnColorSettingsUpdated();
        }
    }

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float BcnRoughness
    {
        get => GetBiomeColorNoise().roughness;
        set
        {
            GetBiomeColorNoise().roughness = value;
            OnColorSettingsUpdated();
        }
    }

    [Export]
    public float BcnBaseRoughness
    {
        get => GetBiomeColorNoise().baseRoughness;
        set
        {
            GetBiomeColorNoise().baseRoughness = value;
            OnColorSettingsUpdated();
        }
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float BcnPersistence
    {
        get => GetBiomeColorNoise().persistence;
        set
        {
            GetBiomeColorNoise().persistence = value;
            OnColorSettingsUpdated();
        }
    }

    [Export]
    public Vector3 BcnCenter
    {
        get => GetBiomeColorNoise().center;
        set
        {
            GetBiomeColorNoise().center = value;
            OnColorSettingsUpdated();
        }
    }

    [Export]
    public float BcnMinValue
    {
        get => GetBiomeColorNoise().minValue;
        set
        {
            GetBiomeColorNoise().minValue = value;
            OnColorSettingsUpdated();
        }
    }

    [Export]
    public float BcnWeightMultiplier
    {
        get => GetBiomeColorNoise().weightMultiplier;
        set
        {
            GetBiomeColorNoise().weightMultiplier = value;
            OnColorSettingsUpdated();
        }
    }

    // 暂时没想到 Godot 里面怎么实现比较好，用这种方式来约定数组
    private void Setter<V, T>(T[] arr, V value, int idx, Action<T, V> setter, bool colorUpdate = false,
        bool shapeUpdate = false)
    {
        if (arr == null || arr.Length <= idx) return;
        setter.Invoke(arr[idx], value);
        if (shapeUpdate) OnShapeSettingsUpdated();
        if (colorUpdate) OnColorSettingsUpdated();
    }

    private V Getter<V, T>(T[] arr, int idx, Func<T, V> getter)
    {
        if (arr == null || arr.Length <= idx) return default;
        return getter != null ? getter.Invoke(arr[idx]) : default;
    }

    private Biome[] GetBiomes() => colorSettings.biomeColorSettings.biomes;

    [ExportSubgroup("Biome 0")]
    [Export]
    public Gradient Gradient0
    {
        get => Getter(GetBiomes(), 0, b => b.gradient);
        set => Setter(GetBiomes(), value, 0, (b, v) => b.gradient = v, true);
    }

    [Export]
    public Color Tint0
    {
        get => Getter(GetBiomes(), 0, b => b.tint);
        set => Setter(GetBiomes(), value, 0, (b, v) => b.tint = v, true);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float StartHeight0
    {
        get => Getter(GetBiomes(), 0, b => b.startHeight);
        set => Setter(GetBiomes(), value, 0, (b, v) => b.startHeight = v, true);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float TintPercent0
    {
        get => Getter(GetBiomes(), 0, b => b.tintPercent);
        set => Setter(GetBiomes(), value, 0, (b, v) => b.tintPercent = v, true);
    }

    [ExportSubgroup("Biome 1")]
    [Export]
    public Gradient Gradient1
    {
        get => Getter(GetBiomes(), 1, b => b.gradient);
        set => Setter(GetBiomes(), value, 1, (b, v) => b.gradient = v, true);
    }

    [Export]
    public Color Tint1
    {
        get => Getter(GetBiomes(), 1, b => b.tint);
        set => Setter(GetBiomes(), value, 1, (b, v) => b.tint = v, true);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float StartHeight1
    {
        get => Getter(GetBiomes(), 1, b => b.startHeight);
        set => Setter(GetBiomes(), value, 1, (b, v) => b.startHeight = v, true);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float TintPercent1
    {
        get => Getter(GetBiomes(), 1, b => b.tintPercent);
        set => Setter(GetBiomes(), value, 1, (b, v) => b.tintPercent = v, true);
    }

    [ExportSubgroup("Biome 2")]
    [Export]
    public Gradient Gradient2
    {
        get => Getter(GetBiomes(), 2, b => b.gradient);
        set => Setter(GetBiomes(), value, 2, (b, v) => b.gradient = v, true);
    }

    [Export]
    public Color Tint2
    {
        get => Getter(GetBiomes(), 2, b => b.tint);
        set => Setter(GetBiomes(), value, 2, (b, v) => b.tint = v, true);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float StartHeight2
    {
        get => Getter(GetBiomes(), 2, b => b.startHeight);
        set => Setter(GetBiomes(), value, 2, (b, v) => b.startHeight = v, true);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float TintPercent2
    {
        get => Getter(GetBiomes(), 2, b => b.tintPercent);
        set => Setter(GetBiomes(), value, 2, (b, v) => b.tintPercent = v, true);
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

    private NoiseLayer[] GetNoiseLayer() => shapeSettings.noiseLayers;

    [ExportSubgroup("Noise Layer 0")]
    [Export]
    public bool Enabled0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.enabled);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.enabled = v,
            shapeUpdate: true);
    }

    [Export]
    public bool UseFirstLayerAsMask0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.useFirstLayerAsMask);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.useFirstLayerAsMask = v,
            shapeUpdate: true);
    }

    [Export]
    public FilterType FilterType0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.noiseSettings.filterType);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.noiseSettings.filterType = v,
            shapeUpdate: true);
    }

    [Export]
    public float Strength0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.noiseSettings.strength);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.noiseSettings.strength = v,
            shapeUpdate: true);
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int NumLayers0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.noiseSettings.numLayers);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.noiseSettings.numLayers = v,
            shapeUpdate: true);
    }

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float Roughness0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.noiseSettings.roughness);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.noiseSettings.roughness = v,
            shapeUpdate: true);
    }

    [Export]
    public float BaseRoughness0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.noiseSettings.baseRoughness);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.noiseSettings.baseRoughness = v,
            shapeUpdate: true);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Persistence0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.noiseSettings.persistence);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.noiseSettings.persistence = v,
            shapeUpdate: true);
    }

    [Export]
    public Vector3 Center0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.noiseSettings.center);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.noiseSettings.center = v,
            shapeUpdate: true);
    }

    [Export]
    public float MinValue0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.noiseSettings.minValue);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.noiseSettings.minValue = v,
            shapeUpdate: true);
    }

    [Export]
    public float WeightMultiplier0
    {
        get => Getter(GetNoiseLayer(), 0, n => n.noiseSettings.weightMultiplier);
        set => Setter(GetNoiseLayer(), value, 0, (n, v) => n.noiseSettings.weightMultiplier = v,
            shapeUpdate: true);
    }

    [ExportSubgroup("Noise Layer 1")]
    [Export]
    public bool Enabled1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.enabled);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.enabled = v,
            shapeUpdate: true);
    }

    [Export]
    public bool UseFirstLayerAsMask1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.useFirstLayerAsMask);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.useFirstLayerAsMask = v,
            shapeUpdate: true);
    }

    [Export]
    public FilterType FilterType1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.noiseSettings.filterType);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.noiseSettings.filterType = v,
            shapeUpdate: true);
    }

    [Export]
    public float Strength1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.noiseSettings.strength);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.noiseSettings.strength = v,
            shapeUpdate: true);
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int NumLayers1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.noiseSettings.numLayers);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.noiseSettings.numLayers = v,
            shapeUpdate: true);
    }

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float Roughness1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.noiseSettings.roughness);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.noiseSettings.roughness = v,
            shapeUpdate: true);
    }

    [Export]
    public float BaseRoughness1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.noiseSettings.baseRoughness);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.noiseSettings.baseRoughness = v,
            shapeUpdate: true);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Persistence1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.noiseSettings.persistence);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.noiseSettings.persistence = v,
            shapeUpdate: true);
    }

    [Export]
    public Vector3 Center1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.noiseSettings.center);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.noiseSettings.center = v,
            shapeUpdate: true);
    }

    [Export]
    public float MinValue1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.noiseSettings.minValue);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.noiseSettings.minValue = v,
            shapeUpdate: true);
    }

    [Export]
    public float WeightMultiplier1
    {
        get => Getter(GetNoiseLayer(), 1, n => n.noiseSettings.weightMultiplier);
        set => Setter(GetNoiseLayer(), value, 1, (n, v) => n.noiseSettings.weightMultiplier = v,
            shapeUpdate: true);
    }


    [ExportSubgroup("Noise Layer 2")]
    [Export]
    public bool Enabled2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.enabled);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.enabled = v,
            shapeUpdate: true);
    }

    [Export]
    public bool UseFirstLayerAsMask2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.useFirstLayerAsMask);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.useFirstLayerAsMask = v,
            shapeUpdate: true);
    }

    [Export]
    public FilterType FilterType2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.noiseSettings.filterType);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.noiseSettings.filterType = v,
            shapeUpdate: true);
    }

    [Export]
    public float Strength2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.noiseSettings.strength);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.noiseSettings.strength = v,
            shapeUpdate: true);
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int NumLayers2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.noiseSettings.numLayers);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.noiseSettings.numLayers = v,
            shapeUpdate: true);
    }

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float Roughness2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.noiseSettings.roughness);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.noiseSettings.roughness = v,
            shapeUpdate: true);
    }

    [Export]
    public float BaseRoughness2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.noiseSettings.baseRoughness);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.noiseSettings.baseRoughness = v,
            shapeUpdate: true);
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Persistence2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.noiseSettings.persistence);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.noiseSettings.persistence = v,
            shapeUpdate: true);
    }

    [Export]
    public Vector3 Center2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.noiseSettings.center);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.noiseSettings.center = v,
            shapeUpdate: true);
    }

    [Export]
    public float MinValue2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.noiseSettings.minValue);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.noiseSettings.minValue = v,
            shapeUpdate: true);
    }

    [Export]
    public float WeightMultiplier2
    {
        get => Getter(GetNoiseLayer(), 2, n => n.noiseSettings.weightMultiplier);
        set => Setter(GetNoiseLayer(), value, 2, (n, v) => n.noiseSettings.weightMultiplier = v,
            shapeUpdate: true);
    }


    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}