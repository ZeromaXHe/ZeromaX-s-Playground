using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class FaceService(IFaceRepo faceRepo, IPointRepo pointRepo) : IFaceService
{
    public void Truncate() => faceRepo.Truncate();
    public Face Add(Vector3[] triVertices) => faceRepo.Add(triVertices);
    public IEnumerable<Face> GetAll() => faceRepo.GetAll();

    public IEnumerable<Point> GetOtherPoints(Face face, Point point)
    {
        var idx = GetPointIdx(face, point);
        yield return pointRepo.GetByPosition(face.TriVertices[(idx + 1) % 3]);
        yield return pointRepo.GetByPosition(face.TriVertices[(idx + 2) % 3]);
    }

    // 顺时针第一个顶点
    public Point GetLeftOtherPoints(Face face, Point point)
    {
        var idx = GetPointIdx(face, point);
        return pointRepo.GetByPosition(face.TriVertices[(idx + 1) % 3]);
    }

    // 顺时针第二个顶点
    public Point GetRightOtherPoints(Face face, Point point)
    {
        var idx = GetPointIdx(face, point);
        return pointRepo.GetByPosition(face.TriVertices[(idx + 2) % 3]);
    }

    private static int GetPointIdx(Face face, Point point)
    {
        if (face.TriVertices.All(facePointId => facePointId != point.Position))
            throw new ArgumentException("Given point must be one of the points on the face!");

        for (var i = 0; i < 3; i++)
        {
            if (face.TriVertices[i] == point.Position)
                return i;
        }

        return -1;
    }
}