using System;
using FrontEndToolFS.HexPlane;
using FrontEndToolFS.Tool;
using Godot;

namespace ZeromaXPlayground.game.HexPlane.Map;

public partial class HexMapGenerator : HexMapGeneratorFS
{
    [Export]
    public HexGridFS Grid
    {
        get => grid;
        set => grid = value;
    }

    [Export(PropertyHint.Range, "0, 0.5")]
    public float JitterProbability
    {
        get => jitterProbability;
        set => jitterProbability = value;
    }

    [Export(PropertyHint.Range, "20, 200")]
    public int ChunkSizeMin
    {
        get => chunkSizeMin;
        set => chunkSizeMin = value;
    }

    [Export(PropertyHint.Range, "20, 200")]
    public int ChunkSizeMax
    {
        get => chunkSizeMax;
        set => chunkSizeMax = value;
    }

    [Export(PropertyHint.Range, "5, 95")]
    public int LandPercentage
    {
        get => landPercentage;
        set => landPercentage = value;
    }

    [Export(PropertyHint.Range, "1, 5")]
    public int WaterLevel
    {
        get => waterLevel;
        set => waterLevel = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float HighRiseProbability
    {
        get => highRiseProbability;
        set => highRiseProbability = value;
    }

    [Export(PropertyHint.Range, "0.0, 0.4")]
    public float SinkProbability
    {
        get => sinkProbability;
        set => sinkProbability = value;
    }

    [Export(PropertyHint.Range, "-4, 0")]
    public int ElevationMinimum
    {
        get => elevationMinimum;
        set => elevationMinimum = value;
    }

    [Export(PropertyHint.Range, "6, 10")]
    public int ElevationMaximum
    {
        get => elevationMaximum;
        set => elevationMaximum = value;
    }

    [Export(PropertyHint.Range, "0, 10")]
    public int MapBorderX
    {
        get => mapBorderX;
        set => mapBorderX = value;
    }

    [Export(PropertyHint.Range, "0, 10")]
    public int MapBorderZ
    {
        get => mapBorderZ;
        set => mapBorderZ = value;
    }

    [Export(PropertyHint.Range, "0, 10")]
    public int RegionBorder
    {
        get => regionBorder;
        set => regionBorder = value;
    }

    [Export(PropertyHint.Range, "1, 4")]
    public int RegionCount
    {
        get => regionCount;
        set => regionCount = value;
    }

    [Export(PropertyHint.Range, "0, 100")]
    public int ErosionPercentage
    {
        get => erosionPercentage;
        set => erosionPercentage = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float EvaporationFactor
    {
        get => evaporationFactor;
        set => evaporationFactor = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float PrecipitationFactor
    {
        get => precipitationFactor;
        set => precipitationFactor = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float RunoffFactor
    {
        get => runoffFactor;
        set => runoffFactor = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float SeepageFactor
    {
        get => seepageFactor;
        set => seepageFactor = value;
    }

    [Export]
    public HexDirection WindDirection
    {
        get => windDirection;
        set => windDirection = value;
    }

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float WindStrength
    {
        get => windStrength;
        set => windStrength = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float StartingMoisture
    {
        get => startingMoisture;
        set => startingMoisture = value;
    }

    [Export(PropertyHint.Range, "0, 20")]
    public float RiverPercentage
    {
        get => riverPercentage;
        set => riverPercentage = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float ExtraLakeProbability
    {
        get => extraLakeProbability;
        set => extraLakeProbability = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float LowTemperature
    {
        get => lowTemperature;
        set => lowTemperature = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float HighTemperature
    {
        get => highTemperature;
        set => highTemperature = value;
    }

    [Export]
    public HemisphereMode Hemisphere
    {
        get => hemisphere;
        set => hemisphere = value;
    }

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float TemperatureJitter
    {
        get => temperatureJitter;
        set => temperatureJitter = value;
    }

    [Export]
    public bool UseFixedSeed
    {
        get => useFixedSeed;
        set => useFixedSeed = value;
    }

    [Export(PropertyHint.Range, "0, 2147483647")]
    public int Seed
    {
        get => seed;
        set => seed = value;
    }
}