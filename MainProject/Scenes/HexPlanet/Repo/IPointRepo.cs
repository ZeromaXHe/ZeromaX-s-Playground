using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

public interface IPointRepo : IRepository<Point>
{
    Point Add(bool chunky, Vector3 position, SphereAxial coords);
    Point GetByCoords(bool chunky, SphereAxial coords);
    int? GetIdByCoords(bool chunky, SphereAxial coords);
    Point GetByPosition(bool chunky, Vector3 position);
    int? GetIdByPosition(bool chunky, Vector3 position);
    IEnumerable<Point> GetAllByChunky(bool chunky);
}