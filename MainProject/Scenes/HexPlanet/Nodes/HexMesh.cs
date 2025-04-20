using System.Linq;
using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-21 16:57
[Tool]
public partial class HexMesh : MeshInstance3D, IHexMesh
{
    public HexMesh() => InitServices();
    public NodeEvent? NodeEvent => null;
    [Export] public bool UseCollider { get; set; }
    [Export] public bool UseCellData { get; set; }
    [Export] public bool UseUvCoordinates { get; set; }
    [Export] public bool UseUv2Coordinates { get; set; }
    [Export] public bool Smooth { get; set; }

    #region 服务

    private static IHexPlanetManagerRepo? _hexPlanetManagerRepo;

    private static void InitServices()
    {
        _hexPlanetManagerRepo ??= Context.GetBeanFromHolder<IHexPlanetManagerRepo>();
    }

    #endregion

    private SurfaceTool _surfaceTool = new();
    private int _vIdx;

    public static readonly Color Weights1 = Colors.Red;
    public static readonly Color Weights2 = Colors.Green;
    public static readonly Color Weights3 = Colors.Blue;
    public static T[] TriArr<T>(T c) => [c, c, c];
    public static T[] QuadArr<T>(T c) => [c, c, c, c];
    public static T[] QuadArr<T>(T c1, T c2) => [c1, c1, c2, c2];

    public void Clear()
    {
        // 清理之前的碰撞体
        foreach (var child in GetChildren())
            child.QueueFree();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        if (!Smooth)
            _surfaceTool.SetSmoothGroup(uint.MaxValue);
        if (UseCellData)
            _surfaceTool.SetCustomFormat(0, SurfaceTool.CustomFormat.RgbFloat);
    }

    public void Apply()
    {
        _surfaceTool.GenerateNormals();
        Mesh = _surfaceTool.Commit();
        // 仅在游戏中生成碰撞体
        if (!Engine.IsEditorHint() && UseCollider)
            CreateTrimeshCollision();
        _surfaceTool.Clear(); // 释放 SurfaceTool 中的内存
        _vIdx = 0;
    }

    public void ShowMesh(Mesh mesh)
    {
        Mesh = mesh;
        if (!UseCollider) return;
        // 更新碰撞体网格
        StaticBody3D staticBody;
        CollisionShape3D collision;
        if (GetChildCount() == 0)
        {
            staticBody = new StaticBody3D();
            AddChild(staticBody);
            collision = new CollisionShape3D();
            staticBody.AddChild(collision);
        }
        else
        {
            staticBody = GetChild<StaticBody3D>(0);
            collision = staticBody.GetChild<CollisionShape3D>(0);
        }

        collision.Shape = mesh.CreateTrimeshShape();
    }

    /// <summary>
    /// 绘制三角形
    /// </summary>
    /// <param name="vs">顶点数组 vertices</param>
    /// <param name="tws">地块权重 tWeights</param>
    /// <param name="uvs">UV</param>
    /// <param name="uvs2">UV2</param>
    /// <param name="tis">地块ID tileIds</param>
    public void AddTriangle(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default) =>
        AddTriangleUnperturbed(vs.Select(_hexPlanetManagerRepo!.Perturb).ToArray(), tws, uvs, uvs2, tis);

    public void AddTriangleUnperturbed(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default)
    {
        for (var i = 0; i < 3; i++)
        {
            if (UseCellData && tws != null)
            {
                _surfaceTool.SetColor(tws[i]);
                _surfaceTool.SetCustom(0, new Color(tis.X, tis.Y, tis.Z));
            }

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

    public void AddQuad(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default) =>
        AddQuadUnperturbed(vs.Select(_hexPlanetManagerRepo!.Perturb).ToArray(), tws, uvs, uvs2, tis);

    public void AddQuadUnperturbed(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default)
    {
        for (var i = 0; i < 4; i++)
        {
            if (UseCellData && tws != null)
            {
                _surfaceTool.SetColor(tws[i]);
                _surfaceTool.SetCustom(0, new Color(tis.X, tis.Y, tis.Z));
            }

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