using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Point(Vector3 position, int id = -1): AEntity(id)
{
    public Vector3 Position { get; } = position;
    public List<int> FaceIds;

    private const float PointComparisonAccuracy = 0.0001f;

    public bool IsOverlapping(Vector3 v) =>
        Mathf.Abs(Position.X - v.X) <= PointComparisonAccuracy &&
        Mathf.Abs(Position.Y - v.Y) <= PointComparisonAccuracy &&
        Mathf.Abs(Position.Z - v.Z) <= PointComparisonAccuracy;
}