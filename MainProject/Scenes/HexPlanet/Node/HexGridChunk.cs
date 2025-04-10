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

    private int _id;
    private readonly Dictionary<int, HexTileLabel> _tileUis = new();
    private ChunkTriangulation _chunkTriangulation;

    #region on-ready 节点

    private Node3D _labels;

    private void InitOnReadyNodes()
    {
        _labels = GetNode<Node3D>("%Labels");
    }

    #endregion

    #region services

    private IChunkService _chunkService;
    private ITileService _tileService;
    private ITileShaderService _tileShaderService;

    private void InitServices()
    {
        _chunkService = Context.GetBean<IChunkService>();
        _tileService = Context.GetBean<ITileService>();
        _tileShaderService = Context.GetBean<ITileShaderService>();
        _tileShaderService.TileExplored += ExploreFeatures;
    }

    #endregion

    public void Init(int id, int mode)
    {
        _id = id;
        InitLabels(mode);
        Refresh();
    }

    private void ExploreFeatures(int tileId) => Features.ExploreFeatures(tileId);

    private void InitLabels(int mode)
    {
        // 清楚之前的标签

        var tileIds = _chunkService.GetById(_id).TileIds;
        var tiles = tileIds.Select(_tileService.GetById);
        foreach (var tile in tiles)
        {
            var label = _labelScene.Instantiate<HexTileLabel>();
            var position = 1.01f * tile.GetCentroid(HexMetrics.Radius + _tileService.GetHeight(tile));
            var scale = HexMetrics.StandardScale;
            label.Scale = Vector3.One * scale;
            label.Position = position;
            Node3dUtil.AlignYAxisToDirection(label, position, Vector3.Up);
            _tileUis.Add(tile.Id, label);
            _labels.AddChild(label);
        }
        // 在场景树中 _Ready 后 Label 才非 null
        RefreshTilesLabelMode(mode);
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

    public override void _Process(double delta)
    {
        if (_id > 0)
        {
            var time = Time.GetTicksMsec();
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
                _tileUis[tile.Id].Position = 1.01f * tile.GetCentroid(HexMetrics.Radius + _tileService.GetHeight(tile));
            }

            Terrain.Apply();
            Rivers.Apply();
            Roads.Apply();
            Water.Apply();
            WaterShore.Apply();
            Estuary.Apply();
            Features.Apply();
            GD.Print($"Chunk {_id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void Refresh() => SetProcess(true);
    public void ShowUi(bool show) => _labels.Visible = show;
    public void ShowUnexploredFeatures(bool show) => Features.ShowUnexploredFeatures(show);
}