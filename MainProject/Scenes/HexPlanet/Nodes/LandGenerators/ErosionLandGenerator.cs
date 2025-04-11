using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// 基于 Catlike Coding 六边形地图教程中的随机升降土地，再运用侵蚀算法原理的陆地生成器
/// 速度较慢，生成十万级地块星球需要约半分钟
/// Author: Zhu XH
/// Date: 2025-03-20 19:01:10
[Tool]
public partial class ErosionLandGenerator : Node
{
    public ErosionLandGenerator() => InitServices();

    [Export(PropertyHint.Range, "5, 95")] private int _landPercentage = 50;

    [Export(PropertyHint.Range, "20, 200")]
    private int _chunkSizeMin = 30;

    [Export(PropertyHint.Range, "20, 200")]
    private int _chunkSizeMax = 100;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _highRiseProbability = 0.25f;

    [Export(PropertyHint.Range, "0.0, 0.4")]
    private float _sinkProbability = 0.2f;

    [Export(PropertyHint.Range, "0, 0.5")] private float _jitterProbability = 0.25f;

    [Export(PropertyHint.Range, "0, 100")] private int _erosionPercentage = 50;

    #region 服务和存储

    private ITileRepo _tileRepo;
    private ITileSearchService _tileSearchService;

    private void InitServices()
    {
        _tileRepo = Context.GetBean<ITileRepo>();
        _tileSearchService = Context.GetBean<ITileSearchService>();
    }

    #endregion

    public int CreateLand(RandomNumberGenerator random, List<MapRegion> regions)
    {
        var landTileCount = Mathf.RoundToInt(_tileRepo.GetCount() * _landPercentage * 0.01f);
        var landBudget = landTileCount;
        // 根据地图尺寸来设置对应循环次数上限，保证大地图也能尽量用完 landBudget
        for (var guard = 0; guard < landTileCount; guard++) // 防止无限循环的守卫值
        {
            var sink = random.Randf() < _sinkProbability;
            foreach (var region in regions)
            {
                var chunkSize = random.RandiRange(_chunkSizeMin, _chunkSizeMax);
                if (sink)
                    landBudget = SinkTerrain(random, chunkSize, landBudget, region);
                else
                {
                    landBudget = RaiseTerrain(random, chunkSize, landBudget, region);
                    if (landBudget <= 0)
                        return landTileCount;
                }
            }
        }

        if (landBudget <= 0) return 0;
        landTileCount -= landBudget;
        GD.PrintErr($"Failed to use up {landBudget} land budget.");
        return landTileCount;
    }

    private int RaiseTerrain(RandomNumberGenerator random, int chunkSize, int budget, MapRegion region)
    {
        var firstTileId = GetRandomCellIndex(region);
        var rise = random.Randf() < _highRiseProbability ? 2 : 1;
        return _tileSearchService.RaiseTerrain(chunkSize, budget, firstTileId, rise, random, _jitterProbability);
    }

    private int SinkTerrain(RandomNumberGenerator random, int chunkSize, int budget, MapRegion region)
    {
        var firstTileId = GetRandomCellIndex(region);
        var sink = random.Randf() < _highRiseProbability ? 2 : 1;
        return _tileSearchService.SinkTerrain(chunkSize, budget, firstTileId, sink, random, _jitterProbability);
    }

    private int GetRandomCellIndex(MapRegion region)
    {
        return GD.RandRange(1, _tileRepo.GetCount());
    }


    public void ErodeLand(RandomNumberGenerator random)
    {
        var erodibleTiles = _tileRepo.GetAll().Where(IsErodible).ToList();
        var targetErodibleCount = (int)(erodibleTiles.Count * (100 - _erosionPercentage) * 0.01f);
        while (erodibleTiles.Count > targetErodibleCount)
        {
            var index = random.RandiRange(0, erodibleTiles.Count - 1);
            var tile = erodibleTiles[index];
            var targetTile = GetErosionTarget(random, tile);
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

            foreach (var neighbor in _tileRepo.GetNeighbors(tile))
            {
                if (neighbor.Data.Elevation == tile.Data.Elevation + 2 && !erodibleTiles.Contains(neighbor))
                    erodibleTiles.Add(neighbor);
            }

            if (IsErodible(targetTile) && !erodibleTiles.Contains(targetTile))
                erodibleTiles.Add(targetTile);
            foreach (var neighbor in _tileRepo.GetNeighbors(targetTile))
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
        return _tileRepo.GetNeighbors(tile)
            .Any(neighbor => neighbor.Data.Elevation <= erodibleElevation);
    }

    private Tile GetErosionTarget(RandomNumberGenerator random, Tile tile)
    {
        var erodibleElevation = tile.Data.Elevation - 2;
        var candidates = _tileRepo.GetNeighbors(tile)
            .Where(neighbor => neighbor.Data.Elevation <= erodibleElevation)
            .ToList();
        return candidates[random.RandiRange(0, candidates.Count - 1)];
    }
}