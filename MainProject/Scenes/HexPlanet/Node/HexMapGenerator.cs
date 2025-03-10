using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexMapGenerator : Node3D
{
    public HexMapGenerator() => InitServices();
    [Export(PropertyHint.Range, "0, 0.5")] private float _jitterProbability = 0.25f;

    [Export(PropertyHint.Range, "20, 200")]
    private int _chunkSizeMin = 30;

    [Export(PropertyHint.Range, "20, 200")]
    private int _chunkSizeMax = 100;

    [Export(PropertyHint.Range, "5, 95")] private int _landPercentage = 50;
    [Export(PropertyHint.Range, "1, 5")] private int _waterLevel = 5;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _highRiseProbability = 0.25f;

    [Export(PropertyHint.Range, "0.0, 0.4")]
    private float _sinkProbability = 0.2f;

    [Export(PropertyHint.Range, "-4, 0")] private int _elevationMinimum = 0;
    [Export(PropertyHint.Range, "6, 10")] private int _elevationMaximum = 10;
    [Export(PropertyHint.Range, "0, 10")] private int _mapBoardX = 5;
    [Export(PropertyHint.Range, "0, 10")] private int _mapBoardZ = 5;
    [Export(PropertyHint.Range, "0, 10")] private int _regionBorder = 5;
    [Export(PropertyHint.Range, "1, 4")] private int _regionCount = 1;
    [Export(PropertyHint.Range, "0, 100")] private int _erosionPercentage = 50;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _evaporationFactor = 0.5f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _precipitationFactor = 0.25f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _runoffFactor = 0.25f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _seepageFactor = 0.125f;

    [Export] private int _windDirection; // 需要改成 enum

    [Export(PropertyHint.Range, "1.0, 10.0")]
    private float _windStrength = 4;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _startingMoisture = 0.1f;

    [Export(PropertyHint.Range, "0, 20")] private float _riverPercentage = 10f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _extraLakeProbability = 0.25f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _lowTemperature = 0f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _highTemperature = 1f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _temperatureJitter = 0.1f;

    [Export] private bool _useFixedSeed = false;

    [Export(PropertyHint.Range, "0, 2147483647")]
    private int _seed = 0;

    private int _landTileCount;
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

    #region 服务

    private ITileService _tileService;
    private ITileSearchService _tileSearchService;

    private void InitServices()
    {
        _tileService = Context.GetBean<ITileService>();
        _tileSearchService = Context.GetBean<ITileSearchService>();
    }

    #endregion

    public void GenerateMap()
    {
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
        foreach (var tile in _tileService.GetAll())
            tile.Data = tile.Data with { Values = tile.Data.Values.WithWaterLevel(_waterLevel) };
        CreateRegions();
        CreateLand();
        ErodeLand();
        CreateClimate();
        CreateRivers();
        SetTerrainType();
        _random.State = initState;
        stopwatch.Stop();
        GD.Print($"Generated map in {stopwatch.ElapsedMilliseconds} ms");
    }

    private void CreateRegions()
    {
        _regions.Clear();
        var borderX = _regionBorder;
        var region = new MapRegion();
        _regions.Add(region);
    }

    private void ErodeLand()
    {
        var erodibleTiles = _tileService.GetAll().Where(IsErodible).ToList();
        var targetErodibleCount = (int)(erodibleTiles.Count * (100 - _erosionPercentage) * 0.01f);
        while (erodibleTiles.Count > targetErodibleCount)
        {
            var index = _random.RandiRange(0, erodibleTiles.Count - 1);
            var tile = erodibleTiles[index];
            var targetTile = GetErosionTarget(tile);
            tile.Data = tile.Data with { Values = tile.Data.Values.WithElevation(tile.Data.Elevation - 1) };
            targetTile.Data = targetTile.Data with
            {
                Values = targetTile.Data.Values.WithElevation(targetTile.Data.Elevation + 1)
            };
            if (!IsErodible(tile))
            {
                var lastIndex = erodibleTiles.Count - 1;
                erodibleTiles[index] = erodibleTiles[lastIndex];
                erodibleTiles.RemoveAt(lastIndex);
            }

            foreach (var neighbor in _tileService.GetNeighbors(tile))
            {
                if (neighbor.Data.Elevation == tile.Data.Elevation + 2 && !erodibleTiles.Contains(neighbor))
                    erodibleTiles.Add(neighbor);
            }

            if (IsErodible(targetTile) && !erodibleTiles.Contains(targetTile))
                erodibleTiles.Add(targetTile);
            foreach (var neighbor in _tileService.GetNeighbors(targetTile))
            {
                // 有一个台阶上去就不是悬崖孤台了
                if (neighbor.Data.Elevation == targetTile.Data.Elevation + 1 && !IsErodible(neighbor))
                    erodibleTiles.Remove(neighbor);
            }
        }
    }

    private bool IsErodible(Tile tile)
    {
        var erodibleElevation = tile.Data.Elevation - 2;
        return _tileService.GetNeighbors(tile)
            .Any(neighbor => neighbor.Data.Elevation <= erodibleElevation);
    }

    private Tile GetErosionTarget(Tile tile)
    {
        var erodibleElevation = tile.Data.Elevation - 2;
        var candidates = _tileService.GetNeighbors(tile)
            .Where(neighbor => neighbor.Data.Elevation <= erodibleElevation)
            .ToList();
        return candidates[_random.RandiRange(0, candidates.Count - 1)];
    }

    private void CreateClimate()
    {
        _climate.Clear();
        _nextClimate.Clear();
        var initialData = new ClimateData { Moisture = _startingMoisture };
        var clearData = new ClimateData();
        for (var i = 0; i <= _tileService.GetCount(); i++)
        {
            _climate.Add(initialData);
            _nextClimate.Add(clearData);
        }

        for (var cycle = 0; cycle < 40; cycle++)
        {
            foreach (var tile in _tileService.GetAll())
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
        var cloudMaximum = 1f - tile.Data.ViewElevation / (_elevationMaximum + 1f);
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
        foreach (var neighbor in _tileService.GetNeighbors(tile))
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
        foreach (var tile in _tileService.GetAll())
        {
            if (tile.Data.IsUnderwater) continue;
            var data = _climate[tile.Id];
            var weight = data.Moisture * (tile.Data.Elevation - _waterLevel) / (_elevationMaximum - _waterLevel);
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
                var isValidOrigin = _tileService.GetNeighbors(origin)
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
            var neighbors = _tileService.GetNeighbors(tile).ToList();
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
                    _tileService.SetOutgoingRiver(tile, neighbor);
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
            var riverToTile = _tileService.GetNeighborByIdx(tile, direction);
            _tileService.SetOutgoingRiver(tile, riverToTile);
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
        var rockDesertElevation = _elevationMaximum - (_elevationMaximum - _waterLevel) / 2;
        foreach (var tile in _tileService.GetAll())
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
                else if (tile.Data.Elevation == _elevationMaximum)
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
                if (tile.Data.Elevation == _waterLevel - 1)
                {
                    int cliffs = 0, slopes = 0;
                    foreach (var neighbor in _tileService.GetNeighbors(tile))
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
                else if (tile.Data.Elevation >= _waterLevel)
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
        var sphereAxial = _tileService.GetSphereAxial(tile);
        var latitude = (sphereAxial.Coords.R + HexMetrics.Divisions) / (3f * HexMetrics.Divisions);
        // 具有南北半球
        latitude *= 2f;
        if (latitude > 1f)
            latitude = 2f - latitude;
        var temperature = Mathf.Lerp(_lowTemperature, _highTemperature, latitude);
        temperature *= 1f - (tile.Data.ViewElevation - _waterLevel) / (_elevationMaximum - _waterLevel + 1f);
        var jitter = HexMetrics.SampleNoise(tile.GetCentroid(HexMetrics.StandardRadius))[_temperatureJitterChannel];
        temperature += (jitter * 2f - 1f) * _temperatureJitter;
        return temperature;
    }

    private void CreateLand()
    {
        _landTileCount = Mathf.RoundToInt(_tileService.GetCount() * _landPercentage * 0.01f);
        var landBudget = _landTileCount;
        // 防止无限循环的守卫值
        for (var guard = 0; guard < 10000; guard++)
        {
            var sink = _random.Randf() < _sinkProbability;
            foreach (var region in _regions)
            {
                var chunkSize = _random.RandiRange(_chunkSizeMin, _chunkSizeMax);
                if (sink)
                    landBudget = SinkTerrain(chunkSize, landBudget, region);
                else
                {
                    landBudget = RaiseTerrain(chunkSize, landBudget, region);
                    if (landBudget <= 0)
                        return;
                }
            }
        }

        if (landBudget <= 0) return;
        _landTileCount -= landBudget;
        GD.PrintErr($"Failed to use up {landBudget} land budget.");
    }

    private int RaiseTerrain(int chunkSize, int budget, MapRegion region)
    {
        var firstTileId = GetRandomCellIndex(region);
        var rise = _random.Randf() < _highRiseProbability ? 2 : 1;
        return _tileSearchService.RaiseTerrain(chunkSize, budget, firstTileId, rise,
            _random, _elevationMaximum, _waterLevel, _jitterProbability);
    }

    private int SinkTerrain(int chunkSize, int budget, MapRegion region)
    {
        var firstTileId = GetRandomCellIndex(region);
        var sink = _random.Randf() < _highRiseProbability ? 2 : 1;
        return _tileSearchService.SinkTerrain(chunkSize, budget, firstTileId, sink,
            _random, _elevationMinimum, _waterLevel, _jitterProbability);
    }

    private int GetRandomCellIndex(MapRegion region)
    {
        return GD.RandRange(1, _tileService.GetCount());
    }
}