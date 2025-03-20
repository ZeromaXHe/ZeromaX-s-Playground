using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constants;

namespace ZeromaXsPlaygroundProject.Demo.HexPlanet;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-12 21:07
[Tool]
public partial class IcosahedronVisual : Node3D
{
    private List<Vector3> _vertices = IcosahedronConstants.Vertices;
    private List<int> _indices = IcosahedronConstants.Indices;

    public override void _Ready()
    {
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        for (var i = 0; i < _vertices.Count; i++)
        {
            var v = _vertices[i];
            surfaceTool.AddVertex(v);
            var textMeshIns = new MeshInstance3D();
            var textMaterial = new StandardMaterial3D();
            textMaterial.AlbedoColor =
                i switch
                {
                    0 => Colors.Red,
                    1 => Colors.Green,
                    2 => Colors.Blue,
                    3 => Colors.Yellow,
                    4 => Colors.Purple,
                    5 => Colors.Cyan,
                    6 => Colors.DarkGreen,
                    7 => Colors.Brown,
                    8 => Colors.Pink,
                    9 => Colors.DarkGray,
                    10 => Colors.YellowGreen,
                    _ => Colors.Magenta,
                };
            var mesh = new TextMesh();
            mesh.Text = i.ToString();
            mesh.Material = textMaterial;
            textMeshIns.Mesh = mesh;
            textMeshIns.Position = v * 1.1f;
            AddChild(textMeshIns);

            if (Mathf.Abs(v.X) > 0.001f || Mathf.Abs(v.Z) > 0.001f)
                textMeshIns.LookAt(-textMeshIns.Position, Vector3.Up);
            else
                textMeshIns.LookAt(-textMeshIns.Position, Vector3.Forward * float.Sign(v.Y));
        }

        foreach (var i in _indices)
            surfaceTool.AddIndex(i);
        var material = new StandardMaterial3D();
        material.AlbedoColor = Colors.White;
        surfaceTool.SetMaterial(material);
        surfaceTool.GenerateNormals();
        var meshIns = new MeshInstance3D();
        meshIns.Mesh = surfaceTool.Commit();
        AddChild(meshIns);
    }
}