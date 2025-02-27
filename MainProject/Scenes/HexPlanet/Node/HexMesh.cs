using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexMesh : MeshInstance3D
{
    [Export] public bool UseCollider { get; set; }
    [Export] public bool UseColor { get; set; }
    [Export] public bool UseUvCoordinates { get; set; }
    [Export] public bool UseUv2Coordinates { get; set; }
    private SurfaceTool _surfaceTool = new();
    private int _vIdx;

    public void Clear()
    {
        // 清理之前的碰撞体
        foreach (var child in GetChildren())
            child.QueueFree();
        _vIdx = 0;
        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        _surfaceTool.SetSmoothGroup(uint.MaxValue);
    }

    public void Apply()
    {
        _surfaceTool.GenerateNormals();
        Mesh = _surfaceTool.Commit();
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
                _surfaceTool.SetColor(cs[i]);
            if (UseUvCoordinates && uvs != null)
                _surfaceTool.SetUV(uvs[i]);
            if (UseUv2Coordinates && uvs2 != null)
                _surfaceTool.SetUV2(uvs2[i]);
            _surfaceTool.AddVertex(vs[i]);
        }

        _surfaceTool.AddIndex(_vIdx);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 2);
        _vIdx += 3;
    }

    public void AddQuad(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null) =>
        AddQuadUnperturbed(vs.Select(HexMetrics.Perturb).ToArray(), cs, uvs, uvs2);

    public void AddQuadUnperturbed(Vector3[] vs, Color[] cs = null, Vector2[] uvs = null, Vector2[] uvs2 = null)
    {
        for (var i = 0; i < 4; i++)
        {
            if (UseColor && cs != null)
                _surfaceTool.SetColor(cs[i]);
            if (UseUvCoordinates && uvs != null)
                _surfaceTool.SetUV(uvs[i]);
            if (UseUvCoordinates && uvs2 != null)
                _surfaceTool.SetUV2(uvs2[i]);
            _surfaceTool.AddVertex(vs[i]);
        }

        _surfaceTool.AddIndex(_vIdx);
        _surfaceTool.AddIndex(_vIdx + 2);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 2);
        _surfaceTool.AddIndex(_vIdx + 3);
        _vIdx += 4;
    }
}