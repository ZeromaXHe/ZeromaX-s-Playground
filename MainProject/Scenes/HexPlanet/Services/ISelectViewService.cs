using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:57
public interface ISelectViewService
{
    void ClearPath();
    int SelectViewSize { get; set; }
    Mesh GenerateMeshForEditMode(int editingTileId, Vector3 position);
    Mesh GenerateMeshForPlayMode(int pathFindingFromTileId, Vector3 position);
}