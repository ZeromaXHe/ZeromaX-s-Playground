using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Impl;

public class PointRepo : Repository<Point>, IPointRepo
{
    private readonly Dictionary<Vector3, int> _tilePositionIndex = new();
    private readonly Dictionary<SphereAxial, int> _tileCoordsIndex = new();
    private readonly Dictionary<Vector3, int> _chunkPositionIndex = new();
    private readonly Dictionary<SphereAxial, int> _chunkCoordsIndex = new();

    public Point Add(bool chunky, Vector3 position, SphereAxial coords) =>
        Add(id => new Point(chunky, position, coords, id));

    public IEnumerable<Point> GetAllByChunky(bool chunky) => GetAll().Where(x => x.Chunky == chunky);

    protected override void AddHook(Point entity)
    {
        if (entity.Chunky)
        {
            _chunkPositionIndex.Add(entity.Position, entity.Id);
            _chunkCoordsIndex.Add(entity.Coords, entity.Id);
        }
        else
        {
            _tilePositionIndex.Add(entity.Position, entity.Id);
            _tileCoordsIndex.Add(entity.Coords, entity.Id);
        }
    }

    protected override void TruncateHook()
    {
        _tilePositionIndex.Clear();
        _tileCoordsIndex.Clear();
        _chunkPositionIndex.Clear();
        _chunkCoordsIndex.Clear();
    }

    public Point GetByCoords(bool chunky, SphereAxial coords) =>
        (chunky ? _chunkCoordsIndex : _tileCoordsIndex).TryGetValue(coords, out var id)
            ? GetById(id)
            : null;

    public int? GetIdByCoords(bool chunky, SphereAxial coords) =>
        (chunky ? _chunkCoordsIndex : _tileCoordsIndex).TryGetValue(coords, out var id)
            ? id
            : null;

    public Point GetByPosition(bool chunky, Vector3 position) =>
        (chunky ? _chunkPositionIndex : _tilePositionIndex).TryGetValue(position, out var id)
            ? GetById(id)
            : null;

    public int? GetIdByPosition(bool chunky, Vector3 position) =>
        (chunky ? _chunkPositionIndex : _tilePositionIndex).TryGetValue(position, out var id)
            ? id
            : null;
}