using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;

public interface IChunkRepo : IRepository<Chunk>
{
    #region 事件

    delegate void RefreshTileLabelEvent(int chunkId, int tileId, string text);

    event RefreshTileLabelEvent RefreshChunkTileLabel;

    void RefreshTileLabel(Tile tile, string text);

    #endregion

    Chunk Add(int centerId, Vector3 pos, List<int> hexFaceIds, List<int> neighborCenterIds);
    Chunk GetByCenterId(int centerId);
    IEnumerable<Chunk> GetNeighbors(Chunk chunk);
    Chunk GetNeighborByIdx(Chunk chunk, int idx);

    // 更新分块的显示级别
    void UpdateChunkInsightAndLod(int id, bool insight, ChunkLod lod);
}