using Commons.Utils.HexSphereGrid;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Repos.PlanetGenerates;
using Godot;
using Infras.Base;

namespace Infras.Repos.Impl.PlanetGenerates;

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

    protected override void DeleteHook(Point entity)
    {
        if (entity.Chunky)
        {
            _chunkPositionIndex.Remove(entity.Position);
            _chunkCoordsIndex.Remove(entity.Coords);
        }
        else
        {
            _tilePositionIndex.Remove(entity.Position);
            _tileCoordsIndex.Remove(entity.Coords);
        }
    }

    protected override void TruncateHook()
    {
        _tilePositionIndex.Clear();
        _tileCoordsIndex.Clear();
        _chunkPositionIndex.Clear();
        _chunkCoordsIndex.Clear();
    }

    public Point? GetByCoords(bool chunky, SphereAxial coords) =>
        (chunky ? _chunkCoordsIndex : _tileCoordsIndex).TryGetValue(coords, out var id)
            ? GetById(id)
            : null;

    public int? GetIdByCoords(bool chunky, SphereAxial coords) =>
        (chunky ? _chunkCoordsIndex : _tileCoordsIndex).TryGetValue(coords, out var id)
            ? id
            : null;

    public Point? GetByPosition(bool chunky, Vector3 position) =>
        (chunky ? _chunkPositionIndex : _tilePositionIndex).TryGetValue(position, out var id)
            ? GetById(id)
            : null;

    public int? GetIdByPosition(bool chunky, Vector3 position) =>
        (chunky ? _chunkPositionIndex : _tilePositionIndex).TryGetValue(position, out var id)
            ? id
            : null;

    public List<Point> GetNeighborCenterIds(List<Face> hexFaces, Point center)
    {
        return (
            from face in hexFaces
            select GetRightOtherPoints(face, center)
        ).ToList();
    }

    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    private IEnumerable<Point> GetOtherPoints(Face face, Point point)
    {
        // 注意：并没有对 face 和 point 的 Chunky 进行校验
        var idx = GetPointIdx(face, point);
        yield return GetByPosition(face.Chunky, face.TriVertices[(idx + 1) % 3])!;
        yield return GetByPosition(face.Chunky, face.TriVertices[(idx + 2) % 3])!;
    }

    // 顺时针第一个顶点
    private Point GetLeftOtherPoints(Face face, Point point)
    {
        var idx = GetPointIdx(face, point);
        return GetByPosition(face.Chunky, face.TriVertices[(idx + 1) % 3])!;
    }

    // 顺时针第二个顶点
    private Point GetRightOtherPoints(Face face, Point point)
    {
        var idx = GetPointIdx(face, point);
        return GetByPosition(face.Chunky, face.TriVertices[(idx + 2) % 3])!;
    }

    private static int GetPointIdx(Face face, Point point)
    {
        if (face.TriVertices.All(facePointId => facePointId != point.Position))
            throw new ArgumentException("Given point must be one of the points on the face!");

        for (var i = 0; i < 3; i++)
        {
            if (face.TriVertices[i] == point.Position)
                return i;
        }

        return -1;
    }

    public SphereAxial GetSphereAxial(Tile tile) => GetById(tile.CenterId)!.Coords;
}