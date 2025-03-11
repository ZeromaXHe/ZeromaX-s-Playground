using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo.Impl;

public class ChunkRepo : Repository<Chunk>, IChunkRepo
{
    private readonly Dictionary<int, int> _centerIdIndex = new();

    public Chunk Add(int centerId, Vector3 pos, List<int> hexFaceIds, List<int> neighborCenterIds) =>
        Add(id => new Chunk(centerId, pos, hexFaceIds, neighborCenterIds, id));

    protected override void AddHook(Chunk entity) => _centerIdIndex.Add(entity.CenterId, entity.Id);
    protected override void TruncateHook() => _centerIdIndex.Clear();

    public Chunk GetByCenterId(int centerId) =>
        _centerIdIndex.TryGetValue(centerId, out var tileId) ? GetById(tileId) : null;
}