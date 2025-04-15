namespace Apps.Applications.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-14 16:00:14
public interface IHexPlanetManagerApplication
{
    void ClearOldData();
    void RefreshAllTiles();
    void InitCivilization();
    void UpdateCivTerritory();
}