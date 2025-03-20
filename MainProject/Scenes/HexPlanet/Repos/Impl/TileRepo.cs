using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Impl;

public class TileRepo : Repository<Tile>, ITileRepo
{
    private readonly Dictionary<int, int> _centerIdIndex = new();

    public Tile Add(int centerId, int chunkId, Vector3 unitCentroid,
        List<int> hexFaceIds, List<int> neighborCenterIds) =>
        Add(id => new Tile(centerId, chunkId, unitCentroid, hexFaceIds, neighborCenterIds, id));

    protected override void AddHook(Tile tile) => _centerIdIndex.Add(tile.CenterId, tile.Id);
    protected override void TruncateHook() => _centerIdIndex.Clear();

    public Tile GetByCenterId(int centerId) =>
        _centerIdIndex.TryGetValue(centerId, out var tileId) ? GetById(tileId) : null;
}