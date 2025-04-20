using Domains.Models.Entities.PlanetGenerates;

namespace Domains.Services.Abstractions.Shaders;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 09:14
public interface ITileShaderService
{
    delegate void RangeVisibilityIncreasedEvent(Tile tile, int range);

    event RangeVisibilityIncreasedEvent? RangeVisibilityIncreased;

    // 对应第一次增加可视度（Visibility）
    delegate void TileExploredEvent(Tile tile);

    event TileExploredEvent? TileExplored;
    void Initialize();
    void RefreshCiv(int tileId);
    void RefreshTerrain(int tileId);
    void RefreshVisibility(int tileId);
    void IncreaseVisibility(Tile tile);
    void DecreaseVisibility(Tile tile);
    void UpdateData(float delta);
}