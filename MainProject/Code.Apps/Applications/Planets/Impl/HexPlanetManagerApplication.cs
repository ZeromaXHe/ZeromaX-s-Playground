using Domains.Models.Entities.Civs;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Caches;
using Domains.Repos.Civs;
using Domains.Repos.PlanetGenerates;
using Domains.Services.Navigations;
using Domains.Services.Shaders;
using Domains.Services.Uis;
using Godot;

namespace Apps.Applications.Planets.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-14 16:00:57
public class HexPlanetManagerApplication(
    ILodMeshCache lodMeshCache,
    IChunkRepo chunkRepo,
    ITileRepo tileRepo,
    IPointRepo pointRepo,
    IFaceRepo faceRepo,
    ICivRepo civRepo,
    ITileSearchService tileSearchService,
    ITileShaderService tileShaderService,
    ISelectViewService selectViewService) : IHexPlanetManagerApplication
{
    public void ClearOldData()
    {
        chunkRepo.Truncate();
        tileRepo.Truncate();
        pointRepo.Truncate();
        faceRepo.Truncate();
        civRepo.Truncate();
        selectViewService.ClearPath();
        lodMeshCache.RemoveAllLodMeshes();
    }
    
    public void RefreshAllTiles()
    {
        foreach (var tile in tileRepo.GetAll())
        {
            tileSearchService.RefreshTileSearchData(tile.Id);
            tileShaderService.RefreshTerrain(tile.Id);
            tileShaderService.RefreshVisibility(tile.Id);
        }
    }
    
    public void InitCivilization()
    {
        // 在可见分块的陆地分块中随机
        var tiles = chunkRepo.GetAll()
            .Where(c => c.Insight)
            .SelectMany(c => c.TileIds)
            .Select(id => tileRepo.GetById(id)!)
            .Where(t => !t.Data.IsUnderwater)
            .ToList();
        for (var i = 0; i < 8; i++)
        {
            var idx = GD.RandRange(0, tiles.Count - 1);
            var tile = tiles[idx];
            var civ = civRepo.Add(new Color(
                Mathf.Lerp(0.3f, 1f, GD.Randf()),
                Mathf.Lerp(0.3f, 1f, GD.Randf()),
                Mathf.Lerp(0.3f, 1f, GD.Randf())));
            UpdateTileCivId(tile, civ);
            tiles[idx] = tiles[^1];
            tiles.RemoveAt(tiles.Count - 1);
        }
    }
    
    public void UpdateCivTerritory()
    {
        foreach (var civ in civRepo.GetAll())
        {
            var tile = tileRepo.GetById(civ.TileIds[GD.RandRange(0, civ.TileIds.Count - 1)])!;
            var conquerTile = tileRepo.GetNeighbors(tile).FirstOrDefault(n => !n.Data.IsUnderwater && n.CivId <= 0);
            if (conquerTile == null) continue;
            UpdateTileCivId(conquerTile, civ);
        }
    }
    
    private void UpdateTileCivId(Tile tile, Civ civ)
    {
        civ.TileIds.Add(tile.Id);
        tile.CivId = civ.Id;
        tileShaderService.RefreshCiv(tile.Id);
    }
}