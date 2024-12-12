using System;
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