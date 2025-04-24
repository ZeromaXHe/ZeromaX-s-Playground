using Commons.Utils.HexSphereGrid;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.Singletons;
using Domains.Services.Abstractions.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Domains.Services.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:23:23
public class MiniMapManagerService(
    IMiniMapManagerRepo miniMapManagerRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    ITileRepo tileRepo,
    IPointRepo pointRepo,
    ITileService tileService) : IMiniMapManagerService
{
    private IMiniMapManager Self => miniMapManagerRepo.Singleton!;

    public void Init(Vector3 orbitCamPos)
    {
        SyncCameraIconPos(orbitCamPos, 0f);
        UpdateCamera();
        Self.TerrainLayer!.Clear();
        Self.ColorLayer!.Clear();
        foreach (var tile in tileRepo.GetAll())
        {
            var sphereAxial = pointRepo.GetSphereAxial(tile);
            Self.TerrainLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, IMiniMapManagerService.TerrainAtlas(tile));
            switch (sphereAxial.Type)
            {
                case SphereAxial.TypeEnum.PoleVertices or SphereAxial.TypeEnum.MidVertices:
                    Self.ColorLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, new Vector2I(2, 3));
                    break;
                case SphereAxial.TypeEnum.EdgesSpecial
                    when sphereAxial.TypeIdx % 6 == 0 || sphereAxial.TypeIdx % 6 == 5:
                    Self.ColorLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, EdgeAtlas(sphereAxial));
                    break;
            }
        }
    }

    // 标准摄像机对应 Divisions = 10
    private static readonly Vector2 StandardCamPos = new(-345, 75);
    private static readonly Vector2 StandardCamZoom = new(0.4f, 0.4f);


    private void UpdateCamera()
    {
        Self.Camera!.Position = StandardCamPos / 10 * hexPlanetManagerRepo.Divisions;
        Self.Camera.Zoom = StandardCamZoom * 10 / hexPlanetManagerRepo.Divisions;
    }

    private static Vector2I? EdgeAtlas(SphereAxial sphereAxial)
    {
        return sphereAxial.Column switch
        {
            0 => new Vector2I(0, 3),
            1 => new Vector2I(0, 2),
            2 => new Vector2I(1, 3),
            3 => new Vector2I(1, 2),
            4 => new Vector2I(2, 2),
            _ => null
        };
    }

    // TODO: 这里用到了平级的 Service，需要重构
    // 同步相机标志的位置
    public void SyncCameraIconPos(Vector3 pos, float delta)
    {
        var tileId = tileService.SearchNearestTileId(pos);
        if (tileId == null)
        {
            GD.PrintErr($"未找到摄像机对应地块：{pos}");
            return;
        }

        var sa = pointRepo.GetSphereAxial(tileRepo.GetById((int)tileId)!);
        // TODO: 缓动，以及更精确的位置转换
        Self.CameraIcon!.GlobalPosition =
            Self.TerrainLayer!.ToGlobal(Self.TerrainLayer.MapToLocal(sa.Coords.ToVector2I()));
    }
}