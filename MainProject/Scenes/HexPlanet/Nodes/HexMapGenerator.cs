using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apps.Queries.Contexts;
using Commons.Utils;
using Contexts;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Planets;
using Godot;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.LandGenerators;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

public enum LandGeneratorType
{
    Erosion, // Catlike Coding 的随机升降土地，然后应用侵蚀算法
    FractalNoise, // 叠加的分形噪声
    RealEarth, // 真实地球
}

public class MapRegion
{
    public int[] IcosahedronId;
}

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-04 12:03
[Tool]
public partial class HexMapGenerator : Node, IHexMapGenerator
{
    public HexMapGenerator()
    {
        InitServices();
        NodeContext.Instance.RegisterSingleton<IHexMapGenerator>(this);
    }

    [Export(PropertyHint.Range, "1, 5")]
    public int DefaultWaterLevel
    {
        get => _planetConfig!.DefaultWaterLevel;
        set => _planetConfig!.DefaultWaterLevel = value;
    }

    [Export(PropertyHint.Range, "10, 15")]
    public int ElevationStep
    {
        get => _planetConfig!.ElevationStep;
        set => _planetConfig!.ElevationStep = value;
    }

    [Export(PropertyHint.Range, "0, 10")] private int _mapBoardX = 5;
    [Export(PropertyHint.Range, "0, 10")] private int _mapBoardZ = 5;
    [Export(PropertyHint.Range, "0, 10")] private int _regionBorder = 5;
    [Export(PropertyHint.Range, "1, 4")] private int _regionCount = 1;
    [Export] private LandGeneratorType _landGeneratorType = LandGeneratorType.Erosion;

