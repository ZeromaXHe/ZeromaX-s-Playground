using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-09 14:19
public partial class EditPreviewChunk : Node3D, IChunk
{
    public EditPreviewChunk() =>
        _chunkTriangulation = new ChunkTriangulation(this);

    [Export] public HexMesh Terrain { get; set; }
    [Export] public ShaderMaterial[] TerrainMaterials { get; set; }
    [Export] public HexMesh Rivers { get; set; }
    [Export] public HexMesh Roads { get; set; }
    [Export] public HexMesh Water { get; set; }
    [Export] public HexMesh WaterShore { get; set; }
    [Export] public HexMesh Estuary { get; set; }
    [Export] public HexFeatureManager Features { get; set; }

    private readonly ChunkTriangulation _chunkTriangulation;
    public HexTileDataOverrider TileDataOverrider { get; set; } = new();
    private int _terrainMaterialIdx = 0;

    public override void _Process(double delta)
    {
        if (TileDataOverrider.OverrideTiles.Count > 0)
        {
            // var time = Time.GetTicksMsec();
            Terrain.Clear();
            Rivers.Clear();
            Roads.Clear();
            Water.Clear();
            WaterShore.Clear();
            Estuary.Clear();
            Features.Clear();
            foreach (var tile in TileDataOverrider.OverrideTiles)
                _chunkTriangulation.Triangulate(tile);
            Terrain.Apply();
            Rivers.Apply();
            Roads.Apply();
            Water.Apply();
            WaterShore.Apply();
            Estuary.Apply();
            Features.Apply();
            if (Visible)
                Features.ShowFeatures(false);
            else
                Features.HideFeatures(false);
            // GD.Print($"EditPreviewChunk BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void Refresh(HexTileDataOverrider tileDataOverrider, IEnumerable<Tile> tiles)
    {
        TileDataOverrider = tileDataOverrider with { OverrideTiles = tiles.ToHashSet() };
        var newMaterialIdx = GetTerrainMaterialIdx();
        if (newMaterialIdx != _terrainMaterialIdx)
        {
            Terrain.MaterialOverride = TerrainMaterials[newMaterialIdx];
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