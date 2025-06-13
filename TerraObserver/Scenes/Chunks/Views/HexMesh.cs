using System.Linq;
using Godot;
using Godot.Abstractions.Extensions.Chunks;
using Godot.Abstractions.Extensions.Planets;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:33:04
[Tool]
public partial class HexMesh : MeshInstance3D, IHexMesh
{
    #region 事件和 Export 属性

    [Export] public bool UseCollider { get; set; }
    [Export] public bool UseCellData { get; set; }
    [Export] public bool UseUvCoordinates { get; set; }
    [Export] public bool UseUv2Coordinates { get; set; }
    [Export] public bool Smooth { get; set; }

    #endregion

    #region 内部属性、变量

    private SurfaceTool _surfaceTool = new();
    private int _vIdx;

    #endregion

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