using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Infras.Writers.Abstractions.Bases;

namespace Infras.Writers.Abstractions.PlanetGenerates;

public interface ITileRepo : IRepository<Tile>
{
    #region 事件

    delegate void RefreshChunkEvent(int id);

    event RefreshChunkEvent RefreshChunk;

    delegate void UnitValidateLocationEvent(int unitId);

    event UnitValidateLocationEvent UnitValidateLocation;

    delegate void RefreshTerrainShaderEvent(int tileId);

    event RefreshTerrainShaderEvent RefreshTerrainShader;

    delegate void ViewElevationChangedEvent(int tileId);

    event ViewElevationChangedEvent ViewElevationChanged;

    #endregion

    Tile Add(int centerId, int chunkId, Vector3 unitCentroid, List<Vector3> unitCorners, List<int> hexFaceIds, List<int> neighborCenterIds);
    Tile? GetByCenterId(int centerId);


    #region 修改 Tile 属性的方法（相当于 Update）

    void SetElevation(Tile tile, int elevation);
    void SetTerrainTypeIndex(Tile tile, int idx);
    void SetWaterLevel(Tile tile, int waterLevel);
    void SetUrbanLevel(Tile tile, int urbanLevel);
    void SetFarmLevel(Tile tile, int farmLevel);
    void SetPlantLevel(Tile tile, int plantLevel);
    void SetWalled(Tile tile, bool walled);
    void SetSpecialIndex(Tile tile, int specialIndex);
    void SetUnitId(Tile tile, int unitId);

    #endregion

    #region 高度

    float GetHeight(Tile tile);
    float GetOverrideHeight(Tile tile, HexTileDataOverrider tileDataOverrider);
    float GetHeightById(int id);

    #endregion

    #region 邻居

    IEnumerable<Tile> GetNeighbors(Tile tile);

    /// <summary>
    /// 根据地块的相邻的顶点 Face 索引，获取该方向上共边的邻居。
    /// </summary>
    /// <param name="tile">地块</param>
    /// <param name="idx">顶点 Face 索引</param>
    /// <returns>一个共边的邻居</returns>
    Tile? GetNeighborByIdx(Tile tile, int idx);

    int GetNeighborIdIdx(Tile tile, int neighborId);
    IEnumerable<Tile> GetTilesInDistance(Tile tile, int dist);

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

    #endregion

    #region 道路

    void AddRoad(Tile tile, Tile neighbor);
    void RemoveRoads(Tile tile);

    #endregion
}