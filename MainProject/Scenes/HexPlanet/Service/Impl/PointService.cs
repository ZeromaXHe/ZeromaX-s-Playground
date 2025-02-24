using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constant;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class PointService(IFaceService faceService, IPointRepo pointRepo): IPointService
{
    private readonly HashSet<int> _framePointIds = [];

    public void ClearData()
    {
        pointRepo.Truncate();
        _framePointIds.Clear();
    }
    
    public void SubdivideIcosahedron(int divisions)
    {
        var time = Time.GetTicksMsec();
        var points = IcosahedronConstants.Vertices
            .Select(v =>
            {
                var p = pointRepo.Add(v);
                _framePointIds.Add(p.Id);
                return p;
            })
            .ToList();
        var indices = IcosahedronConstants.Indices;
        for (var idx = 0; idx < indices.Count; idx += 3)
        {
            var p0 = points[indices[idx]];
            var p1 = points[indices[idx + 1]];
            var p2 = points[indices[idx + 2]];
            var bottomSide = new List<Point> { p0 };
            var leftSide = Subdivide(p0, p1, divisions, true);
            var rightSide = Subdivide(p0, p2, divisions, true);
            for (var i = 1; i <= divisions; i++)
            {
                var previousPoints = bottomSide;
                bottomSide = Subdivide(leftSide[i], rightSide[i], i, i == divisions);
                for (var j = 0; j < i; j++)
                {
                    faceService.Add(previousPoints[j], bottomSide[j], bottomSide[j + 1]);
                    if (j == 0) continue;
                    faceService.Add(previousPoints[j - 1], previousPoints[j], bottomSide[j]);
                }
            }
        }
        GD.Print($"SubdivideIcosahedron cost: {Time.GetTicksMsec() - time} ms");

        return;

        List<Point> Subdivide(Point from, Point target, int count, bool checkFrameExist)
        {
            var segments = new List<Point> { from };

            for (var i = 1; i < count; i++)
            {
                // 注意这里用 Slerp 而不是 Lerp，让所有的 Point 都在单位球面而不是单位正二十面体上，方便我们后面 VP 树找最近点
                var v = from.Position.Slerp(target.Position, (float)i / count);
                Point newPoint = null;
                if (checkFrameExist)
                {
                    var existingPoint = _framePointIds.Select(pointRepo.GetById)
                        .FirstOrDefault(candidatePoint => candidatePoint.IsOverlapping(v));
                    if (existingPoint != null)
                        newPoint = existingPoint;
                }

                if (newPoint == null)
                {
                    newPoint = pointRepo.Add(v);
                    if (checkFrameExist)
                        _framePointIds.Add(newPoint.Id);
                }

                segments.Add(newPoint);
            }

            segments.Add(target);
            return segments;
        }
    }
}