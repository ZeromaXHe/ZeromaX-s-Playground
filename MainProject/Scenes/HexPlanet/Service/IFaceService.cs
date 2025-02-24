using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IFaceService
{
    void ClearData();
    Face Add(Point p0, Point p1, Point p2);

    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    IEnumerable<Point> GetOtherPoints(Face face, Point point);
}