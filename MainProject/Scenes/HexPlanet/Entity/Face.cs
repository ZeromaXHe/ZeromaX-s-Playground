using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Face(Vector3 center, int id = -1) : AEntity(id)
{
    public Vector3 Center { get; } = center; // 三角形重心 median point
    public Vector3[] TriVertices; // 第一个顶点是非水平边的顶点
    public bool IsAdjacentTo(Face face) => TriVertices.Intersect(face.TriVertices).Count() == 2;
}