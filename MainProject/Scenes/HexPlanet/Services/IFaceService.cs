using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public interface IFaceService
{
    #region 透传存储库方法

    void Truncate();
    Face GetById(int id);
    IEnumerable<Face> GetAllByChunky(bool chunky);

    #endregion

    Face Add(bool chunky, Vector3[] triVertices);

    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    IEnumerable<Point> GetOtherPoints(Face face, Point point);

    // 顺时针第一个顶点
    Point GetLeftOtherPoints(Face face, Point point);

    // 顺时针第二个顶点
    Point GetRightOtherPoints(Face face, Point point);
}