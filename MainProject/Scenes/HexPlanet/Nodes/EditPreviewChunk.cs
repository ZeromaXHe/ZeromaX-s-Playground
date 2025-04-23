using System.Collections.Generic;
using System.Linq;
using Apps.Queries.Contexts;
using Contexts;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-09 14:19
public partial class EditPreviewChunk : Node3D, IEditPreviewChunk
{
    public EditPreviewChunk()
    {
        NodeContext.Instance.RegisterSingleton<IEditPreviewChunk>(this);
        Context.RegisterToHolder<IEditPreviewChunk>(this);
    }

    public NodeEvent NodeEvent { get; } = new(process: true);
    public override void _ExitTree() => NodeContext.Instance.DestroySingleton<IEditPreviewChunk>();
    public override void _Process(double delta) => NodeEvent.EmitProcessed(delta);

    [Export] public HexMesh? Terrain { get; set; }
    public IHexMesh? GetTerrain() => Terrain;
    [Export] public ShaderMaterial[]? TerrainMaterials { get; set; }
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

    public HexTileDataOverrider TileDataOverrider { get; set; } = new();
    public ChunkLod Lod { get; set; } = ChunkLod.Full; // 默认值是给预览用的
    private int _terrainMaterialIdx;

    public void Refresh(HexTileDataOverrider tileDataOverrider, IEnumerable<Tile> tiles)
    {
        TileDataOverrider = tileDataOverrider with { OverrideTiles = tiles.ToHashSet() };
        var newMaterialIdx = GetTerrainMaterialIdx();
        if (newMaterialIdx != _terrainMaterialIdx)
        {
            Terrain!.MaterialOverride = TerrainMaterials![newMaterialIdx];
            _terrainMaterialIdx = newMaterialIdx;
        }

        SetProcess(true);
    }

    private int GetTerrainMaterialIdx()
    {
        if (!TileDataOverrider.ApplyTerrain) return 0;
        return TileDataOverrider.ActiveTerrain + 1;
    }
}