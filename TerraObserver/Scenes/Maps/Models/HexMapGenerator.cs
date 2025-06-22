using Godot;
using TO.Abstractions.Views.Maps;

namespace TerraObserver.Scenes.Maps.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-07 10:14:24
[Tool]
[GlobalClass]
public partial class HexMapGenerator: Resource, IHexMapGenerator
{
    [ExportGroup("基础设置")]
    [Export(PropertyHint.Range, "0, 10")] public int MapBoardX { get; set; } = 5;
    [Export(PropertyHint.Range, "0, 10")] public int MapBoardZ { get; set; } = 5;
    [Export(PropertyHint.Range, "0, 10")] public int RegionBorder { get; set; } = 5;
    [Export(PropertyHint.Range, "1, 4")] public int RegionCount { get; set; } = 1;

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
    
    [ExportGroup("Catlike Coding 侵蚀算法设置")]
    [Export(PropertyHint.Range, "5, 95")] public int LandPercentage { get; set; } = 50;

    [Export(PropertyHint.Range, "20, 200")]
    public int ChunkSizeMin { get; set; } = 30;

    [Export(PropertyHint.Range, "20, 200")]
    public int ChunkSizeMax { get; set; } = 100;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float HighRiseProbability { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0.0, 0.4")]
    public float SinkProbability { get; set; } = 0.2f;

    [Export(PropertyHint.Range, "0, 0.5")] public float JitterProbability { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0, 100")] public int ErosionPercentage { get; set; } = 50;
}