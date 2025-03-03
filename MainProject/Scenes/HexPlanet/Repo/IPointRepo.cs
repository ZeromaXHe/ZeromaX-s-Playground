using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

public interface IPointRepo : IRepository<Point>
{
    Point Add(Vector3 position, TileType type, int typeIdx);
    Point GetByPosition(Vector3 position);
    int? GetIdByPosition(Vector3 position);
}