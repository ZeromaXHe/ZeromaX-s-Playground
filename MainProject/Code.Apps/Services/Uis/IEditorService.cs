using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;

namespace Apps.Services.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-12 09:18
public interface IEditorService
{
    delegate void LabelModeChangedEvent(int labelMode);

    event LabelModeChangedEvent LabelModeChanged;

    delegate void EditModeChangedEvent(bool editMode);

    event EditModeChangedEvent EditModeChanged;
    int LabelMode { get; set; }
    HexTileDataOverrider TileOverrider { get; set; }
    void EditTiles(Tile tile, bool isDrag, Tile? previousTile, Tile dragTile);

    #region 编辑

    void SetEditMode(bool toggle);
    void SetLabelMode(long mode);
    void SelectTerrain(long index);
    void SetApplyTerrain(bool toggle);
    void SetElevation(double elevation);
    void SetApplyElevation(bool toggle);
    void SetBrushSize(double brushSize);
    void SetRiverMode(long mode);
    void SetRoadMode(long mode);
    void SetApplyWaterLevel(bool toggle);
    void SetWaterLevel(double level);
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