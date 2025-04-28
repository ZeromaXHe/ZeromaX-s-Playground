using Domains.Services.Abstractions.Nodes.IdInstances;
using Domains.Services.Abstractions.Nodes.Singletons.ChunkManagers;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Apps.Commands.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:42:32
public class EditPreviewChunkCommander
{
    private readonly IEditPreviewChunkRepo _editPreviewChunkRepo;

    private readonly IChunkTriangulationService _chunkTriangulationService;
    private readonly IHexGridChunkService _hexGridChunkService;
    private readonly ITileRepo _tileRepo;

    public EditPreviewChunkCommander(IEditPreviewChunkRepo editPreviewChunkRepo,
        IChunkTriangulationService chunkTriangulationService, IHexGridChunkService hexGridChunkService,
        ITileRepo tileRepo)
    {
        _editPreviewChunkRepo = editPreviewChunkRepo;
        _editPreviewChunkRepo.Processed += OnProcessed;

        _chunkTriangulationService = chunkTriangulationService;
        _hexGridChunkService = hexGridChunkService;
        _tileRepo = tileRepo;
        _tileRepo.RefreshChunk += RefreshChunk;
    }

    public void ReleaseEvents()
    {
        _editPreviewChunkRepo.Processed -= OnProcessed;
        _tileRepo.RefreshChunk -= RefreshChunk;
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
            feature.Clear(true, _hexGridChunkService.ClearFeatures);
            foreach (var tile in Self.TileDataOverrider.OverrideTiles)
                _hexGridChunkService.ClearFeatures(tile, true);
            foreach (var tile in Self.TileDataOverrider.OverrideTiles)
                _chunkTriangulationService.Triangulate(tile, Self);
            terrain.Apply();
            rivers.Apply();
            roads.Apply();
            water.Apply();
            waterShore.Apply();
            estuary.Apply();
            feature.Apply();
            foreach (var tile in Self.TileDataOverrider.OverrideTiles)
                if (Self.Visible)
                    _hexGridChunkService.ShowFeatures(tile, false, true);
                else
                    _hexGridChunkService.HideFeatures(tile, true);
            // GD.Print($"EditPreviewChunk BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        Self.SetProcess(false);
    }

    private void RefreshChunk(int id)
    {
        if (_editPreviewChunkRepo.IsRegistered())
            Self.SetProcess(true);
    }
}