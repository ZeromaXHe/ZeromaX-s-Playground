using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 20:42:16
public interface IMiniMapManager : INode2D
{
    delegate void ClickedEvent(Vector3 posDirection);

    event ClickedEvent? Clicked;

    #region on-ready 节点

    TileMapLayer? TerrainLayer { get; }
    TileMapLayer? ColorLayer { get; }
    Camera2D? Camera { get; }
    Sprite2D? CameraIcon { get; }

    #endregion

    void Init(Vector3 orbitCamPos);
    void ClickOnMiniMap();
}