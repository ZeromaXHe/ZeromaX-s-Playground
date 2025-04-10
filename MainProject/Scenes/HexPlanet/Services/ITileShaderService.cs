using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 09:14
public interface ITileShaderService
{
    delegate void TileExploredEvent(int tileId);

    event TileExploredEvent TileExplored;
    void Initialize();
    void RefreshCiv(int tileId);
    void RefreshTerrain(int tileId);
    void RefreshVisibility(int tileId);
    void IncreaseVisibility(Tile fromTile, int range);
    void DecreaseVisibility(Tile fromTile, int range);
    void UpdateData(float delta);
}