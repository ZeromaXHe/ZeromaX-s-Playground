using Domains.Models.Entities.PlanetGenerates;

namespace Apps.Queries.Abstractions.Tiles;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 07:23:13
public interface ITileShaderApplication
{
    void IncreaseVisibility(Tile fromTile, int range);
    void DecreaseVisibility(Tile fromTile, int range);
}