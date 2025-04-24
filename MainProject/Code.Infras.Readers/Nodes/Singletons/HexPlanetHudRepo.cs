using Commons.Constants;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 12:23:18
public class HexPlanetHudRepo : SingletonNodeRepo<IHexPlanetHud>, IHexPlanetHudRepo
{
    public event IHexPlanetHudRepo.LabelModeChangedEvent? LabelModeChanged;
    public event IHexPlanetHudRepo.EditModeChangedEvent? EditModeChanged;
    public event Action<Tile?>? ChosenTileChanged;
    private void OnChosenTileChanged(Tile? tile) => ChosenTileChanged?.Invoke(tile);

    protected override void ConnectNodeEvents()
    {
        Singleton!.ChosenTileChanged += OnChosenTileChanged;
    }

    protected override void DisconnectNodeEvents()
    {
        Singleton!.ChosenTileChanged -= OnChosenTileChanged;
    }

    private IHexPlanetHud HexPlanetHud => Singleton!;

    private HexTileDataOverrider TileOverrider
    {
        get => HexPlanetHud.TileOverrider;
        set => HexPlanetHud.TileOverrider = value;
    }
    
    #region 编辑

    public int GetLabelMode() => IsRegistered() ? HexPlanetHud.LabelMode : 0;
    public bool GetEditMode() => !IsRegistered() || HexPlanetHud.TileOverrider.EditMode;

    public HexTileDataOverrider GetTileOverrider() => TileOverrider;

    public void SetEditMode(bool toggle)
    {
        var editMode = HexPlanetHud.TileOverrider.EditMode;
        HexPlanetHud.TileOverrider = HexPlanetHud.TileOverrider with { EditMode = toggle };
        if (toggle != editMode)
            EditModeChanged?.Invoke(toggle);
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexMapEditMode, toggle);
    }

    public void SetLabelMode(long mode)
    {
        var before = HexPlanetHud.LabelMode;
        var intMode = (int)mode;
        HexPlanetHud.LabelMode = intMode;
        if (before != intMode)
            LabelModeChanged?.Invoke(intMode);
    }

    public void SetTerrain(long index)
    {
        TileOverrider = TileOverrider with { ApplyTerrain = index > 0 };
        if (TileOverrider.ApplyTerrain)
        {
            TileOverrider = TileOverrider with { ActiveTerrain = (int)index - 1 };
        }
    }

    public void SetApplyTerrain(bool toggle) => TileOverrider = TileOverrider with { ApplyTerrain = toggle };

    public void SetElevation(double elevation)
    {
        TileOverrider = TileOverrider with { ActiveElevation = (int)elevation };
        HexPlanetHud.ElevationValueLabel!.Text = HexPlanetHud.TileOverrider.ActiveElevation.ToString();
    }

    public void SetApplyElevation(bool toggle) => TileOverrider = TileOverrider with { ApplyElevation = toggle };

    public void SetBrushSize(double brushSize)
    {
        TileOverrider = TileOverrider with { BrushSize = (int)brushSize };
        HexPlanetHud.BrushLabel!.Text = $"笔刷大小：{HexPlanetHud.TileOverrider.BrushSize}";
    }

    public void SetRiverMode(long mode) => TileOverrider = TileOverrider with { RiverMode = (OptionalToggle)mode };
    public void SetRoadMode(long mode) => TileOverrider = TileOverrider with { RoadMode = (OptionalToggle)mode };
    public void SetApplyWaterLevel(bool toggle) => TileOverrider = TileOverrider with { ApplyWaterLevel = toggle };

    public void SetWaterLevel(double level)
    {
        TileOverrider = TileOverrider with { ActiveWaterLevel = (int)level };
        HexPlanetHud.WaterValueLabel!.Text = HexPlanetHud.TileOverrider.ActiveWaterLevel.ToString();
    }

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