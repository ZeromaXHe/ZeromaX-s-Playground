using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Impl;

public class ChunkRepo : Repository<Chunk>, IChunkRepo
{
    #region 事件

    public event IChunkRepo.RefreshTileLabelEvent RefreshChunkTileLabel;
    
    public void RefreshTileLabel(Tile tile, string text)
    {
        var chunk = GetById(tile.ChunkId);
        RefreshChunkTileLabel?.Invoke(chunk.Id, tile.Id, text);
    }

    #endregion

    private readonly Dictionary<int, int> _centerIdIndex = new();

    public Chunk Add(int centerId, Vector3 pos, List<int> hexFaceIds, List<int> neighborCenterIds) =>
        Add(id => new Chunk(centerId, pos, hexFaceIds, neighborCenterIds, id));

    protected override void AddHook(Chunk entity) => _centerIdIndex.Add(entity.CenterId, entity.Id);
    protected override void TruncateHook() => _centerIdIndex.Clear();

    public Chunk GetByCenterId(int centerId) =>
        _centerIdIndex.TryGetValue(centerId, out var tileId) ? GetById(tileId) : null;

    public IEnumerable<Chunk> GetNeighbors(Chunk chunk) =>
        chunk.NeighborCenterIds.Select(GetByCenterId);

    public Chunk GetNeighborByIdx(Chunk chunk, int idx) =>
        idx >= 0 && idx < chunk.NeighborCenterIds.Count
            ? GetByCenterId(chunk.NeighborCenterIds[idx])
            : null;

    public void UpdateChunkInsightAndLod(int id, bool insight, ChunkLod lod)
    {
        var chunk = GetById(id);
        if (chunk == null) return;
        chunk.Insight = insight;
        chunk.Lod = lod;
    }
}