using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Demo.HexPlanet;

[Tool]
public partial class IcosahedronVisual : Node3D
{
    private static readonly float Sqrt5 = Mathf.Sqrt(5f); // √5
    private static readonly float Sqrt5divBy1 = 1f / Sqrt5; // 1/√5

    private List<Vector3> _vertices =
    [
        new Vector3(0f, 1f, 0f),
        new Vector3(2f * Sqrt5divBy1, Sqrt5divBy1, 0f),
        new Vector3((5f - Sqrt5) / 10f, Sqrt5divBy1, Mathf.Sqrt((5f + Sqrt5) / 10f)),
        new Vector3((-5f - Sqrt5) / 10f, Sqrt5divBy1, Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((-5f - Sqrt5) / 10f, Sqrt5divBy1, -Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((5f - Sqrt5) / 10f, Sqrt5divBy1, -Mathf.Sqrt((5f + Sqrt5) / 10f)),
        new Vector3(0f, -1f, 0f),
        new Vector3(-2f * Sqrt5divBy1, -Sqrt5divBy1, 0f),
        new Vector3((-5f + Sqrt5) / 10f, -Sqrt5divBy1, -Mathf.Sqrt((5f + Sqrt5) / 10f)),
        new Vector3((5f + Sqrt5) / 10f, -Sqrt5divBy1, -Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((5f + Sqrt5) / 10f, -Sqrt5divBy1, Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((-5f + Sqrt5) / 10f, -Sqrt5divBy1, Mathf.Sqrt((5f + Sqrt5) / 10f)),
    ];

    private List<int> _indices =
    [
        0, 1, 2,
        0, 2, 3,
        0, 3, 4,
        0, 4, 5,
        0, 5, 1,
        1, 9, 10,
        1, 10, 2,
        2, 10, 11,
        2, 11, 3,
        3, 11, 7,
        3, 7, 4,
        4, 7, 8,
        4, 8, 5,
        5, 8, 9,
        5, 9, 1,
        6, 8, 7,
        6, 9, 8,
        6, 10, 9,
        6, 11, 10,
        6, 7, 11,
    ];

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
            
            if (Mathf.Abs(v.X) > 0.0001f || Mathf.Abs(v.Z) > 0.0001f)
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