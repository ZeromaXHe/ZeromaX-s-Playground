using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Services.Abstractions.Uis;
using Infras.Writers.Abstractions.PlanetGenerates;

namespace Domains.Services.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-12 09:18
public class EditorService(ITileRepo tileRepo) : IEditorService
{
    public event IEditorService.LabelModeChangedEvent? LabelModeChanged;
    public event IEditorService.EditModeChangedEvent? EditModeChanged;

    private int _labelMode;

    public int LabelMode
    {
        get => _labelMode;
        set
        {
            var before = _labelMode;
            _labelMode = value;
            if (before != _labelMode)
                LabelModeChanged?.Invoke(_labelMode);
        }
    }

    private HexTileDataOverrider _tileOverrider = new();

    public HexTileDataOverrider TileOverrider
    {
        get => _tileOverrider;
        set
        {
            var editMode = _tileOverrider.EditMode;
            _tileOverrider = value;
            if (_tileOverrider.EditMode != editMode)
                EditModeChanged?.Invoke(_tileOverrider.EditMode);
        }
    }

    public void EditTiles(Tile tile, bool isDrag, Tile? previousTile, Tile? dragTile)
    {
        foreach (var t in tileRepo.GetTilesInDistance(tile, TileOverrider.BrushSize))
            EditTile(t, isDrag, previousTile, dragTile);
    }

    private void EditTile(Tile tile, bool isDrag, Tile? previousTile, Tile? dragTile)
    {
        if (TileOverrider.ApplyTerrain)
            tileRepo.SetTerrainTypeIndex(tile, TileOverrider.ActiveTerrain);
        if (TileOverrider.ApplyElevation)
            tileRepo.SetElevation(tile, TileOverrider.ActiveElevation);
        if (TileOverrider.ApplyWaterLevel)
            tileRepo.SetWaterLevel(tile, TileOverrider.ActiveWaterLevel);
        if (TileOverrider.ApplySpecialIndex)
            tileRepo.SetSpecialIndex(tile, TileOverrider.ActiveSpecialIndex);
        if (TileOverrider.ApplyUrbanLevel)
            tileRepo.SetUrbanLevel(tile, TileOverrider.ActiveUrbanLevel);
        if (TileOverrider.ApplyFarmLevel)
            tileRepo.SetFarmLevel(tile, TileOverrider.ActiveFarmLevel);
        if (TileOverrider.ApplyPlantLevel)
            tileRepo.SetPlantLevel(tile, TileOverrider.ActivePlantLevel);
        if (TileOverrider.RiverMode == OptionalToggle.No)
            tileRepo.RemoveRiver(tile);
        if (TileOverrider.RoadMode == OptionalToggle.No)
            tileRepo.RemoveRoads(tile);
        if (TileOverrider.WalledMode != OptionalToggle.Ignore)
            tileRepo.SetWalled(tile, TileOverrider.WalledMode == OptionalToggle.Yes);
        if (isDrag)
        {
            if (TileOverrider.RiverMode == OptionalToggle.Yes)
                tileRepo.SetOutgoingRiver(previousTile!, dragTile!);
            if (TileOverrider.RoadMode == OptionalToggle.Yes)
                tileRepo.AddRoad(previousTile!, dragTile!);
        }
    }

    #region 编辑

    public void SetEditMode(bool toggle) => TileOverrider = TileOverrider with { EditMode = toggle };
    public void SetLabelMode(long mode) => LabelMode = (int)mode;

    public void SetTerrain(long index)
    {
        TileOverrider = TileOverrider with { ApplyTerrain = index > 0 };
        if (TileOverrider.ApplyTerrain)
        {
            TileOverrider = TileOverrider with { ActiveTerrain = (int)index - 1 };
        }
    }
    
    public void SetApplyTerrain(bool toggle) => TileOverrider = TileOverrider with { ApplyTerrain = toggle };

    public void SetElevation(double elevation) =>
        TileOverrider = TileOverrider with { ActiveElevation = (int)elevation };

    public void SetApplyElevation(bool toggle) => TileOverrider = TileOverrider with { ApplyElevation = toggle };
    public void SetBrushSize(double brushSize) => TileOverrider = TileOverrider with { BrushSize = (int)brushSize };
    public void SetRiverMode(long mode) => TileOverrider = TileOverrider with { RiverMode = (OptionalToggle)mode };
    public void SetRoadMode(long mode) => TileOverrider = TileOverrider with { RoadMode = (OptionalToggle)mode };
    public void SetApplyWaterLevel(bool toggle) => TileOverrider = TileOverrider with { ApplyWaterLevel = toggle };
    public void SetWaterLevel(double level) => TileOverrider = TileOverrider with { ActiveWaterLevel = (int)level };
    public void SetApplyUrbanLevel(bool toggle) => TileOverrider = TileOverrider with { ApplyUrbanLevel = toggle };
    public void SetUrbanLevel(double level) => TileOverrider = TileOverrider with { ActiveUrbanLevel = (int)level };
    public void SetApplyFarmLevel(bool toggle) => TileOverrider = TileOverrider with { ApplyFarmLevel = toggle };
    public void SetFarmLevel(double level) => TileOverrider = TileOverrider with { ActiveFarmLevel = (int)level };
    public void SetApplyPlantLevel(bool toggle) => TileOverrider = TileOverrider with { ApplyPlantLevel = toggle };
    public void SetPlantLevel(double level) => TileOverrider = TileOverrider with { ActivePlantLevel = (int)level };
    public void SetWalledMode(long mode) => TileOverrider = TileOverrider with { WalledMode = (OptionalToggle)mode };
    public void SetApplySpecialIndex(bool toggle) => TileOverrider = TileOverrider with { ApplySpecialIndex = toggle };
    public void SetSpecialIndex(long index) => TileOverrider = TileOverrider with { ActiveSpecialIndex = (int)index };

    #endregion
}