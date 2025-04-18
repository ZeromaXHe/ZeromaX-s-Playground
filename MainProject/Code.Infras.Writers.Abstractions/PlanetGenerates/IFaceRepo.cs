using Domains.Models.Entities.PlanetGenerates;
using Godot;
using Infras.Writers.Abstractions.Bases;

namespace Infras.Writers.Abstractions.PlanetGenerates;

public interface IFaceRepo : IRepository<Face>
{
    Face Add(bool chunky, Vector3[] triVertices);
    IEnumerable<Face> GetAllByChunky(bool chunky);
    List<Face> GetOrderedFaces(Point center);
    Vector3 GetCornerByFaceId(Tile tile, int id, float radius = 1f, float size = 1f);
}