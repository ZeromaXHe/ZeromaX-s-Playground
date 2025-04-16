using Godot;
using GodotNodes.Abstractions;

namespace Apps.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 20:42:16
public interface IMiniMapManager : INode2D
{
    #region on-ready 节点

    TileMapLayer? TerrainLayer { get; }
    TileMapLayer? ColorLayer { get; }
    Camera2D? Camera { get; }
    Sprite2D? CameraIcon { get; }

    #endregion

    void Init(Vector3 orbitCamPos);
    void ClickOnMiniMap();
}