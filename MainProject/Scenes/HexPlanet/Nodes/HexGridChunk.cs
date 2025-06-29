using System.Collections.Generic;
using System.Linq;
using Apps.Queries.Abstractions.Features;
using Commons.Utils;
using Contexts;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Planets;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Services.Abstractions.Shaders;
using Domains.Services.Abstractions.Uis;
using Godot;
using Infras.Readers.Abstractions.Caches;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-22 23:31
[Tool]
public partial class HexGridChunk : Node3D, IChunk, IHexGridChunk
{
    public HexGridChunk() => InitServices();

    [Export] public HexMesh? Terrain { get; set; }
    [Export] public HexMesh? Rivers { get; set; }
    [Export] public HexMesh? Roads { get; set; }
    [Export] public HexMesh? Water { get; set; }
    [Export] public HexMesh? WaterShore { get; set; }
    [Export] public HexMesh? Estuary { get; set; }
    [Export] public HexFeatureManager? Features { get; set; }
    public HexTileDataOverrider TileDataOverrider => new();
    [Export] private PackedScene? _labelScene;

    #region on-ready 节点

    private Node3D? _labels;

    private void InitOnReadyNodes()
    {
        _labels = GetNode<Node3D>("%Labels");
    }

    #endregion

    #region 服务与存储

    private static ILodMeshCache? _lodMeshCache;
    private static IPointRepo? _pointRepo;
    private static IChunkRepo? _chunkRepo;
    private static ITileRepo? _tileRepo;
    private static ITileShaderService? _tileShaderService;
    private static IPlanetConfig? _planetConfig;
    private static IEditorService? _editorService;
    private static IFeatureApplication? _featureApplication;

    private void InitServices()
    {
        _lodMeshCache ??= Context.GetBeanFromHolder<ILodMeshCache>();
        _pointRepo ??= Context.GetBeanFromHolder<IPointRepo>();
        _chunkRepo ??= Context.GetBeanFromHolder<IChunkRepo>();
        _tileRepo ??= Context.GetBeanFromHolder<ITileRepo>();
        _tileShaderService ??= Context.GetBeanFromHolder<ITileShaderService>();
        _planetConfig ??= Context.GetBeanFromHolder<IPlanetConfig>();
        _editorService ??= Context.GetBeanFromHolder<IEditorService>();
        _featureApplication ??= Context.GetBeanFromHolder<IFeatureApplication>();
    }

    private void OnEditorEditModeChanged(bool mode) =>
        RefreshTilesLabelMode(mode ? _editorService!.LabelMode : 0);

    private void CleanEventListeners()
    {
        _editorService!.LabelModeChanged -= RefreshTilesLabelMode;
        _editorService.EditModeChanged -= OnEditorEditModeChanged;
    }

    #endregion

    private static bool EditMode => _editorService!.TileOverrider.EditMode;
    private static int LabelMode => _editorService!.LabelMode;

    private int _id;
    private readonly Dictionary<int, HexTileLabel> _usingTileUis = new();
    private readonly Queue<HexTileLabel> _unusedTileUis = new();
    private ChunkTriangulation? _chunkTriangulation;

    private void HideLabel(int tileId)
    {
        var label = _usingTileUis[tileId];
        label.Hide();
        _usingTileUis.Remove(tileId);
        _unusedTileUis.Enqueue(label);
    }

    private void ShowLabel(int tileId)
    {
        HexTileLabel label;
        if (_unusedTileUis.Count == 0)
        {
            label = _labelScene!.Instantiate<HexTileLabel>();
            _labels!.AddChild(label);
        }
        else
            label = _unusedTileUis.Dequeue();

        var tile = _tileRepo!.GetById(tileId)!;
        var position = 1.01f * tile.GetCentroid(_planetConfig!.Radius + _tileRepo.GetHeight(tile));
        var scale = _planetConfig.StandardScale;
        label.Scale = Vector3.One * scale;
        label.Position = position;
        Node3dUtil.AlignYAxisToDirection(label, position, Vector3.Up);
        _usingTileUis.Add(tile.Id, label);
    }

    public void UsedBy(Chunk chunk)
    {
        InitLabels(chunk.Id);
        _id = chunk.Id;
        // 默认不生成网格，而是先查缓存
        SetProcess(false);
        ShowInSight(chunk.Lod);
    }

#if !FEATURE_NEW
    public void ExploreFeatures(int tileId) => Features!.ExploreFeatures(tileId);
#endif

    private void InitLabels(int id)
    {
        // 隐藏之前的标签
        foreach (var (tileId, _) in _usingTileUis)
            HideLabel(tileId);

        var tileIds = _chunkRepo!.GetById(id)!.TileIds;
        foreach (var tileId in tileIds)
            ShowLabel(tileId);
        // 在场景树中 _Ready 后 Label 才非 null
        RefreshTilesLabelMode(EditMode ? LabelMode : 0);
    }

    public void RefreshTileLabel(int tileId, string text) =>
        _usingTileUis[tileId].Label.Text = text;

