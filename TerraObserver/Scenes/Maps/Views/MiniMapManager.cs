using Godot;
using TO.Domains.Types.Maps;

namespace TerraObserver.Scenes.Maps.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-03 14:42:05
public partial class MiniMapManager : Node2D, IMiniMapManager
{
    #region 事件

    public delegate void ClickedEvent(Vector3 posDirection);

    public event ClickedEvent? Clicked;
    public void EmitClicked(Vector3 posDirection) => Clicked?.Invoke(posDirection);

    #endregion

    #region on-ready

    public TileMapLayer? TerrainLayer { get; private set; }
    public TileMapLayer? ColorLayer { get; private set; }
    public Camera2D? Camera { get; private set; }
    public Sprite2D? CameraIcon { get; private set; }

    #endregion

    #region 生命周期

    public override void _Ready()
    {
        TerrainLayer = GetNode<TileMapLayer>("%TerrainLayer");
        ColorLayer = GetNode<TileMapLayer>("%ColorLayer");
        Camera = GetNode<Camera2D>("%Camera2D");
        CameraIcon = GetNode<Sprite2D>("%CameraIcon");
    }

    #endregion
}