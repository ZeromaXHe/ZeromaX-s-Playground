using Apps.Applications.Uis;
using Apps.Contexts;
using Apps.Events;
using Apps.Nodes;
using Commons.Utils.HexSphereGrid;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-05 12:21
public partial class MiniMapManager : Node2D, IMiniMapManager
{
    public MiniMapManager()
    {
        InitApps();
        NodeContext.Instance.RegisterSingleton<IMiniMapManager>(this);
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

    #region 应用服务

    private IMiniMapManagerApp? _miniMapManagerApp;

    private void InitApps()
    {
        _miniMapManagerApp = Context.GetBeanFromHolder<IMiniMapManagerApp>();
    }

    #endregion

    public override void _Ready()
    {
        InitOnReadyNodes();
        _miniMapManagerApp!.OnReady();
        GD.Print("MiniMapManager _Ready");
    }

    public override void _ExitTree()
    {
        _miniMapManagerApp!.OnExitTree();
        NodeContext.Instance.DestroySingleton<IMiniMapManager>();
    }

    public void ClickOnMiniMap()
    {
        var mousePos = TerrainLayer!.GetLocalMousePosition();
        var mapVec = TerrainLayer.LocalToMap(mousePos);
        var sa = new SphereAxial(mapVec.X, mapVec.Y);
        if (!sa.IsValid()) return;
        OrbitCameraEvent.EmitNewDestination(sa.ToLongitudeAndLatitude().ToDirectionVector3());
    }

    public void Init(Vector3 orbitCamPos) => _miniMapManagerApp!.Init(orbitCamPos);
}