    public void RefreshTilesLabelMode(int mode)
    {
        switch (mode)
        {
            case 0:
                // 不显示
                foreach (var (_, label) in _usingTileUis)
                {
                    label.Label.Text = "";
                    label.Label.FontSize = 64;
                    label.Hide();
                }

                break;
            case 1:
                // 坐标
                foreach (var (tileId, label) in _usingTileUis)
                {
                    var coords = _pointRepo!.GetSphereAxial(_tileRepo!.GetById(tileId)!);
                    label.Label.Text = $"{coords.Coords}\n{coords.Type},{coords.TypeIdx}";
                    label.Label.FontSize = 24;
                    label.Show();
                }

                break;
            case 2:
                // ID
                foreach (var (tileId, label) in _usingTileUis)
                {
                    label.Label.Text = tileId.ToString();
                    label.Label.FontSize = 64;
                    label.Show();
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
            var chunk = _chunkRepo!.GetById(_id)!;
            var tileIds = chunk.TileIds;
            var tiles = tileIds.Select(id => _tileRepo!.GetById(id)!).ToList();
            foreach (var tile in tiles)
            {
                _chunkTriangulation!.Triangulate(tile);
                _usingTileUis.TryGetValue(tile.Id, out var tileUi);
                if (tileUi != null)
                {
                    tileUi.Position =
                        1.01f * tile.GetCentroid(_planetConfig!.Radius + _tileRepo!.GetHeight(tile));
                }
                else GD.PrintErr($"Tile {tile.Id} UI not found");
            }

            ApplyNewData(!IsHandlingLodGaps(chunk));
#if FEATURE_NEW
            foreach (var tile in tiles)
                _featureApplication.ShowFeatures(tile, !EditMode, false);
#else
            Features!.ShowFeatures(!EditMode);
#endif
            // GD.Print($"Chunk {_id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    private void ApplyNewData(bool cacheMesh)
    {
        Terrain!.Apply();
        Rivers!.Apply(); // 河流暂时不支持 Lod
        Roads!.Apply(); // 道路暂时不支持 Lod
        Water!.Apply();
        WaterShore!.Apply();
        Estuary!.Apply();
        Features!.Apply(); // 特征暂时无 Lod

        if (!cacheMesh)
            return;
        var lod = _chunkTriangulation!.Lod;
        if (Terrain.Mesh == null)
            GD.PrintErr($"Chunk {_id} Terrain Mesh is null");
        _lodMeshCache!.AddLodMeshes(lod, _id,
            [Terrain.Mesh!, Water.Mesh!, WaterShore.Mesh!, Estuary.Mesh!]);
    }

    private void ClearOldData()
    {
        Terrain!.Clear();
        Rivers!.Clear();
        Roads!.Clear();
        Water!.Clear();
        WaterShore!.Clear();
        Estuary!.Clear();
#if FEATURE_NEW
        Features.Clear(false, true);
#else
        Features!.Clear();
#endif
    }

    private static bool IsHandlingLodGaps(Chunk chunk) =>
        (chunk.Lod == ChunkLod.PlaneHex && _chunkRepo!.GetNeighbors(chunk).Any(n => n.Lod >= ChunkLod.SimpleHex))
        || (chunk.Lod == ChunkLod.TerracesHex && _chunkRepo!.GetNeighbors(chunk).Any(n => n.Lod == ChunkLod.Full));

    public void UpdateLod(ChunkLod lod, bool idChanged = true)
    {
        if (lod == _chunkTriangulation!.Lod && !idChanged) return;
        _chunkTriangulation.Lod = lod;

        var chunk = _chunkRepo!.GetById(_id)!; // 获取当前分块
        if (IsHandlingLodGaps(chunk))
        {
            // 对于需要处理接缝的情况，不使用缓存
            SetProcess(true);
            return;
        }

        var meshes = _lodMeshCache!.GetLodMeshes(lod, _id);
        // 如果之前生成过 Lod 网格，直接应用；否则重新生成
        if (meshes != null)
        {
            Terrain!.ShowMesh(meshes[(int)MeshType.Terrain]);
            Water!.ShowMesh(meshes[(int)MeshType.Water]);
            WaterShore!.ShowMesh(meshes[(int)MeshType.WaterShore]);
            Estuary!.ShowMesh(meshes[(int)MeshType.Estuary]);
        }
        else SetProcess(true);
    }

    public void Refresh()
    {
        // 让所有旧的网格缓存过期
        _lodMeshCache!.RemoveLodMeshes(_id);
        SetProcess(true);
    }

    public void ShowUi(bool show) => _labels!.Visible = show;

    private void ShowInSight(ChunkLod lod)
    {
        Show();
        UpdateLod(lod);
#if FEATURE_NEW
        // 编辑模式下全部显示，游戏模式下仅显示探索过的
        foreach (var tile in _chunkRepo.GetById(_id)!.TileIds.Select(_tileRepo.GetById))
            _featureApplication.ShowFeatures(tile, !EditMode, false);
#else
        Features!.ShowFeatures(!EditMode); // 编辑模式下全部显示，游戏模式下仅显示探索过的
#endif
        OnEditorEditModeChanged(EditMode);
        _editorService!.LabelModeChanged += RefreshTilesLabelMode;
        _editorService.EditModeChanged += OnEditorEditModeChanged;
    }

    public void HideOutOfSight()
    {
        Hide();
        Terrain!.Clear();
        Rivers!.Clear();
        Roads!.Clear();
        Water!.Clear();
        WaterShore!.Clear();
        Estuary!.Clear();
#if FEATURE_NEW
        Features.Clear(false, false);
#else
        Features!.Clear();
#endif
        _id = 0; // 重置 id，归还给池子
        _editorService!.LabelModeChanged -= RefreshTilesLabelMode;
        _editorService.EditModeChanged -= OnEditorEditModeChanged;
    }
}