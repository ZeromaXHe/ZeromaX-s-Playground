using Apps.Queries.Applications.Uis;
using Apps.Queries.Contexts;
using Commons.Utils.HexSphereGrid;
using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-05 12:21
public partial class MiniMapManager : Node2D, IMiniMapManager
{
    public event IMiniMapManager.ClickedEvent? Clicked;

    public MiniMapManager()
    {
        NodeContext.Instance.RegisterSingleton<IMiniMapManager>(this);
        Context.RegisterToHolder<IMiniMapManager>(this);
    }

    public NodeEvent? NodeEvent => null;

    public override void _Ready()
    {
        InitOnReadyNodes();
        GD.Print("MiniMapManager _Ready");
    }

    public override void _ExitTree()
    {
        NodeContext.Instance.DestroySingleton<IMiniMapManager>();
    }

    #region on-ready 节点

    public TileMapLayer? TerrainLayer { get; private set; }
    public TileMapLayer? ColorLayer { get; private set; }
    public Camera2D? Camera { get; private set; }
    public Sprite2D? CameraIcon { get; private set; }

    private void InitOnReadyNodes()
    {
        TerrainLayer = GetNode<TileMapLayer>("%TerrainLayer");
        ColorLayer = GetNode<TileMapLayer>("%ColorLayer");
        Camera = GetNode<Camera2D>("%Camera2D");
        CameraIcon = GetNode<Sprite2D>("%CameraIcon");
    }

    #endregion

    public void ClickOnMiniMap()
    {
        var mousePos = TerrainLayer!.GetLocalMousePosition();
        var mapVec = TerrainLayer.LocalToMap(mousePos);
        var sa = new SphereAxial(mapVec.X, mapVec.Y);
        if (!sa.IsValid()) return;
        Clicked?.Invoke(sa.ToLongitudeAndLatitude().ToDirectionVector3());
    }
}