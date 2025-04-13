using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Navigations;
using Domains.Services.Shaders;

namespace Apps.Applications.Tiles.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 07:23:22
public class TileShaderApplication(ITileSearchService tileSearchService, ITileShaderService tileShaderService)
    : ITileShaderApplication
{
    public void IncreaseVisibility(Tile fromTile, int range)
    {
        var tiles = tileSearchService.GetVisibleTiles(fromTile, range);
        foreach (var tile in tiles)
            tileShaderService.IncreaseVisibility(tile);
    }

    public void DecreaseVisibility(Tile fromTile, int range)
    {
        var tiles = tileSearchService.GetVisibleTiles(fromTile, range);
        foreach (var tile in tiles)
            tileShaderService.DecreaseVisibility(tile);
    }
}