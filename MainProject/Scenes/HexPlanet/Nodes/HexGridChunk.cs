using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Impl;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-22 23:31
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
    [Export] public Nodes.HexFeatureManager Features { get; set; }
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

    private void OnEditorEditModeChanged(bool mode) =>
        RefreshTilesLabelMode(mode ? _editorService.LabelMode : 0);

    private void CleanEventListeners()
    {
        _editorService.LabelModeChanged -= RefreshTilesLabelMode;
        _editorService.EditModeChanged -= OnEditorEditModeChanged;
    }

    #endregion

    private static bool EditMode => _editorService.TileOverrider.EditMode;
    private static int LabelMode => _editorService.LabelMode;

    private int _id;
    private readonly Dictionary<int, HexTileLabel> _tileUis = new();
    private ChunkTriangulation _chunkTriangulation;
    private readonly bool[] _lodInitialized = new bool[System.Enum.GetValues<ChunkLod>().Length];

    public void Init(int id)
    {
        InitLabels(id);
        _id = id;
    }

    public void ExploreFeatures(int tileId) => Features.ExploreFeatures(tileId);

    private void InitLabels(int id)
    {
        // 清楚之前的标签
        var tileIds = _chunkService.GetById(id).TileIds;
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
            ClearOldData();
            var tileIds = _chunkService.GetById(_id).TileIds;
            var tiles = tileIds.Select(_tileService.GetById);
            foreach (var tile in tiles)
            {
                _chunkTriangulation.Triangulate(tile);
                _tileUis.TryGetValue(tile.Id, out var tileUi);
                if (tileUi != null)
                {
                    tileUi.Position =
                        1.01f * tile.GetCentroid(_planetSettingService.Radius + _tileService.GetHeight(tile));
                }
                else GD.PrintErr($"Tile {tile.Id} UI not found");
            }

            ApplyNewData();
            Features.ShowFeatures(!EditMode);
            // GD.Print($"Chunk {_id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    private void ApplyNewData()
    {
        var lod = _chunkTriangulation.Lod;
        _lodInitialized[(int)lod] = true;
        Terrain.Apply(lod);
        Rivers.Apply(); // 河流暂时不支持 Lod
        Roads.Apply(); // 道路暂时不支持 Lod
        Water.Apply(lod);
        WaterShore.Apply(lod);
        Estuary.Apply(lod);
        Features.Apply(); // 特征暂时无 Lod
    }

    private void ClearOldData()
    {
        Terrain.Clear();
        Rivers.Clear();
        Roads.Clear();
        Water.Clear();
        WaterShore.Clear();
        Estuary.Clear();
        Features.Clear();
        for (var i = 0; i < _lodInitialized.Length; i++)
            _lodInitialized[i] = false;
    }

    public void UpdateLod(ChunkLod lod)
    {
        if (lod == _chunkTriangulation.Lod) return;
        _chunkTriangulation.Lod = lod;
        // 如果之前生成过 Lod 网格，直接应用；否则重新生成
        if (_lodInitialized[(int)lod])
        {
            Terrain.ShowLod(lod);
            Water.ShowLod(lod);
            WaterShore.ShowLod(lod);
            Estuary.ShowLod(lod);
        }
        else SetProcess(true);
    }

    public void Refresh() => SetProcess(true);
    public void ShowUi(bool show) => _labels.Visible = show;

    public void ShowInSight(ChunkLod lod)
    {
        Show();
        UpdateLod(lod);
        Features.ShowFeatures(!EditMode); // 编辑模式下全部显示，游戏模式下仅显示探索过的
        OnEditorEditModeChanged(EditMode);
        _editorService.LabelModeChanged += RefreshTilesLabelMode;
        _editorService.EditModeChanged += OnEditorEditModeChanged;
    }

    public void HideOutOfSight()
    {
        Hide();
        _editorService.LabelModeChanged -= RefreshTilesLabelMode;
        _editorService.EditModeChanged -= OnEditorEditModeChanged;
    }
}