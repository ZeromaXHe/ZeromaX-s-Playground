using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;

public interface IPointRepo : IRepository<Point>
{
    Point Add(bool chunky, Vector3 position, SphereAxial coords);
    Point GetByCoords(bool chunky, SphereAxial coords);
    int? GetIdByCoords(bool chunky, SphereAxial coords);
    Point GetByPosition(bool chunky, Vector3 position);
    int? GetIdByPosition(bool chunky, Vector3 position);
    IEnumerable<Point> GetAllByChunky(bool chunky);
    List<Point> GetNeighborCenterIds(List<Face> hexFaces, Point center);
    SphereAxial GetSphereAxial(Tile tile);
}