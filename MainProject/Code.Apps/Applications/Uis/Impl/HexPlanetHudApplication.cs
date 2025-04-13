using Apps.Models.Responses;
using Commons.Constants;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Planets;
using Domains.Repos.PlanetGenerates;
using Domains.Services.Uis;
using Godot;

namespace Apps.Applications.Uis.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 13:00:56
public class HexPlanetHudApplication(
    IPlanetConfig planetConfig,
    IChunkRepo chunkRepo,
    ITileRepo tileRepo,
    IPointRepo pointRepo,
    IEditorService editorService,
    IMiniMapService miniMapService)
    : IHexPlanetHudApplication
{
    public PlanetInfoRespDto GetPlanetInfo() =>
        new(planetConfig.Radius, planetConfig.Divisions, planetConfig.ChunkDivisions,
            chunkRepo.GetCount(), tileRepo.GetCount());

    public TileInfoRespDto GetTileInfo(Tile tile) => new(pointRepo.GetSphereAxial(tile), tileRepo.GetHeight(tile));

    public void EditTiles(Tile tile, bool isDrag, Tile? previousTile, Tile dragTile) =>
        editorService.EditTiles(tile, isDrag, previousTile, dragTile);

    public int GetElevationStep() => planetConfig.ElevationStep;
    public ImageTexture GenerateRectMap() => miniMapService.GenerateRectMap();

    #region 编辑功能

    public void SetEditMode(bool toggle)
    {
        editorService.SetEditMode(toggle);
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexMapEditMode, toggle);
    }

    public bool GetEditMode() => editorService.TileOverrider.EditMode;
    public void SetLabelMode(long mode) => editorService.SetLabelMode(mode);
    public void SetTerrain(long index) => editorService.SetTerrain(index);
    public void SetApplyTerrain(bool toggle) => editorService.SetApplyTerrain(toggle);
    public void SetElevation(double elevation) => editorService.SetElevation(elevation);
    public int GetActiveElevation() => editorService.TileOverrider.ActiveElevation;
    public void SetApplyElevation(bool toggle) => editorService.SetApplyElevation(toggle);
    public void SetBrushSize(double brushSize) => editorService.SetBrushSize(brushSize);
    public int GetBrushSize() => editorService.TileOverrider.BrushSize;
    public void SetRiverMode(long mode) => editorService.SetRiverMode(mode);
    public void SetRoadMode(long mode) => editorService.SetRoadMode(mode);
    public void SetApplyWaterLevel(bool toggle) => editorService.SetApplyWaterLevel(toggle);
    public void SetWaterLevel(double level) => editorService.SetWaterLevel(level);
    public int GetActiveWaterLevel() => editorService.TileOverrider.ActiveWaterLevel;
    public void SetApplyUrbanLevel(bool toggle) => editorService.SetApplyUrbanLevel(toggle);
    public void SetUrbanLevel(double level) => editorService.SetUrbanLevel(level);
    public void SetApplyFarmLevel(bool toggle) => editorService.SetApplyFarmLevel(toggle);
    public void SetFarmLevel(double level) => editorService.SetFarmLevel(level);
    public void SetApplyPlantLevel(bool toggle) => editorService.SetApplyPlantLevel(toggle);
    public void SetPlantLevel(double level) => editorService.SetPlantLevel(level);
    public void SetWalledMode(long mode) => editorService.SetWalledMode(mode);
    public void SetApplySpecialIndex(bool toggle) => editorService.SetApplySpecialIndex(toggle);
    public void SetSpecialIndex(long index) => editorService.SetSpecialIndex(index);

    #endregion
}