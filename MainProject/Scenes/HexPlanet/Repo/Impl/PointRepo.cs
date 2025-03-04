using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo.Impl;

public class PointRepo : Repository<Point>, IPointRepo
{
    private readonly Dictionary<Vector3, int> _positionIndex = new();
    private readonly Dictionary<SphereAxial, int> _coordsIndex = new();

    public Point Add(Vector3 position, SphereAxial coords) =>
        Add(id => new Point(position, coords, id));

    protected override void AddHook(Point entity)
    {
        _positionIndex.Add(entity.Position, entity.Id);
        _coordsIndex.Add(entity.Coords, entity.Id);
    }

    protected override void TruncateHook()
    {
        _positionIndex.Clear();
        _coordsIndex.Clear();
    }

    public Point GetByCoords(SphereAxial coords) =>
        _coordsIndex.TryGetValue(coords, out var id) ? GetById(id) : null;

    public int? GetIdByCoords(SphereAxial coords) =>
        _coordsIndex.TryGetValue(coords, out var id) ? id : null;

    public Point GetByPosition(Vector3 position) =>
        _positionIndex.TryGetValue(position, out var id) ? GetById(id) : null;

    public int? GetIdByPosition(Vector3 position) => _positionIndex.TryGetValue(position, out var id) ? id : null;
}