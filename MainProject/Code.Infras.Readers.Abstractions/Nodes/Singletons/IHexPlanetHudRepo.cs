using Domains.Models.ValueObjects.PlanetGenerates;
using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Abstractions.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 12:21:18
public interface IHexPlanetHudRepo : ISingletonNodeRepo<IHexPlanetHud>
{
    delegate void LabelModeChangedEvent(int labelMode);

    event LabelModeChangedEvent LabelModeChanged;

    delegate void EditModeChangedEvent(bool editMode);

    event EditModeChangedEvent EditModeChanged;

    #region 编辑

    int GetLabelMode();
    bool GetEditMode();
    HexTileDataOverrider GetTileOverrider();
    void SetEditMode(bool toggle);
    void SetLabelMode(long mode);
    void SetTerrain(long index);
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