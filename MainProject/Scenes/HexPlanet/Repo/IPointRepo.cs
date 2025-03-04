using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

public interface IPointRepo : IRepository<Point>
{
    Point Add(Vector3 position, SphereAxial coords);
    Point GetByCoords(SphereAxial coords);
    int? GetIdByCoords(SphereAxial coords);
    Point GetByPosition(Vector3 position);
    int? GetIdByPosition(Vector3 position);
}