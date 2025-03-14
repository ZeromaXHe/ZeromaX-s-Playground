using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexGridChunk : Node3D, IChunk
{
    public HexGridChunk() => InitServices();

    [Export] public HexMesh Terrain { get; set; }
    [Export] public HexMesh Rivers { get; set; }
    [Export] public HexMesh Roads { get; set; }
    [Export] public HexMesh Water { get; set; }
    [Export] public HexMesh WaterShore { get; set; }
    [Export] public HexMesh Estuary { get; set; }
    [Export] public HexFeatureManager Features { get; set; }
    public HexTileDataOverrider TileDataOverrider => new();
    [Export] private PackedScene _labelScene;

    #region on-ready 节点

    private Node3D _labels;

    private void InitOnReadyNodes()
    {
        _labels = GetNode<Node3D>("%Labels");
    }

    #endregion

    #region services

    private static IChunkService _chunkService;
    private static ITileService _tileService;
    private static ITileShaderService _tileShaderService;
    private static IPlanetSettingService _planetSettingService;
    private static IEditorService _editorService;

    private void InitServices()
    {
        _chunkService ??= Context.GetBean<IChunkService>();
        _tileService ??= Context.GetBean<ITileService>();
        _tileShaderService ??= Context.GetBean<ITileShaderService>();
        _planetSettingService ??= Context.GetBean<IPlanetSettingService>();
        _editorService ??= Context.GetBean<IEditorService>();
    }

    private void OnEditorEditModeChanged(bool mode)
    {
        switch (mode)
        {
            // 开启编辑模式
            case true when _editorService.LabelMode != 0:
                RefreshTilesLabelMode(_editorService.LabelMode);
                break;
            // 关闭编辑模式
            case false:
                RefreshTilesLabelMode(0);
                break;
        }
    }

    private void CleanEventListeners()
    {
        _editorService.LabelModeChanged -= RefreshTilesLabelMode;
        _editorService.EditModeChanged -= OnEditorEditModeChanged;
    }

    #endregion

    private bool EditMode => _editorService.TileOverrider.EditMode;
    private int LabelMode => _editorService.LabelMode;

    private int _id;
    private readonly Dictionary<int, HexTileLabel> _tileUis = new();
    private ChunkTriangulation _chunkTriangulation;

    public void Init(int id)
    {
        _id = id;
        InitLabels();
        Refresh();
    }

    public void ExploreFeatures(int tileId) => Features.ExploreFeatures(tileId);

    private void InitLabels()
    {
        // 清楚之前的标签
        var tileIds = _chunkService.GetById(_id).TileIds;
        var tiles = tileIds.Select(_tileService.GetById);
        foreach (var tile in tiles)
        {
            var label = _labelScene.Instantiate<HexTileLabel>();
            var position = 1.01f * tile.GetCentroid(_planetSettingService.Radius + _tileService.GetHeight(tile));
            var scale = _planetSettingService.StandardScale;
            label.Scale = Vector3.One * scale;
            label.Position = position;
            Node3dUtil.AlignYAxisToDirection(label, position, Vector3.Up);
            _tileUis.Add(tile.Id, label);
            _labels.AddChild(label);
        }

        // 在场景树中 _Ready 后 Label 才非 null
        RefreshTilesLabelMode(EditMode ? LabelMode : 0);
    }

    public void RefreshTileLabel(int tileId, string text) =>
        _tileUis[tileId].Label.Text = text;

    public void RefreshTilesLabelMode(int mode)
    {
        switch (mode)
        {
            case 0:
                // 不显示
                foreach (var (_, label) in _tileUis)
                {
                    label.Label.Text = "";
                    label.Label.FontSize = 64;
                }

                break;
            case 1:
                // 坐标
                foreach (var (tileId, label) in _tileUis)
                {
                    var coords = _tileService.GetSphereAxial(_tileService.GetById(tileId));
                    label.Label.Text = $"{coords.Coords}\n{coords.Type},{coords.TypeIdx}";
                    label.Label.FontSize = 24;
                }

                break;
            case 2:
                // ID
                foreach (var (tileId, label) in _tileUis)
                {
                    label.Label.Text = tileId.ToString();
                    label.Label.FontSize = 64;
                }

                break;
        }
    }

    public override void _Ready()
    {
        InitOnReadyNodes();
        _chunkTriangulation = new ChunkTriangulation(this);
    }

    public override void _ExitTree() => CleanEventListeners();

    public override void _Process(double delta)
    {
        if (_id > 0)
        {
            // var time = Time.GetTicksMsec();
            Terrain.Clear();
            Rivers.Clear();
            Roads.Clear();
            Water.Clear();
            WaterShore.Clear();
            Estuary.Clear();
            Features.Clear();
            var tileIds = _chunkService.GetById(_id).TileIds;
            var tiles = tileIds.Select(_tileService.GetById);
            foreach (var tile in tiles)
            {
                _chunkTriangulation.Triangulate(tile);
                _tileUis[tile.Id].Position =
                    1.01f * tile.GetCentroid(_planetSettingService.Radius + _tileService.GetHeight(tile));
            }

            Terrain.Apply();
            Rivers.Apply();
            Roads.Apply();
            Water.Apply();
            WaterShore.Apply();
            Estuary.Apply();
            Features.Apply();
            if (Visible)
                Features.ShowFeatures(!EditMode);
            // GD.Print($"Chunk {_id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void Refresh() => SetProcess(true);
    public void ShowUi(bool show) => _labels.Visible = show;

    public void ShowInSight()
    {
        Show();
        Features.ShowFeatures(!EditMode); // 编辑模式下全部显示，游戏模式下仅显示探索过的
        OnEditorEditModeChanged(EditMode);
        _editorService.LabelModeChanged += RefreshTilesLabelMode;
        _editorService.EditModeChanged += OnEditorEditModeChanged;
    }

    public void HideOutOfSight()
    {
        Hide();
        Features.HideFeatures(false);
        _editorService.LabelModeChanged -= RefreshTilesLabelMode;
        _editorService.EditModeChanged -= OnEditorEditModeChanged;
    }
}