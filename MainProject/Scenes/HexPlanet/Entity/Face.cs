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
    
    public IEnumerable<Point> GetOtherPoints(Point point)
    {
        if (PointIds.All(facePointId => facePointId != point.Id))
            throw new ArgumentException("Given point must be one of the points on the face!");

        return PointIds
            .Where(pId => pId != point.Id)
            .Select(Point.GetById);
    }
    
    public bool IsAdjacentTo(Face face) => PointIds.Intersect(face.PointIds).Count() == 2;
    
    #region 数据查询

    private static readonly Dictionary<int, Face> Repo = new();

    public static void Truncate() => Repo.Clear();
    
    public static Face Add(Point p0, Point p1, Point p2)
    {
        var center = (p0.Position + p1.Position + p2.Position) / 3f;
        // 决定缠绕顺序
        var normal = Math3dUtil.GetNormal(p0.Position, p1.Position, p2.Position);
        List<int> pointIds = Math3dUtil.IsNormalAwayFromOrigin(center, normal, Vector3.Zero)
            ? [p0.Id, p2.Id, p1.Id]
            : [p0.Id, p1.Id, p2.Id];
        
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