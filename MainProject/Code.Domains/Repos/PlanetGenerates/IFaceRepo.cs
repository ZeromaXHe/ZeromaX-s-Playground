using Domains.Bases;
using Domains.Models.Entities.PlanetGenerates;
using Godot;

namespace Domains.Repos.PlanetGenerates;

public interface IFaceRepo : IRepository<Face>
{
    Face Add(bool chunky, Vector3[] triVertices);
    IEnumerable<Face> GetAllByChunky(bool chunky);
    List<Face> GetOrderedFaces(Point center);
}