    [ExportSubgroup("水循环设置")] [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _evaporationFactor = 0.5f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _precipitationFactor = 0.25f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _runoffFactor = 0.25f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _seepageFactor = 0.125f;

    [Export(PropertyHint.Range, "0, 5")] private int _windDirection; // 需要改成 enum

    [Export(PropertyHint.Range, "1.0, 10.0")]
    private float _windStrength = 4;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _startingMoisture = 0.1f;

    [Export(PropertyHint.Range, "0, 20")] private float _riverPercentage = 10f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _extraLakeProbability = 0.25f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _lowTemperature;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _highTemperature = 1f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _temperatureJitter = 0.1f;

    [ExportSubgroup("生成种子")] [Export] private bool _useFixedSeed;

    [Export(PropertyHint.Range, "0, 2147483647")]
    private int _seed;

    private int _landTileCount;
    private RandomNumberGenerator _random = new();


    private readonly List<MapRegion> _regions = [];

    private struct ClimateData
    {
        public float Clouds, Moisture;
    }

    private List<ClimateData> _climate = [];
    private List<ClimateData> _nextClimate = [];
    private int _temperatureJitterChannel;
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

    #region on-ready 节点

    private ErosionLandGenerator? _erosionLandGenerator;
    private FractalNoiseLandGenerator? _fractalNoiseLandGenerator;
    private RealEarthLandGenerator? _realEarthLandGenerator;

    private void InitOnReadyNodes()
    {
        _erosionLandGenerator = GetNode<ErosionLandGenerator>("%ErosionLandGenerator");
        _fractalNoiseLandGenerator = GetNode<FractalNoiseLandGenerator>("%FractalNoiseLandGenerator");
        _realEarthLandGenerator = GetNode<RealEarthLandGenerator>("%RealEarthLandGenerator");
    }

    #endregion

    #region 服务和存储

    private IPointRepo? _pointRepo;
    private ITileRepo? _tileRepo;
    private INoiseConfig? _noiseConfig;
    private IPlanetConfig? _planetConfig;

    private void InitServices()
    {
        _pointRepo = Context.GetBeanFromHolder<IPointRepo>();
        _tileRepo = Context.GetBeanFromHolder<ITileRepo>();
        _noiseConfig = Context.GetBeanFromHolder<INoiseConfig>();
        _planetConfig = Context.GetBeanFromHolder<IPlanetConfig>();
    }

    #endregion

    public override void _Ready() => InitOnReadyNodes();
    public override void _ExitTree() => NodeContext.Instance.DestroySingleton<IHexMapGenerator>();

    public void GenerateMap()
    {
        var time = Time.GetTicksMsec();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var initState = _random.State;
        if (!_useFixedSeed)
        {
            _random.Randomize();
            _seed = _random.RandiRange(0, int.MaxValue)
                    ^ (int)DateTime.Now.Ticks
                    ^ (int)Time.GetTicksUsec()
                    & int.MaxValue;
        }

        GD.Print($"Generating map with seed {_seed}");
        _random.Seed = (ulong)_seed;
        foreach (var tile in _tileRepo!.GetAll())
            tile.Data = tile.Data with
            {
                Values = tile.Data.Values.WithWaterLevel(DefaultWaterLevel).WithElevation(0)
            };
        switch (_landGeneratorType)
        {
            case LandGeneratorType.Erosion:
                CreateRegions();
                GD.Print($"--- CreatedRegions in {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Restart();

                _landTileCount = _erosionLandGenerator!.CreateLand(_random, _regions);
                GD.Print($"--- CreatedLand in {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Restart();

                _erosionLandGenerator.ErodeLand(_random);
                GD.Print($"--- ErodeLand in {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Restart();
                break;
            case LandGeneratorType.FractalNoise:
                _landTileCount = _fractalNoiseLandGenerator!.CreateLand(_random);
                GD.Print($"--- CreatedLand in {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Restart();
                break;
            case LandGeneratorType.RealEarth:
            default:
                _landTileCount = _realEarthLandGenerator!.CreateLand();
                GD.Print($"--- CreatedLand in {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Restart();
                break;
        }

        CreateClimate();
        GD.Print($"--- CreateClimate in {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Restart();

        CreateRivers();
        GD.Print($"--- CreateRivers in {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Restart();

        SetTerrainType();
        _random.State = initState;
        GD.Print($"--- SetTerrainType in {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Stop();
        GD.Print($"Generated map in {Time.GetTicksMsec() - time} ms");
    }

    private void CreateRegions()
    {
        _regions.Clear();
        var borderX = _regionBorder;
        var region = new MapRegion();
        _regions.Add(region);
    }

    private void CreateClimate()
    {
        _climate.Clear();
        _nextClimate.Clear();
        var initialData = new ClimateData { Moisture = _startingMoisture };
        var clearData = new ClimateData();
        for (var i = 0; i <= _tileRepo!.GetCount(); i++)
        {
            _climate.Add(initialData);
            _nextClimate.Add(clearData);
        }

        for (var cycle = 0; cycle < 40; cycle++)
        {
            foreach (var tile in _tileRepo.GetAll())
                EvolveClimate(tile);
            (_nextClimate, _climate) = (_climate, _nextClimate);
        }
    }

    private void EvolveClimate(Tile tile)
    {
        var tileClimate = _climate[tile.Id];
        if (tile.Data.IsUnderwater)
        {
            tileClimate.Moisture = 1f;
            tileClimate.Clouds += _evaporationFactor;
        }
        else
        {
            var evaporation = tileClimate.Moisture * _evaporationFactor;
            tileClimate.Moisture -= evaporation;
            tileClimate.Clouds += evaporation;
        }

        var precipitation = tileClimate.Clouds * _precipitationFactor;
        tileClimate.Clouds -= precipitation;
        tileClimate.Moisture += precipitation;
        var cloudMaximum = 1f - tile.Data.ViewElevation / (ElevationStep + 1f);
        if (tileClimate.Clouds > cloudMaximum)
        {
            tileClimate.Moisture += tileClimate.Clouds - cloudMaximum;
            tileClimate.Clouds = cloudMaximum;
        }

        var mainDispersalDirection = tile.OppositeIdx(_windDirection);
        var edgeCount = tile.IsPentagon() ? 5 : 6;
        var cloudDispersal = tileClimate.Clouds * (1f / (edgeCount - 1 + _windStrength));
        var runoff = tileClimate.Moisture * _runoffFactor * (1f / edgeCount);
        var seepage = tileClimate.Moisture * _seepageFactor * (1f / edgeCount);
        foreach (var neighbor in _tileRepo!.GetNeighbors(tile))
        {
            var neighborClimate = _nextClimate[neighbor.Id];
            if (tile.GetNeighborIdx(neighbor) == mainDispersalDirection)
                neighborClimate.Clouds += cloudDispersal * _windStrength;
            else
                neighborClimate.Clouds += cloudDispersal;
            var elevationDelta = neighbor.Data.ViewElevation - tile.Data.ViewElevation;
            if (elevationDelta < 0)
            {
                tileClimate.Moisture -= runoff;
                neighborClimate.Moisture += runoff;
            }
            else if (elevationDelta == 0)
            {
                tileClimate.Moisture -= seepage;
                neighborClimate.Moisture += seepage;
            }

            _nextClimate[neighbor.Id] = neighborClimate;
        }

        var nextTileClimate = _nextClimate[tile.Id];
        nextTileClimate.Moisture += tileClimate.Moisture;
        if (nextTileClimate.Moisture > 1f)
            nextTileClimate.Moisture = 1f;
        _nextClimate[tile.Id] = nextTileClimate;
        _climate[tile.Id] = new ClimateData();
    }

    private void CreateRivers()
    {
        var riverOrigins = new List<Tile>();
        foreach (var tile in _tileRepo!.GetAll())
        {
            if (tile.Data.IsUnderwater) continue;
            var data = _climate[tile.Id];
            var weight = data.Moisture * (tile.Data.Elevation - DefaultWaterLevel) /
                         (ElevationStep - DefaultWaterLevel);
            if (weight > 0.75f)
            {
                riverOrigins.Add(tile);
                riverOrigins.Add(tile);
            }

            if (weight > 0.5f)
                riverOrigins.Add(tile);
            if (weight > 0.25f)
                riverOrigins.Add(tile);
        }

        var riverBudget = Mathf.RoundToInt(_landTileCount * _riverPercentage * 0.01f);
        GD.Print($"{riverOrigins.Count} river origins with river budget {riverBudget}");
        while (riverBudget > 0 && riverOrigins.Count > 0)
        {
            var lastIndex = riverOrigins.Count - 1;
            var index = _random.RandiRange(0, lastIndex);
            var origin = riverOrigins[index];
            riverOrigins[index] = riverOrigins[lastIndex];
            riverOrigins.RemoveAt(lastIndex);
            if (!origin.Data.HasRiver)
            {
                var isValidOrigin = _tileRepo.GetNeighbors(origin)
                    .All(neighbor => !neighbor.Data.HasRiver && !neighbor.Data.IsUnderwater);
                if (isValidOrigin)
                    riverBudget -= CreateRiver(origin);
            }
        }

        if (riverBudget > 0)
            GD.PrintErr($"Failed to use up river budget {riverBudget}");
    }

    private readonly List<int> _flowDirections = [];

    private int CreateRiver(Tile origin)
    {
        var length = 1;
        var tile = origin;
        var direction = 0;
        while (!tile.Data.IsUnderwater)
        {
            var minNeighborElevation = int.MaxValue;
            _flowDirections.Clear();
            var neighbors = _tileRepo!.GetNeighbors(tile).ToList();
            foreach (var neighbor in neighbors)
            {
                if (neighbor.Data.Elevation < minNeighborElevation)
                    minNeighborElevation = neighbor.Data.Elevation;
                if (neighbor == origin || neighbor.Data.HasIncomingRiver)
                    continue;
                var delta = neighbor.Data.Elevation - tile.Data.Elevation;
                if (delta > 0)
                    continue;
                if (neighbor.Data.HasOutgoingRiver)
                {
                    _tileRepo.SetOutgoingRiver(tile, neighbor);
                    return length;
                }

                var d = tile.GetNeighborIdx(neighbor);
                if (delta < 0)
                {
                    _flowDirections.Add(d);
                    _flowDirections.Add(d);
                    _flowDirections.Add(d);
                }

                if (length == 1 || (d != tile.Next2Idx(direction) && d != tile.Previous2Idx(direction)))
                    _flowDirections.Add(d);
                _flowDirections.Add(d);
            }

            if (_flowDirections.Count == 0)
            {
                if (length == 1)
                    return 0;
                if (minNeighborElevation >= tile.Data.Elevation)
                {
                    tile.Data = tile.Data with { Values = tile.Data.Values.WithWaterLevel(minNeighborElevation) };
                    if (minNeighborElevation == tile.Data.Elevation)
                        tile.Data = tile.Data with
                        {
                            Values = tile.Data.Values.WithElevation(minNeighborElevation - 1)
                        };
                }

                break;
            }

            direction = _flowDirections[_random.RandiRange(0, _flowDirections.Count - 1)];
            var riverToTile = _tileRepo.GetNeighborByIdx(tile, direction)!;
            _tileRepo.SetOutgoingRiver(tile, riverToTile);
            length++;
            if (minNeighborElevation >= tile.Data.Elevation && _random.Randf() < _extraLakeProbability)
            {
                // 湖泊
                tile.Data = tile.Data with { Values = tile.Data.Values.WithWaterLevel(tile.Data.Elevation) };
                // 由于我们现在水面的绘制原理是连接实际的水面高度，所以需要把周围一圈邻居的水面高度都修改一下
                foreach (var neighbor in neighbors)
                    neighbor.Data = neighbor.Data with
                    {
                        Values = neighbor.Data.Values.WithWaterLevel(tile.Data.WaterLevel)
                    };
                tile.Data = tile.Data with { Values = tile.Data.Values.WithElevation(tile.Data.Elevation - 1) };
            }

            tile = riverToTile;
        }

        return length;
    }

    private void SetTerrainType()
    {
        _temperatureJitterChannel = _random.RandiRange(0, 3);
        var rockDesertElevation = ElevationStep - (ElevationStep - DefaultWaterLevel) / 2;
        foreach (var tile in _tileRepo!.GetAll())
        {
            var temperature = DetermineTemperature(tile);
            var moisture = _climate[tile.Id].Moisture;
            if (!tile.Data.IsUnderwater)
            {
                var t = 0;
                for (; t < _temperatureBands.Length; t++)
                    if (temperature < _temperatureBands[t])
                        break;
                var m = 0;
                for (; m < _moistureBands.Length; m++)
                    if (moisture < _moistureBands[m])
                        break;
                var tileBiome = _biomes[t * 4 + m];
                if (tileBiome.Terrain == 0)
                {
                    // 假设如果一个单元格的高度比水位更接近最高高度，沙子就会变成岩石。这是岩石沙漠高程线
                    if (tile.Data.Elevation >= rockDesertElevation)
                        tileBiome.Terrain = 3;
                }
                // 强制处于最高海拔的单元格变成雪盖，无论它们有多暖和，只要它们不太干燥
                else if (tile.Data.Elevation == ElevationStep)
                    tileBiome.Terrain = 4;

                // 确保植物不会出现在雪地上
                if (tileBiome.Terrain == 4)
                    tileBiome.Plant = 0;
                // 如果等级还没有达到最高点，让我们也增加河流沿岸的植物等级
                else if (tileBiome.Plant < 3 && tile.Data.HasRiver)
                    tileBiome.Plant++;
                tile.Data = tile.Data with
                {
                    Values = tile.Data.Values.WithTerrainTypeIndex(tileBiome.Terrain)
                        .WithPlantLevel(tileBiome.Plant)
                };
            }
            else
            {
                int terrain;
                if (tile.Data.Elevation == DefaultWaterLevel - 1)
                {
                    int cliffs = 0, slopes = 0;
                    foreach (var neighbor in _tileRepo.GetNeighbors(tile))
                    {
                        var delta = neighbor.Data.Elevation - tile.Data.WaterLevel;
                        if (delta == 0)
                            slopes++;
                        else if (delta > 0)
                            cliffs++;
                    }

                    if (cliffs + slopes > 3)
                        terrain = 1;
                    else if (cliffs > 0)
                        terrain = 3;
                    else if (slopes > 0)
                        terrain = 0;
                    else
                        terrain = 1;
                }
                // 用草来建造比水位更高的单元格，这些是由河流形成的湖泊
                else if (tile.Data.Elevation >= DefaultWaterLevel)
                    terrain = 1;
                // 负海拔的单元格位于深处，让我们用岩石来做
                else if (tile.Data.Elevation < 0)
                    terrain = 3;
                else
                    terrain = 2;

                // 确保在最冷的温度带内不会出现绿色的水下单元格。用泥代替这些单元格
                if (terrain == 1 && temperature < _temperatureBands[0])
                    terrain = 2;
                tile.Data = tile.Data with { Values = tile.Data.Values.WithTerrainTypeIndex(terrain) };
            }
        }
    }

    private float DetermineTemperature(Tile tile)
    {
        var sphereAxial = _pointRepo!.GetSphereAxial(tile);
        var latitude = (sphereAxial.Coords.R + _planetConfig!.Divisions) /
                       (3f * _planetConfig.Divisions);
        // 具有南北半球
        latitude *= 2f;
        if (latitude > 1f)
            latitude = 2f - latitude;
        var temperature = Mathf.Lerp(_lowTemperature, _highTemperature, latitude);
        temperature *= 1f - (tile.Data.ViewElevation - DefaultWaterLevel) / (ElevationStep - DefaultWaterLevel + 1f);
        var jitter = _noiseConfig!.SampleNoise(tile.GetCentroid(HexMetrics.StandardRadius))[_temperatureJitterChannel];
        temperature += (jitter * 2f - 1f) * _temperatureJitter;
        return temperature;
    }
}