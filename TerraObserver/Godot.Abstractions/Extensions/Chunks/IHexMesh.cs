using Godot.Abstractions.Bases;

namespace Godot.Abstractions.Extensions.Chunks;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 15:22:08
public interface IHexMesh : IMeshInstance3D
{
    void Clear();
    void Apply();
    void ShowMesh(Mesh mesh);
    void AddTriangle(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default);
    void AddQuad(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default);
}