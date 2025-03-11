using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IPointService
{
    #region 透传存储库方法

    void Truncate();
    Point GetById(int id);
    int? GetIdByPosition(bool chunky, Vector3 pos);
    IEnumerable<Point> GetAllByChunky(bool chunky);

    #endregion

    void InitPointsAndFaces(bool chunky, int divisions);
    List<Face> GetOrderedFaces(Point center);
    List<Point> GetNeighborCenterIds(List<Face> hexFaces, Point center);
}