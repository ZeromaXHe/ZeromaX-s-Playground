using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class HexMapGenerator : Node3D
{
    [Export] private float _jitterProbability = 0.25f;
    [Export] private int _chunkSizeMin = 30;
    [Export] private int _chunkSizeMax = 100;
    [Export] private int _landPercentage = 50;
    [Export] private int _waterLevel = 3;
    [Export] private float _highRiseProbability = 0.25f;
    [Export] private float _sinkProbability = 0.2f;
    [Export] private int _elevationMinimum = 0;
    [Export] private int _elevationMaximum = 10;
    [Export] private int _regionBorder = 5;
    [Export] private int _regionCount = 1;
    [Export] private int _erosionPercentage = 50;
    [Export] private float _evaporationFactor = 0.5f;
    [Export] private float _precipitationFactor = 0.25f;
    [Export] private float _runoffFactor = 0.25f;
    [Export] private float _seepageFactor = 0.125f;
    [Export] private float _startingMoisture = 0.1f;
    [Export] private float _riverPercentage = 10f;
    [Export] private float _extraLakeProbability = 0.25f;
    [Export] private float _lowTemperature = 0f;
    [Export] private float _highTemperature = 1f;
    [Export] private float _temperatureJitter = 0.1f;
    [Export] private bool _useFixedSeed = false;
    [Export] private ulong _seed = 0;

    private int landCount;
    private RandomNumberGenerator _random = new();

    private class MapRegion
    {
        public int[] IcosahedronId;
    }

    private readonly List<MapRegion> _regions = [];

    private struct ClimateData
    {
        public float Clouds, Moisture;
    }

    private List<ClimateData> _climate = [];
    private List<ClimateData> _nextClimate = [];
    private int _temperatureJitterChannel = 0;
    private readonly float[] _temperatureBands = [0.1f, 0.3f, 0.6f];
    private readonly float[] _moistureBands = [0.12f, 0.28f, 0.85f];

    private struct Biome(int terrain, int plant)
    {
        public int Terrain = terrain, Plant = plant;
    }

    private readonly Biome[] _biomes =
    [
        new Biome(0, 0), new Biome(4, 0), new Biome(4, 0), new Biome(4, 0),
        new Biome(0, 0), new Biome(2, 0), new Biome(2, 1), new Biome(2, 2),
        new Biome(0, 0), new Biome(1, 0), new Biome(1, 1), new Biome(1, 2),
        new Biome(0, 0), new Biome(1, 1), new Biome(1, 2), new Biome(1, 3)
    ];

    private int GetRandomCellIndex(MapRegion region)
    {
        return 0;
    }
}