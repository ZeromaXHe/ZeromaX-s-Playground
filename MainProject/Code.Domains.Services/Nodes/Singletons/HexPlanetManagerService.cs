using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.Singletons;
using Domains.Services.Abstractions.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Domains.Services.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:22:14
public class HexPlanetManagerService(
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    IHexPlanetHudRepo hexPlanetHudRepo,
    IUnitManagerRepo unitManagerRepo,
    ITileRepo tileRepo,
    ITileService tileService,
    IEditPreviewChunkService editPreviewChunkService)
    : IHexPlanetManagerService
{
    private IHexPlanetManager Self => hexPlanetManagerRepo.Singleton!;


    // TODO：因为使用了平级 Service，需要重构
    public Tile? GetTileUnderCursor()
    {
        var pos = Self.GetTileCollisionPositionUnderCursor();
        if (pos == Vector3.Zero) return null;
        var tileId = tileService.SearchNearestTileId(pos);
        return tileId == null ? null : tileRepo.GetById((int)tileId);
    }

    public bool UpdateUiInEditMode()
    {
        if (!hexPlanetHudRepo.GetTileOverrider().EditMode) return false;
        // 编辑模式下更新预览网格
        UpdateEditPreviewChunk();
        if (Input.IsActionJustPressed("destroy_unit"))
        {
            DestroyUnit();
            return true;
        }

        if (Input.IsActionJustPressed("create_unit"))
        {
            CreateUnit();
            return true;
        }

        return false;
    }

    // TODO: 由于使用了平级 Service，需要重构
    private void UpdateEditPreviewChunk()
    {
        var tile = GetTileUnderCursor();
        // 更新地块预览
        editPreviewChunkService.Update(tile);
    }

    private void CreateUnit()
    {
        var tile = GetTileUnderCursor();
        if (tile == null || tile.UnitId > 0)
        {
            GD.Print($"CreateUnit failed: tile {tile}, unitId: {tile?.UnitId}");
            return;
        }

        GD.Print($"CreateUnit at tile {tile.Id}");
        unitManagerRepo.Singleton!.AddUnit(tile.Id, GD.Randf() * Mathf.Tau);
    }

    private void DestroyUnit()
    {
        var tile = GetTileUnderCursor();
        if (tile is not { UnitId: > 0 })
            return;
        unitManagerRepo.Singleton!.RemoveUnit(tile.UnitId);
    }
}