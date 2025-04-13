using Godot;

namespace Domains.Services.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:57
public interface ISelectViewService
{
    void ClearPath();
    Mesh? GenerateMeshForEditMode(int editingTileId, Vector3 position);
    Mesh? GenerateMeshForPlayMode(int pathFindingFromTileId, Vector3 position);
}