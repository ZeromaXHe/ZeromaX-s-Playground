using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo.Impl;

public class PointRepo : Repository<Point>, IPointRepo
{
    private readonly Dictionary<Vector3, int> _positionIndex = new();
    public Point Add(Vector3 position) => Add(id => new Point(position, id));
    protected override void AddHook(Point entity) => _positionIndex.Add(entity.Position, entity.Id);
    protected override void TruncateHook() => _positionIndex.Clear();

    public Point GetByPosition(Vector3 position) =>
        _positionIndex.TryGetValue(position, out var id) ? GetById(id) : null;

    public int? GetIdByPosition(Vector3 position) => _positionIndex.TryGetValue(position, out var id) ? id : null;
}