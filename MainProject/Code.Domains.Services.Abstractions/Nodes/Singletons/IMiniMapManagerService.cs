using Domains.Models.Entities.PlanetGenerates;
using Godot;

namespace Domains.Services.Abstractions.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:03:18
public interface IMiniMapManagerService
{
    void Init(Vector3 orbitCamPos);
    void SyncCameraIconPos(Vector3 pos, float delta);

    public static Vector2I? TerrainAtlas(Tile tile)
    {
        if (tile.Data.IsUnderwater)
            return tile.Data.WaterLevel - tile.Data.Elevation > 1 ? new Vector2I(0, 1) : new Vector2I(1, 1);
        return tile.Data.TerrainTypeIndex switch
        {
            0 => new Vector2I(3, 0), // 0 沙漠
            1 => new Vector2I(0, 0), // 1 草原
            2 => new Vector2I(2, 0), // 2 泥地
            3 => new Vector2I(3, 1), // 3 岩石
            4 => new Vector2I(2, 1), // 4 雪地
            _ => null
        };
    }
}