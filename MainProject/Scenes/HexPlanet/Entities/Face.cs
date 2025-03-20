using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-14 11:50
public class Face(bool chunky, Vector3 center, int id = -1) : AEntity(id)
{
    // 是，属于分块；否，属于地块
    public bool Chunky { get; } = chunky;
    public Vector3 Center { get; } = center; // 三角形重心 median point
    public Vector3[] TriVertices; // 第一个顶点是非水平边的顶点，后续水平边的两点按照顺时针方向排列
    public bool IsAdjacentTo(Face face) => TriVertices.Intersect(face.TriVertices).Count() == 2;
}