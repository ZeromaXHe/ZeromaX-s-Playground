using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface ITileService
{
    #region 透传存储库方法

    Tile GetById(int id);
    Tile GetByCenterId(int centerId);
    int GetCount();

    #endregion

    // 最近邻搜索 Tile id
    int? SearchNearestTileId(Vector3 pos);

    // 单位高度
    float UnitHeight { get; set; }
    float GetHeight(Tile tile);
    float GetHeightById(int id);
    float SetHeight(Tile tile, float height);
    void ClearData();

    // 初始化地块
    void InitTiles();

    // 获取地块的形状角落顶点（顺时针顺序）
    IEnumerable<Vector3> GetCorners(Tile tile, float radius, float size = 1f);
    Vector3 GetCorner(Tile tile, int idx, float radius = 1f, float size = 1f);
    Vector3 GetCornerByFaceId(Tile tile, int id, float radius = 1f, float size = 1f);
    Vector3 GetCenter(Tile tile, float radius);
    IEnumerable<Tile> GetNeighbors(Tile tile);
    Tile GetNeighborByIdx(Tile tile, int idx);
    IEnumerable<Tile> GetTilesInDistance(Tile tile, int dist);
    List<Vector3> GetNeighborCommonCorners(Tile tile, Tile neighbor, float radius = 1f);

    /// <summary>
    /// 根据地块的相邻的两个顶点 Face 索引，获取该方向上共边的邻居。
    /// </summary>
    /// <param name="tile">地块</param>
    /// <param name="idx1">顶点 Face 索引 1</param>
    /// <param name="idx2">顶点 Face 索引 2</param>
    /// <returns>一个共边的邻居</returns>
    Tile GetNeighborByDirection(Tile tile, int idx1, int idx2);

    /// <summary>
    /// 根据地块的顶点 Face 索引，获取该方向上的共角落的两个邻居。
    /// </summary>
    /// <param name="tile">地块</param>
    /// <param name="idx">顶点 Face 索引</param>
    /// <param name="filterNeighborId">需要过滤掉（不返回）的邻居 id</param>
    /// <returns>两个共角落的邻居（如果过滤，则为一个）</returns>
    List<Tile> GetNeighborsByDirection(Tile tile, int idx, int filterNeighborId = -1);
}