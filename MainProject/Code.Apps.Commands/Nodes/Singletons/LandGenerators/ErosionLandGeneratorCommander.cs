using Domains.Services.Abstractions.Nodes.Singletons.LandGenerators;
using Domains.Services.Abstractions.Searches;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;
using Nodes.Abstractions.LandGenerators;

namespace Apps.Commands.Nodes.Singletons.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:39:45
public class ErosionLandGeneratorCommander
{
    private readonly IErosionLandGeneratorService _erosionLandGeneratorService;
    private readonly IErosionLandGeneratorRepo _erosionLandGeneratorRepo;

    private readonly IHexMapGeneratorRepo _hexMapGeneratorRepo;
    private readonly ITileRepo _tileRepo;
    private readonly ITileSearchService _tileSearchService;

    public ErosionLandGeneratorCommander(
        IErosionLandGeneratorService erosionLandGeneratorService,
        IErosionLandGeneratorRepo erosionLandGeneratorRepo,
        IHexMapGeneratorRepo hexMapGeneratorRepo,
        ITileRepo tileRepo,
        ITileSearchService tileSearchService)
    {
        _erosionLandGeneratorService = erosionLandGeneratorService;
        _erosionLandGeneratorRepo = erosionLandGeneratorRepo;
        _erosionLandGeneratorRepo.Ready += OnReady;
        _erosionLandGeneratorRepo.TreeExiting += OnTreeExiting;
        _hexMapGeneratorRepo = hexMapGeneratorRepo;
        _tileRepo = tileRepo;
        _tileSearchService = tileSearchService;
    }

    public void ReleaseEvents()
    {
        _erosionLandGeneratorRepo.Ready -= OnReady;
        _erosionLandGeneratorRepo.TreeExiting -= OnTreeExiting;
    }

    private void OnReady()
    {
        _hexMapGeneratorRepo.CreatingErosionLand += CreateLand;
        _hexMapGeneratorRepo.ErodingLand += _erosionLandGeneratorService.ErodeLand;
    }

    private void OnTreeExiting()
    {
        _hexMapGeneratorRepo.CreatingErosionLand -= CreateLand;
        _hexMapGeneratorRepo.ErodingLand -= _erosionLandGeneratorService.ErodeLand;
    }

    private IErosionLandGenerator Self => _erosionLandGeneratorRepo.Singleton!;

    private int CreateLand(RandomNumberGenerator random, List<MapRegion> regions)
    {
        var landTileCount = Mathf.RoundToInt(_tileRepo.GetCount() * Self.LandPercentage * 0.01f);
        var landBudget = landTileCount;
        // 根据地图尺寸来设置对应循环次数上限，保证大地图也能尽量用完 landBudget
        for (var guard = 0; guard < landTileCount; guard++) // 防止无限循环的守卫值
        {
            var sink = random.Randf() < Self.SinkProbability;
            foreach (var region in regions)
            {
                var chunkSize = random.RandiRange(Self.ChunkSizeMin, Self.ChunkSizeMax);
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
        var rise = random.Randf() < Self.HighRiseProbability ? 2 : 1;
        return _tileSearchService.RaiseTerrain(chunkSize, budget, firstTileId, rise, random, Self.JitterProbability);
    }

    private int SinkTerrain(RandomNumberGenerator random, int chunkSize, int budget, MapRegion region)
    {
        var firstTileId = GetRandomCellIndex(region);
        var sink = random.Randf() < Self.HighRiseProbability ? 2 : 1;
        return _tileSearchService.SinkTerrain(chunkSize, budget, firstTileId, sink, random, Self.JitterProbability);
    }

    private int GetRandomCellIndex(MapRegion region)
    {
        return GD.RandRange(1, _tileRepo.GetCount());
    }
}