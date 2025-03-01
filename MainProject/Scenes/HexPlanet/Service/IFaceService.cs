using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IFaceService
{
    void Truncate();
    Face Add(Point p0, Point p1, Point p2);

    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    IEnumerable<Point> GetOtherPoints(Face face, Point point);

    // 顺时针第一个顶点
    Point GetLeftOtherPoints(Face face, Point point);

    // 顺时针第二个顶点
    Point GetRightOtherPoints(Face face, Point point);
}