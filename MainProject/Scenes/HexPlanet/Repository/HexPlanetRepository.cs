using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repository;

public class HexPlanetRepository
{
    private readonly Dictionary<int, Point> _points = new();
    private readonly Dictionary<int, List<int>> _pointIdToFaceId = new();
    private readonly Dictionary<int, List<int>> _faceIdToPointId = new();
    private readonly Dictionary<int, Face> _faces = new();
    private readonly Dictionary<int, Tile> _tiles = new();
    private readonly Dictionary<int, int> _tileCenterIdIndex = new();

    public Point AddPoint(Vector3 position)
    {
        var point = new Point(position, _points.Count);
        _points.Add(point.Id, point);
        return point;
    }

    public Point GetPointById(int id) => _points.GetValueOrDefault(id);
    public IEnumerable<Point> GetAllPoints() => _points.Values;
    public List<int> GetFacePointIdsById(int id) => _faceIdToPointId.GetValueOrDefault(id);
    public IEnumerable<Point> GetFacePointsById(int id) => GetFacePointIdsById(id).Select(GetPointById);

    public Face AddFace(Vector3 center, List<int> pointIds)
    {
        var face = new Face(center, _faces.Count);
        _faces.Add(face.Id, face);
        _faceIdToPointId[face.Id] = pointIds;
        foreach (var pId in pointIds)
            if (_pointIdToFaceId.TryGetValue(pId, out var value))
                value.Add(face.Id);
            else _pointIdToFaceId.Add(pId, [face.Id]);
        return face;
    }

    public Face GetFaceById(int id) => _faces.GetValueOrDefault(id);

    public IEnumerable<Face> GetPointFacesByPointId(int id) =>
        _pointIdToFaceId.GetValueOrDefault(id).Select(GetFaceById);

    public Tile AddTile(int centerId, List<int> hexFaceIds, List<int> neighborCenterIds, float height = 0f)
    {
        var tile = new Tile(centerId, hexFaceIds, neighborCenterIds, height, _tiles.Count);
        _tiles.Add(tile.Id, tile);
        _tileCenterIdIndex.Add(centerId, tile.Id);
        return tile;
    }

    public Tile GetTileByCenterId(int centerId) =>
        _tileCenterIdIndex.TryGetValue(centerId, out var tileId)
            ? _tiles.GetValueOrDefault(tileId)
            : null;

    public IEnumerable<Tile> GetAllTiles() => _tiles.Values;
}