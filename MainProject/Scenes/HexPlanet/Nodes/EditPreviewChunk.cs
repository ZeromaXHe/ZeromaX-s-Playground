using System.Collections.Generic;
using System.Linq;
using Apps.Contexts;
using Apps.Nodes;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Repos.PlanetGenerates;
using Domains.Services.Uis;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-09 14:19
public partial class EditPreviewChunk : Node3D, IChunk, IEditPreviewChunk
{
    public EditPreviewChunk()
    {
        _chunkTriangulation = new ChunkTriangulation(this);
        InitServices();
        NodeContext.Instance.RegisterSingleton<IEditPreviewChunk>(this);
    }

    [Export] public HexMesh? Terrain { get; set; }
    [Export] public ShaderMaterial[]? TerrainMaterials { get; set; }
    [Export] public HexMesh? Rivers { get; set; }
    [Export] public HexMesh? Roads { get; set; }
    [Export] public HexMesh? Water { get; set; }
    [Export] public HexMesh? WaterShore { get; set; }
    [Export] public HexMesh? Estuary { get; set; }
    [Export] public HexFeatureManager? Features { get; set; }

    #region 服务与存储

    private ITileRepo? _tileRepo;
    private IEditorService? _editorService;

    private void InitServices()
    {
        _tileRepo = Context.GetBeanFromHolder<ITileRepo>();
        _editorService = Context.GetBeanFromHolder<IEditorService>();
    }

    #endregion

    private readonly ChunkTriangulation _chunkTriangulation;
    public HexTileDataOverrider TileDataOverrider { get; set; } = new();
    private int _terrainMaterialIdx;

    public override void _ExitTree() => NodeContext.Instance.DestroySingleton<IEditPreviewChunk>();

    public override void _Process(double delta)
    {
        if (TileDataOverrider.OverrideTiles.Count > 0)
        {
            // var time = Time.GetTicksMsec();
            Terrain!.Clear();
            Rivers!.Clear();
            Roads!.Clear();
            Water!.Clear();
            WaterShore!.Clear();
            Estuary!.Clear();
#if FEATURE_NEW
            Features.Clear(true, false);
            foreach (var tile in TileDataOverrider.OverrideTiles)
                _featureApplication.HideFeatures(tile, true);
#else
            Features!.Clear();
#endif
            foreach (var tile in TileDataOverrider.OverrideTiles)
                _chunkTriangulation.Triangulate(tile);
            Terrain.Apply();
            Rivers.Apply();
            Roads.Apply();
            Water.Apply();
            WaterShore.Apply();
            Estuary.Apply();
            Features.Apply();
#if FEATURE_NEW
            foreach (var tile in TileDataOverrider.OverrideTiles)
                if (Visible)
                    _featureApplication.ShowFeatures(tile, false, true);
                else
                    _featureApplication.HideFeatures(tile, true);
#else
            if (Visible)
                Features.ShowFeatures(false);
            else
                Features.HideFeatures(false);
#endif
            // GD.Print($"EditPreviewChunk BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void Update(Tile? tile)
    {
        if (tile != null)
        {
            // 更新地块预览
            Refresh(_editorService!.TileOverrider,
                _tileRepo!.GetTilesInDistance(tile, _editorService.TileOverrider.BrushSize));
            Show();
        }
        else Hide();
    }

    private void Refresh(HexTileDataOverrider tileDataOverrider, IEnumerable<Tile> tiles)
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