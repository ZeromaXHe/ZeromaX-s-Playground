using System.Collections.Generic;
using Contexts;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-22 23:31
[Tool]
public partial class HexGridChunk : Node3D, IHexGridChunk
{
    public HexGridChunk()
    {
        Context.RegisterToHolder<IHexGridChunk>(this);
    }

    public NodeEvent NodeEvent { get; } = new(process: true);
    public override void _Ready() => InitOnReadyNodes();
    public override void _Process(double delta) => NodeEvent.EmitProcessed(delta);

    [Export] public HexMesh? Terrain { get; set; }
    public IHexMesh? GetTerrain() => Terrain;
    [Export] public HexMesh? Rivers { get; set; }
    public IHexMesh? GetRivers() => Rivers;
    [Export] public HexMesh? Roads { get; set; }
    public IHexMesh? GetRoads() => Roads;
    [Export] public HexMesh? Water { get; set; }
    public IHexMesh? GetWater() => Water;
    [Export] public HexMesh? WaterShore { get; set; }
    public IHexMesh? GetWaterShore() => WaterShore;
    [Export] public HexMesh? Estuary { get; set; }
    public IHexMesh? GetEstuary() => Estuary;
    [Export] public HexFeatureManager? Features { get; set; }
    public IHexFeatureManager? GetFeatures() => Features;
    [Export] private PackedScene? _labelScene;

    public HexTileDataOverrider TileDataOverrider { get; set; } = new();
    public ChunkLod Lod { get; set; } = ChunkLod.JustHex;

    #region on-ready 节点

    private Node3D? _labels;

    private void InitOnReadyNodes()
    {
        _labels = GetNode<Node3D>("%Labels");
    }

    #endregion

    public int Id { get; set; }
    public Dictionary<int, IHexTileLabel> UsingTileUis { get; } = new();
    public Queue<IHexTileLabel> UnusedTileUis { get; } = new();

    public IHexTileLabel InstantiateHexTileLabel()
    {
        var label = _labelScene!.Instantiate<HexTileLabel>();
        _labels!.AddChild(label);
        return label;
    }

    public void RefreshTileLabel(int tileId, string text) =>
        UsingTileUis[tileId].Label!.Text = text;

    public void ShowUi(bool show) => _labels!.Visible = show;
}