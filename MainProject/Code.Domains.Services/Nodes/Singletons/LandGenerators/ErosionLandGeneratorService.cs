using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.Singletons.LandGenerators;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons.LandGenerators;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions.LandGenerators;

namespace Domains.Services.Nodes.Singletons.LandGenerators;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:14:22
public class ErosionLandGeneratorService(IErosionLandGeneratorRepo erosionLandGeneratorRepo, ITileRepo tileRepo)
    : IErosionLandGeneratorService
{
    private IErosionLandGenerator Self => erosionLandGeneratorRepo.Singleton!;
    
    public void ErodeLand(RandomNumberGenerator random)
    {
        var erodibleTiles = tileRepo.GetAll().Where(IsErodible).ToList();
        var targetErodibleCount = (int)(erodibleTiles.Count * (100 - Self.ErosionPercentage) * 0.01f);
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

            foreach (var neighbor in tileRepo.GetNeighbors(tile))
            {
                if (neighbor.Data.Elevation == tile.Data.Elevation + 2 && !erodibleTiles.Contains(neighbor))
                    erodibleTiles.Add(neighbor);
            }

            if (IsErodible(targetTile) && !erodibleTiles.Contains(targetTile))
                erodibleTiles.Add(targetTile);
            foreach (var neighbor in tileRepo.GetNeighbors(targetTile))
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
        return tileRepo.GetNeighbors(tile)
            .Any(neighbor => neighbor.Data.Elevation <= erodibleElevation);
    }

    private Tile GetErosionTarget(RandomNumberGenerator random, Tile tile)
    {
        var erodibleElevation = tile.Data.Elevation - 2;
        var candidates = tileRepo.GetNeighbors(tile)
            .Where(neighbor => neighbor.Data.Elevation <= erodibleElevation)
            .ToList();
        return candidates[random.RandiRange(0, candidates.Count - 1)];
    }
}