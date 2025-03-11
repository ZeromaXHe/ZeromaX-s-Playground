using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IPointService
{
    void Truncate();
    void InitPointsAndFaces(bool chunky, int divisions);
    public IEnumerable<Point> GetAll(bool chunky);
}