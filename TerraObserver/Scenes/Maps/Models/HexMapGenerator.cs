using System.Collections.Generic;
using Godot;
using TO.Domains.Types.Maps;

namespace TerraObserver.Scenes.Maps.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-07 10:14:24
[Tool]
[GlobalClass]
public partial class HexMapGenerator : Resource, IHexMapGenerator
{
    #region Export 属性

    [ExportGroup("地图生成设置")]
    [Export]
    public LandGenerator? LandGenerator
    {
        get => _landGenerator;
        set
        {
            _landGenerator = value;
            EmitChanged();
        }
    }

    private LandGenerator? _landGenerator;

    public ILandGenerator GetLandGenerator => LandGenerator!;
    [Export(PropertyHint.Range, "1, 5")] public int DefaultWaterLevel { get; set; } = 5;

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

    #endregion

    #region 普通属性

    public RandomNumberGenerator Rng { get; } = new();
    public int LandTileCount { get; set; }
    public List<ClimateData> Climate { get; set; } = [];
    public List<ClimateData> NextClimate { get; set; } = [];
    public int TemperatureJitterChannel { get; set; }

    #endregion
}