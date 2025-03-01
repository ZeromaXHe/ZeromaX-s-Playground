using System;
using System.Collections.Generic;
using System.Linq;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class FaceService(IFaceRepo faceRepo, IPointRepo pointRepo) : IFaceService
{
    public void Truncate() => faceRepo.Truncate();

    public Face Add(Point p0, Point p1, Point p2)
    {
        var face = faceRepo.Add(p0, p1, p2);
        foreach (var p in face.PointIds.Select(pointRepo.GetById))
        {
            if (p.FaceIds == null)
                p.FaceIds = [face.Id];
            else p.FaceIds.Add(face.Id);
        }

        return face;
    }

    public IEnumerable<Point> GetOtherPoints(Face face, Point point)
    {
        var idx = GetPointIdx(face, point);
        yield return pointRepo.GetById(face.PointIds[(idx + 1) % 3]);
        yield return pointRepo.GetById(face.PointIds[(idx + 2) % 3]);
    }

    // 顺时针第一个顶点
    public Point GetLeftOtherPoints(Face face, Point point)
    {
        var idx = GetPointIdx(face, point);
        return pointRepo.GetById(face.PointIds[(idx + 1) % 3]);
    }

    // 顺时针第二个顶点
    public Point GetRightOtherPoints(Face face, Point point)
    {
        var idx = GetPointIdx(face, point);
        return pointRepo.GetById(face.PointIds[(idx + 2) % 3]);
    }

    private static int GetPointIdx(Face face, Point point)
    {
        if (face.PointIds.All(facePointId => facePointId != point.Id))
            throw new ArgumentException("Given point must be one of the points on the face!");

        return face.PointIds.FindIndex(pId => pId == point.Id);
    }
}