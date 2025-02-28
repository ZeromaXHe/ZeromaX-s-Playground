using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface ITileService
{
    delegate void RefreshTileEvent(int id);

    event RefreshTileEvent RefreshTile;
    #region 透传存储库方法

    Tile GetById(int id);
    Tile GetByCenterId(int centerId);
    int GetCount();
    IEnumerable<Tile> GetAll();

    #endregion

    #region 修改 Tile 属性的方法（相当于 Update）

    void SetHeight(Tile tile, float height);
    void SetElevation(Tile tile, int elevation);
    void SetTerrainTypeIndex(Tile tile, int idx);
    void SetWaterLevel(Tile tile, int waterLevel);
    void SetUrbanLevel(Tile tile, int urbanLevel);
    void SetFarmLevel(Tile tile, int farmLevel);
    void SetPlantLevel(Tile tile, int plantLevel);
    void SetWalled(Tile tile, bool walled);
    void SetSpecialIndex(Tile tile, int specialIndex);

    #endregion

    // 最近邻搜索 Tile id
    int? SearchNearestTileId(Vector3 pos);

    // 单位高度
    float UnitHeight { get; set; }
    float GetHeight(Tile tile);
    float GetHeightById(int id);
    void ClearData();

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
    Vector3 GetCenter(Tile tile, float radius);

    #region 邻居

    IEnumerable<Tile> GetNeighbors(Tile tile);

    /// <summary>
    /// 根据地块的相邻的顶点 Face 索引，获取该方向上共边的邻居。
    /// </summary>
    /// <param name="tile">地块</param>
    /// <param name="idx">顶点 Face 索引</param>
    /// <returns>一个共边的邻居</returns>
    Tile GetNeighborByIdx(Tile tile, int idx);

    int GetNeighborIdIdx(Tile tile, int neighborId);
    bool IsNeighbor(Tile tile1, Tile tile2);
    IEnumerable<Tile> GetTilesInDistance(Tile tile, int dist);
    List<Vector3> GetNeighborCommonCorners(Tile tile, Tile neighbor, float radius = 1f);

    /// <summary>
    /// 根据地块的顶点 Face 索引，获取该方向上的共角落的两个邻居。
    /// </summary>
    /// <param name="tile">地块</param>
    /// <param name="idx">顶点 Face 索引</param>
    /// <returns>两个共角落的邻居（如果过滤，则为一个）</returns>
    List<Tile> GetCornerNeighborsByIdx(Tile tile, int idx);

    #endregion

    #region 河流

    void RemoveRiver(Tile tile);
    void SetOutgoingRiver(Tile tile, Tile riverToTile);
    float GetStreamBedHeight(Tile tile);
    bool HasRiverThroughEdge(Tile tile, int idx);
    float GetRiverSurfaceHeight(Tile tile);
    int GetRiverBeginOrEndIdx(Tile tile);

    #endregion

    #region 道路

    void AddRoad(Tile tile, Tile neighbor);
    void RemoveRoads(Tile tile);

    #endregion

    #region 水面

    float GetWaterSurfaceHeight(Tile tile);
    Vector3 GetFirstWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f);
    Vector3 GetSecondWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f);

    #endregion

    // TODO: 感觉放在这里有点不太合适，未来调整
    float GetWallHeight();
    float GetWallThickness();

    void UpdateTileLabel(Tile tile, string text);
}