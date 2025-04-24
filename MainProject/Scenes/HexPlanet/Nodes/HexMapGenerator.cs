using System;
using System.Collections.Generic;
using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-04 12:03
[Tool]
public partial class HexMapGenerator : Node, IHexMapGenerator
{
    public HexMapGenerator()
    {
        Context.RegisterToHolder<IHexMapGenerator>(this);
    }

    public event IHexMapGenerator.CreatingErosionLandEvent? CreatingErosionLand;

    public int EmitCreatingErosionLand(RandomNumberGenerator rng, List<MapRegion> regions) =>
        CreatingErosionLand?.Invoke(rng, regions) ?? 0;

    public event Action<RandomNumberGenerator>? ErodingLand;
    public void EmitErodingLand(RandomNumberGenerator rng) => ErodingLand?.Invoke(rng);
    public event IHexMapGenerator.CreatingFractalNoiseLandEvent? CreatingFractalNoiseLand;
    public int EmitCreatingFractalNoiseLand(RandomNumberGenerator rng) => CreatingFractalNoiseLand?.Invoke(rng) ?? 0;
    public event IHexMapGenerator.CreatingRealEarthLandEvent? CreatingRealEarthLand;
    public int EmitCreatingRealEarthLand() => CreatingRealEarthLand?.Invoke() ?? 0;
    public NodeEvent? NodeEvent => null;

    [Export(PropertyHint.Range, "0, 10")] public int MapBoardX { get; set; } = 5;
    [Export(PropertyHint.Range, "0, 10")] public int MapBoardZ { get; set; } = 5;
    [Export(PropertyHint.Range, "0, 10")] public int RegionBorder { get; set; } = 5;
    [Export(PropertyHint.Range, "1, 4")] public int RegionCount { get; set; } = 1;
    [Export] public LandGeneratorType LandGeneratorType { get; set; } = LandGeneratorType.Erosion;

    [ExportSubgroup("水循环设置")]
    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float EvaporationFactor { get; set; } = 0.5f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float PrecipitationFactor { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float RunoffFactor { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float SeepageFactor { get; set; } = 0.125f;

    [Export(PropertyHint.Range, "0, 5")] public int WindDirection { get; set; } // 需要改成 enum

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float WindStrength { get; set; } = 4;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float StartingMoisture { get; set; } = 0.1f;

    [Export(PropertyHint.Range, "0, 20")] public float RiverPercentage { get; set; } = 10f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float ExtraLakeProbability { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float LowTemperature { get; set; }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float HighTemperature { get; set; } = 1f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float TemperatureJitter { get; set; } = 0.1f;

    [ExportSubgroup("生成种子")] [Export] public bool UseFixedSeed { get; set; }

    [Export(PropertyHint.Range, "0, 2147483647")]
    public int Seed { get; set; }
}