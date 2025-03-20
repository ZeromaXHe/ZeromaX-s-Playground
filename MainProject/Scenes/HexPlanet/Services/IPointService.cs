using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
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