using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Point(Vector3 position, int id = -1)
{
    public Vector3 Position { get; } = position;
    public int Id { get; } = id;
    public List<int> FaceIds;

    private const float PointComparisonAccuracy = 0.0001f;

    public bool IsOverlapping(Vector3 v) =>
        Mathf.Abs(Position.X - v.X) <= PointComparisonAccuracy &&
        Mathf.Abs(Position.Y - v.Y) <= PointComparisonAccuracy &&
        Mathf.Abs(Position.Z - v.Z) <= PointComparisonAccuracy;

    #region 数据查询

    private static readonly Dictionary<int, Point> Repo = new();

    public static void Truncate() => Repo.Clear();

    public static Point Add(Vector3 position)
    {
        var point = new Point(position, Repo.Count);
        Repo.Add(point.Id, point);
        return point;
    }

    public static Point GetById(int id) => Repo.GetValueOrDefault(id);
    public static IEnumerable<Point> GetAll() => Repo.Values;

    #endregion
}