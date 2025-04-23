using Domains.Services.Abstractions.Nodes.ChunkManagers;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Nodes.Abstractions;

namespace Apps.Commands;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:42:32
public class EditPreviewChunkCommander
{
    private readonly IEditPreviewChunkRepo _editPreviewChunkRepo;

    private readonly IChunkTriangulationService _chunkTriangulationService;

    public EditPreviewChunkCommander(IEditPreviewChunkRepo editPreviewChunkRepo,
        IChunkTriangulationService chunkTriangulationService)
    {
        _editPreviewChunkRepo = editPreviewChunkRepo;
        _editPreviewChunkRepo.Processed += OnProcessed;

        _chunkTriangulationService = chunkTriangulationService;
    }

    private IEditPreviewChunk Self => _editPreviewChunkRepo.Singleton!;

    private void OnProcessed(double delta)
    {
        if (Self.TileDataOverrider.OverrideTiles.Count > 0)
        {
            // var time = Time.GetTicksMsec();
            var terrain = Self.GetTerrain()!;
            var rivers = Self.GetRivers()!;
            var roads = Self.GetRoads()!;
            var water = Self.GetWater()!;
            var waterShore = Self.GetWaterShore()!;
            var estuary = Self.GetEstuary()!;
            var feature = Self.GetFeatures()!;
            terrain.Clear();
            rivers.Clear();
            roads.Clear();
            water.Clear();
            waterShore.Clear();
            estuary.Clear();
#if FEATURE_NEW
            Features.Clear(true, false);
            foreach (var tile in TileDataOverrider.OverrideTiles)
                _featureApplication.HideFeatures(tile, true);
#else
            feature.Clear();
#endif
            foreach (var tile in Self.TileDataOverrider.OverrideTiles)
                _chunkTriangulationService.Triangulate(tile, Self);
            terrain.Apply();
            rivers.Apply();
            roads.Apply();
            water.Apply();
            waterShore.Apply();
            estuary.Apply();
            feature.Apply();
#if FEATURE_NEW
            foreach (var tile in TileDataOverrider.OverrideTiles)
                if (Visible)
                    _featureApplication.ShowFeatures(tile, false, true);
                else
                    _featureApplication.HideFeatures(tile, true);
#else
            if (Self.Visible)
                feature.ShowFeatures(false);
            else
                feature.HideFeatures(false);
#endif
            // GD.Print($"EditPreviewChunk BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        Self.SetProcess(false);
    }
}