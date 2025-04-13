using Apps.Models.Responses;
using Domains.Models.Entities.PlanetGenerates;
using Godot;

namespace Apps.Applications.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 13:00:13
public interface IHexPlanetHudApplication
{
    PlanetInfoRespDto GetPlanetInfo();
    TileInfoRespDto GetTileInfo(Tile tile);
    void EditTiles(Tile tile, bool isDrag, Tile? previousTile, Tile dragTile);
    int GetElevationStep();
    ImageTexture GenerateRectMap();

    #region 编辑功能

    void SetEditMode(bool toggle);
    bool GetEditMode();
    void SetLabelMode(long mode);
    void SetTerrain(long index);
    void SetApplyTerrain(bool toggle);
    void SetElevation(double elevation);
    int GetActiveElevation();
    void SetApplyElevation(bool toggle);
    void SetBrushSize(double brushSize);
    int GetBrushSize();
    void SetRiverMode(long mode);
    void SetRoadMode(long mode);
    void SetApplyWaterLevel(bool toggle);
    void SetWaterLevel(double level);
    int GetActiveWaterLevel();
    void SetApplyUrbanLevel(bool toggle);
    void SetUrbanLevel(double level);
    void SetApplyFarmLevel(bool toggle);
    void SetFarmLevel(double level);
    void SetApplyPlantLevel(bool toggle);
    void SetPlantLevel(double level);
    void SetWalledMode(long mode);
    void SetApplySpecialIndex(bool toggle);
    void SetSpecialIndex(long index);

    #endregion
}