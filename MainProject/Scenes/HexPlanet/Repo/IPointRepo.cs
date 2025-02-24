using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

public interface IPointRepo : IRepository<Point>
{
    Point Add(Vector3 position);
    Point GetByPosition(Vector3 position);
    int? GetIdByPosition(Vector3 position);
}