using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Services.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public interface ITileService
{
    // 最近邻搜索 Tile id
    int? SearchNearestTileId(Vector3 pos);

    // 初始化地块
    void InitTiles();
}