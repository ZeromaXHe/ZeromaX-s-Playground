using Commons.Utils.HexSphereGrid;
using Domains.Models.Entities.PlanetGenerates;
using Godot;
using Infras.Writers.Abstractions.Bases;

namespace Infras.Writers.Abstractions.PlanetGenerates;

public interface IPointRepo : IRepository<Point>
{
    Point Add(bool chunky, Vector3 position, SphereAxial coords);
    Point? GetByCoords(bool chunky, SphereAxial coords);
    int? GetIdByCoords(bool chunky, SphereAxial coords);
    Point? GetByPosition(bool chunky, Vector3 position);
    int? GetIdByPosition(bool chunky, Vector3 position);
    IEnumerable<Point> GetAllByChunky(bool chunky);
    List<Point> GetNeighborCenterIds(List<Face> hexFaces, Point center);
    SphereAxial GetSphereAxial(Tile tile);
}