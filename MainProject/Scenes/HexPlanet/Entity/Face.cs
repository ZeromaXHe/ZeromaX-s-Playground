using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Face(Vector3 center, int id = -1) : AEntity(id)
{
    public Vector3 Center { get; } = center; // 三角形重心 median point
    public List<int> PointIds;
    public bool IsAdjacentTo(Face face) => PointIds.Intersect(face.PointIds).Count() == 2;
}