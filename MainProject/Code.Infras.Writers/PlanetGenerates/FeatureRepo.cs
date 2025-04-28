using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using Infras.Writers.Abstractions.PlanetGenerates;
using Infras.Writers.Base;

namespace Infras.Writers.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-12 19:40:49
public class FeatureRepo : Repository<Feature>, IFeatureRepo
{
    private readonly Dictionary<int, List<int>> _tileIdIndex = new();

    public Feature Add(FeatureType type, Transform3D transform, int tileId, bool preview) =>
        Add(id => new Feature(type, transform, tileId, preview, id));

    protected override void AddHook(Feature entity)
    {
        if (_tileIdIndex.TryGetValue(entity.TileId, out var ids))
            ids.Add(entity.Id);
        else
            _tileIdIndex.Add(entity.TileId, [entity.Id]);
    }

    protected override void DeleteHook(Feature entity)
    {
        if (!_tileIdIndex.TryGetValue(entity.TileId, out var ids))
            return;
        ids.Remove(entity.Id);
        if (ids.Count == 0)
            _tileIdIndex.Remove(entity.TileId);
    }

    protected override void TruncateHook() => _tileIdIndex.Clear();

    public IEnumerable<Feature> GetByTileId(int tileId) =>
        _tileIdIndex.TryGetValue(tileId, out var ids) ? ids.Select(id => GetById(id)!) : [];

    public void DeleteByTileId(int tileId)
    {
        if (!_tileIdIndex.Remove(tileId, out var ids))
            return;
        ids.ForEach(Delete);
    }
}