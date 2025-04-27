using Commons.Utils;
using Domains.Services.Abstractions.Nodes.IdInstances;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.Civs;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Domains.Services.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-27 16:03:31
public class HexUnitService(IHexPlanetManagerRepo hexPlanetManagerRepo, ITileRepo tileRepo, IUnitRepo unitRepo)
    : IHexUnitService
{
    public void Travel(IHexUnit unit, IHexUnitPath path)
    {
        unit.Path = path;
        unit.PathOriented = false;
        unit.PathTileIdx = 0;
        // 提前把实际单位数据设置到目标 Tile 中
        var fromTile = tileRepo.GetById(unit.TileId)!;
        tileRepo.SetUnitId(fromTile, 0);
        var toTile = unit.Path.Tiles![^1];
        tileRepo.SetUnitId(toTile, unit.Id);
        unitRepo.GetById(unit.Id)!.TileId = toTile.Id;
        unit.SetTravelTileId(toTile.Id);
    }

    public void ValidateLocation(IHexUnit unit)
    {
        var tile = tileRepo.GetById(unit.TileId)!;
        var position = tile.GetCentroid(hexPlanetManagerRepo.Radius + hexPlanetManagerRepo.GetHeight(tile));
        Node3dUtil.PlaceOnSphere(unit, position, hexPlanetManagerRepo.StandardScale,
            alignForward: Vector3.Up); // 单位不需要抬高，场景里已经设置好了
        unit.BeginRotation = unit.Rotation;
    }
}