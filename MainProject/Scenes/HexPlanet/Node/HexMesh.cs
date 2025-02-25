using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexMesh : MeshInstance3D, IHexMesh
{
    [Export] public bool UseCollider {get; set;}
    [Export] public bool UseColor {get; set;}
    [Export] public bool UseUvCoordinates {get; set;}
    [Export] public bool UseUv2Coordinates {get; set;}
    public SurfaceTool SurfaceTool { get; set; } = new();
    public int VIdx { get; set; }

    public void Clear()
    {
        // 清理之前的碰撞体
        foreach (var child in GetChildren())
            child.QueueFree();
        VIdx = 0;
        SurfaceTool.Clear();
        SurfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        SurfaceTool.SetSmoothGroup(uint.MaxValue);
    }

    public void Apply()
    {
        SurfaceTool.GenerateNormals();
        Mesh = SurfaceTool.Commit();
        // 仅在游戏中生成碰撞体
        if (!Engine.IsEditorHint() && UseCollider)
            CreateTrimeshCollision();
    }

    public void AddTriangle(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null) =>
        AddTriangleUnperturbed(vs.Select(HexMetrics.Perturb).ToArray(), cs, uvs, uvs2);

    public void AddTriangleUnperturbed(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null)
    {
        for (var i = 0; i < 3; i++)
        {
            if (UseColor && cs != null)
                SurfaceTool.SetColor(cs[i]);
            if (UseUvCoordinates && uvs != null)
                SurfaceTool.SetUV(uvs[i]);
            if (UseUv2Coordinates && uvs2 != null)
                SurfaceTool.SetUV2(uvs2[i]);
            SurfaceTool.AddVertex(vs[i]);
        }

        SurfaceTool.AddIndex(VIdx);
        SurfaceTool.AddIndex(VIdx + 1);
        SurfaceTool.AddIndex(VIdx + 2);
        VIdx += 3;
    }

    public void AddQuad(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null) =>
        AddQuadUnperturbed(vs.Select(HexMetrics.Perturb).ToArray(), cs, uvs, uvs2);

    public void AddQuadUnperturbed(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null)
    {
        for (var i = 0; i < 4; i++)
        {
            if (UseColor && cs != null)
                SurfaceTool.SetColor(cs[i]);
            if (UseUvCoordinates && uvs != null)
                SurfaceTool.SetUV(uvs[i]);
            if (UseUvCoordinates && uvs2 != null)
                SurfaceTool.SetUV2(uvs2[i]);
            SurfaceTool.AddVertex(vs[i]);
        }

        SurfaceTool.AddIndex(VIdx);
        SurfaceTool.AddIndex(VIdx + 2);
        SurfaceTool.AddIndex(VIdx + 1);
        SurfaceTool.AddIndex(VIdx + 1);
        SurfaceTool.AddIndex(VIdx + 2);
        SurfaceTool.AddIndex(VIdx + 3);
        VIdx += 4;
    }
}