using Domains.Bases;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Repos.PlanetGenerates;

public interface IChunkRepo : IRepository<Chunk>
{
    #region 事件

    delegate void RefreshTileLabelEvent(int chunkId, int tileId, string text);

    event RefreshTileLabelEvent RefreshChunkTileLabel;

    void RefreshTileLabel(Tile tile, string text);

    #endregion

    Chunk Add(int centerId, Vector3 pos, List<int> hexFaceIds, List<int> neighborCenterIds);
    Chunk? GetByCenterId(int centerId);
    IEnumerable<Chunk> GetNeighbors(Chunk chunk);
    Chunk? GetNeighborByIdx(Chunk chunk, int idx);

    // 更新分块的显示级别
    void UpdateChunkInsightAndLod(int id, bool insight, ChunkLod lod);
}