using Domains.Models.Entities.PlanetGenerates;

namespace Domains.Services.Shaders;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 09:14
public interface ITileShaderService
{
    void Initialize();
    void RefreshCiv(int tileId);
    void RefreshTerrain(int tileId);
    void RefreshVisibility(int tileId);
    void IncreaseVisibility(Tile tile);
    void DecreaseVisibility(Tile tile);
    void UpdateData(float delta);
}