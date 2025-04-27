using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.IdInstances;
using Domains.Services.Abstractions.Nodes.Singletons.Planets;
using Domains.Services.Abstractions.Searches;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Writers.Abstractions.Civs;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions.Planets;

namespace Domains.Services.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:18:53
public class UnitManagerService(
    IUnitManagerRepo unitManagerRepo,
    IUnitRepo unitRepo,
    ITileRepo tileRepo,
    ITileSearchService tileSearchService,
    IHexUnitService hexUnitService) : IUnitManagerService
{
    private IUnitManager Self => unitManagerRepo.Singleton!;

    public void FindPath(Tile? tile)
    {
        if (Self.PathFromTileId != 0)
        {
            if (tile == null || tile.Id == Self.PathFromTileId)
            {
                // 重复点选同一地块，则取消选择
                Self.PathFromTileId = 0;
            }
            else MoveUnit(tile);
        }
        else
            // 当前没有选择地块（即没有选中单位）的话，则在有单位时选择该地块
            Self.PathFromTileId = tile == null || tile.UnitId == 0 ? 0 : tile.Id;
    }

    // TODO: 因为使用了平级 Service，后续需要重构！
    private void MoveUnit(Tile toTile)
    {
        var fromTile = tileRepo.GetById(Self.PathFromTileId)!;
        var path = tileSearchService.FindPath(fromTile, toTile, true);
        if (path is { Count: > 1 })
        {
            // 确实有找到从出发点到 tile 的路径
            var unit = Self.Units[fromTile.UnitId];
            var hexUnitPath = Self.GetHexUnitPathPool()!.NewTask(unit, path);
            hexUnitService.Travel(unit, hexUnitPath);
        }

        Self.PathFromTileId = 0;
    }

    public void ClearAllUnits()
    {
        if (!unitManagerRepo.IsRegistered())
            return;
        foreach (var unit in Self.Units.Values)
            unit.Die();
        Self.Units.Clear();
        unitRepo.Truncate();
    }
}