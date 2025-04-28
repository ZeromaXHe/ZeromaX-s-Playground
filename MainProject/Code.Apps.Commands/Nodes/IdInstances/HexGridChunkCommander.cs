using Domains.Services.Abstractions.Nodes.IdInstances;
using Domains.Services.Abstractions.Nodes.Singletons.ChunkManagers;
using Godot;
using Infras.Readers.Abstractions.Caches;
using Infras.Readers.Abstractions.Nodes.IdInstances;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Apps.Commands.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 14:18:07
public class HexGridChunkCommander
{
    private readonly IHexGridChunkService _hexGridChunkService;
    private readonly IHexGridChunkRepo _hexGridChunkRepo;

    private readonly IChunkTriangulationService _chunkTriangulationService;
    private readonly IHexPlanetHudRepo _hexPlanetHudRepo;
    private readonly IHexPlanetManagerRepo _hexPlanetManagerRepo;
    private readonly IChunkRepo _chunkRepo;
    private readonly ITileRepo _tileRepo;
    private readonly ILodMeshCache _lodMeshCache;

    public HexGridChunkCommander(IHexGridChunkService hexGridChunkService, IHexGridChunkRepo hexGridChunkRepo,
        IChunkTriangulationService chunkTriangulationService, IHexPlanetHudRepo hexPlanetHudRepo,
        IHexPlanetManagerRepo hexPlanetManagerRepo, IChunkRepo chunkRepo, ITileRepo tileRepo,
        ILodMeshCache lodMeshCache)
    {
        _hexGridChunkService = hexGridChunkService;
        _hexGridChunkRepo = hexGridChunkRepo;
        _hexGridChunkRepo.Processed += OnProcessed;

        _chunkTriangulationService = chunkTriangulationService;
        _hexPlanetHudRepo = hexPlanetHudRepo;
        _hexPlanetHudRepo.LabelModeChanged += _hexGridChunkService.RefreshTilesLabelMode;
        _hexPlanetHudRepo.EditModeChanged += _hexGridChunkService.OnEditorEditModeChanged;
        _hexPlanetManagerRepo = hexPlanetManagerRepo;
        _chunkRepo = chunkRepo;
        _chunkRepo.RefreshChunkTileLabel += _hexGridChunkRepo.OnChunkServiceRefreshChunkTileLabel;
        _tileRepo = tileRepo;
        _tileRepo.RefreshChunk += _hexGridChunkService.RefreshChunk;
        _lodMeshCache = lodMeshCache;
    }


    public void ReleaseEvents()
    {
        _hexGridChunkRepo.Processed -= OnProcessed;
        _hexPlanetHudRepo.LabelModeChanged -= _hexGridChunkService.RefreshTilesLabelMode;
        _hexPlanetHudRepo.EditModeChanged -= _hexGridChunkService.OnEditorEditModeChanged;
        _chunkRepo.RefreshChunkTileLabel -= _hexGridChunkRepo.OnChunkServiceRefreshChunkTileLabel;
        _tileRepo.RefreshChunk -= _hexGridChunkService.RefreshChunk;
    }

    private void OnProcessed(IHexGridChunk instance, double delta)
    {
        if (instance.Id > 0)
        {
            // var time = Time.GetTicksMsec();
            ClearOldData(instance);
            var chunk = _chunkRepo.GetById(instance.Id)!;
            var tileIds = chunk.TileIds;
            var tiles = tileIds.Select(id => _tileRepo.GetById(id)!).ToList();
            foreach (var tile in tiles)
            {
                _chunkTriangulationService.Triangulate(tile, instance);
                instance.UsingTileUis.TryGetValue(tile.Id, out var tileUi);
                if (tileUi != null)
                {
                    tileUi.Position =
                        1.01f * tile.GetCentroid(_hexPlanetManagerRepo.Radius + _hexPlanetManagerRepo.GetHeight(tile));
                }
                else GD.PrintErr($"Tile {tile.Id} UI not found");
            }

            ApplyNewData(instance, !_hexGridChunkService.IsHandlingLodGaps(chunk));
            foreach (var tile in tiles)
                _hexGridChunkService.ShowFeatures(tile, !_hexPlanetHudRepo.GetEditMode(), false);
            // GD.Print($"Chunk {_id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        instance.SetProcess(false);
    }

    private void ApplyNewData(IHexGridChunk instance, bool cacheMesh)
    {
        var terrain = instance.GetTerrain()!;
        var water = instance.GetWater()!;
        var waterShore = instance.GetWaterShore()!;
        var estuary = instance.GetEstuary()!;
        terrain.Apply();
        instance.GetRivers()!.Apply(); // 河流暂时不支持 Lod
        instance.GetRoads()!.Apply(); // 道路暂时不支持 Lod
        water.Apply();
        waterShore.Apply();
        estuary.Apply();
        instance.GetFeatures()!.Apply(); // 特征暂时无 Lod

        if (!cacheMesh)
            return;
        var lod = instance.Lod;
        if (terrain.Mesh == null)
            GD.PrintErr($"Chunk {instance.Id} Terrain Mesh is null");
        _lodMeshCache.AddLodMeshes(lod, instance.Id,
            [terrain.Mesh!, water.Mesh!, waterShore.Mesh!, estuary.Mesh!]);
    }

    private void ClearOldData(IHexGridChunk instance)
    {
        instance.GetTerrain()!.Clear();
        instance.GetRivers()!.Clear();
        instance.GetRoads()!.Clear();
        instance.GetWater()!.Clear();
        instance.GetWaterShore()!.Clear();
        instance.GetEstuary()!.Clear();
        instance.GetFeatures()!.Clear(false, _hexGridChunkService.ClearFeatures);
    }
}