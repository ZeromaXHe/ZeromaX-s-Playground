using Apps.Queries.Contexts;
using Contexts;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Uis;
using Godot;
using GodotNodes.Abstractions.Addition;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Nodes.Abstractions.Planets;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-27 11:26:48
public partial class SelectTileViewer : MeshInstance3D, ISelectTileViewer
{
    public SelectTileViewer()
    {
        InitServices();
        NodeContext.Instance.RegisterSingleton<ISelectTileViewer>(this);
        Context.RegisterToHolder<ISelectTileViewer>(this);
    }

    #region 服务

    private ISelectViewService? _selectViewService;
    private IHexPlanetHudRepo? _hexPlanetHudRepo;

    private void InitServices()
    {
        _selectViewService = Context.GetBeanFromHolder<ISelectViewService>();
        _hexPlanetHudRepo = Context.GetBeanFromHolder<IHexPlanetHudRepo>();
    }

    #endregion

    public override void _ExitTree() => NodeContext.Instance.DestroySingleton<ISelectTileViewer>();
    public NodeEvent? NodeEvent => null;

    private bool EditMode => _hexPlanetHudRepo!.GetTileOverrider().EditMode;
    private int EditingTileId { get; set; }
    public void SelectEditingTile(Tile tile) => EditingTileId = tile.Id;
    public void CleanEditingTile() => EditingTileId = 0;

    public void Update(int pathFromTileId, Vector3 position)
    {
        if (EditMode)
            UpdateInEditMode(position);
        else
            UpdateInPlayMode(pathFromTileId, position);
    }

    private void UpdateInPlayMode(int pathFromTileId, Vector3 position)
    {
        if (pathFromTileId == 0)
        {
            Hide();
            return;
        }

        Show();
        var mesh = _selectViewService!.GenerateMeshForPlayMode(pathFromTileId, position);
        if (mesh != null)
            Mesh = mesh;
    }

    private void UpdateInEditMode(Vector3 position)
    {
        if (position != Vector3.Zero || EditingTileId > 0)
        {
            // 更新选择地块框
            Show();
            var mesh = _selectViewService!.GenerateMeshForEditMode(EditingTileId, position);
            if (mesh != null)
                Mesh = mesh;
        }
        else
        {
            // GD.Print("No tile under cursor, _selectTileViewer not visible");
            Hide();
        }
    }
}