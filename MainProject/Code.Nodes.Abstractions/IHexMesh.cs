using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:34:11
public interface IHexMesh : IMeshInstance3D
{
    void Clear();
    void Apply();
    void ShowMesh(Mesh mesh);

    void AddTriangle(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default);

    void AddTriangleUnperturbed(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default);

    void AddQuad(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default);

    void AddQuadUnperturbed(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default);
}