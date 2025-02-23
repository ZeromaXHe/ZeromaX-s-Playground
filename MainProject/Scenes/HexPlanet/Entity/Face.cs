using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Face(Vector3 center, int id = -1)
{
    public Vector3 Center { get; } = center; // 三角形重心 median point
    public int Id { get; } = id;
    public List<int> PointIds;
    
    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    public IEnumerable<Point> GetOtherPoints(Point point)
    {
        if (PointIds.All(facePointId => facePointId != point.Id))
            throw new ArgumentException("Given point must be one of the points on the face!");

        var idx = PointIds.FindIndex(pId => pId == point.Id);
        yield return Point.GetById(PointIds[(idx + 1) % 3]);
        yield return Point.GetById(PointIds[(idx + 2) % 3]);
    }
    
    public bool IsAdjacentTo(Face face) => PointIds.Intersect(face.PointIds).Count() == 2;
    
    #region 数据查询

    private static readonly Dictionary<int, Face> Repo = new();

    public static void Truncate() => Repo.Clear();
    
    public static Face Add(Point p0, Point p1, Point p2)
    {
        var center = (p0.Position + p1.Position + p2.Position) / 3f;
        List<int> pointIds = Math3dUtil.IsRightVSeq(Vector3.Zero, p0.Position, p1.Position, p2.Position)
            ? [p0.Id, p1.Id, p2.Id]
            : [p0.Id, p2.Id, p1.Id];
        
        var face = new Face(center, Repo.Count);
        Repo.Add(face.Id, face);
        face.PointIds = pointIds;
        foreach (var p in pointIds.Select(Point.GetById))
        {
            if (p.FaceIds == null)
                p.FaceIds = [face.Id];
            else p.FaceIds.Add(face.Id);
        }

        return face;
    }

    public static Face GetById(int id) => Repo.GetValueOrDefault(id);

    #endregion
}