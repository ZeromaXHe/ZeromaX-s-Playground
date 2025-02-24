using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

public interface IFaceRepo : IRepository<Face>
{
    Face Add(Point p0, Point p1, Point p2);
}