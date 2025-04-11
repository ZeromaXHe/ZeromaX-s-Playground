using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public interface ITileService
{
    // 最近邻搜索 Tile id
    int? SearchNearestTileId(Vector3 pos);

    float GetHeight(Tile tile);
    float GetOverrideHeight(Tile tile, HexTileDataOverrider tileDataOverrider);
    float GetHeightById(int id);

    // 初始化地块
    void InitTiles();

    // 获取地块的形状角落顶点（顺时针顺序）
    IEnumerable<Vector3> GetCorners(Tile tile, float radius, float size = 1f);
    Vector3 GetCornerByFaceId(Tile tile, int id, float radius = 1f, float size = 1f);

    // 按照 tile 高度查询 idx (顺时针第一个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    Vector3 GetFirstCorner(Tile tile, int idx, float radius = 1f, float size = 1f);

    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    Vector3 GetSecondCorner(Tile tile, int idx, float radius = 1f, float size = 1f);

    // 按照 tile 高度查询 idx (顺时针第一个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    Vector3 GetFirstSolidCorner(Tile tile, int idx, float radius = 1f, float size = 1f);

    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    Vector3 GetSecondSolidCorner(Tile tile, int idx, float radius = 1f, float size = 1f);
    Vector3 GetEdgeMiddle(Tile tile, int idx, float radius = 1f, float size = 1f);
    Vector3 GetSolidEdgeMiddle(Tile tile, int idx, float radius = 1f, float size = 1f);

    #region 水面

    Vector3 GetFirstWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f);
    Vector3 GetSecondWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f);

    #endregion
}