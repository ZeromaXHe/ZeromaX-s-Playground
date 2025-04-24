using Domains.Models.Entities.Civs;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.Singletons;
using Domains.Services.Abstractions.Nodes.Singletons.ChunkManagers;
using Domains.Services.Abstractions.Nodes.Singletons.Planets;
using Domains.Services.Abstractions.PlanetGenerates;
using Domains.Services.Abstractions.Searches;
using Domains.Services.Abstractions.Shaders;
using Godot;
using Infras.Readers.Abstractions.Caches;
using Infras.Readers.Abstractions.Nodes.IdInstances;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Writers.Abstractions.Civs;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Apps.Commands.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 14:53:49
public class HexPlanetManagerCommander
{
    private readonly IHexPlanetManagerRepo _hexPlanetManagerRepo;

    private readonly IHexMapGeneratorService _hexMapGeneratorService;
    private readonly ICelestialMotionManagerService _celestialMotionManagerService;
    private readonly ISelectTileViewerService _selectTileViewerService;
    private readonly IUnitManagerService _unitManagerService;
    private readonly IChunkLoaderService _chunkLoaderService;
    private readonly ITileSearchService _tileSearchService;
    private readonly ITileShaderService _tileShaderService;
    private readonly IChunkService _chunkService;
    private readonly ITileService _tileService;
    private readonly IChunkRepo _chunkRepo;
    private readonly ITileRepo _tileRepo;
    private readonly IPointRepo _pointRepo;
    private readonly IFaceRepo _faceRepo;
    private readonly ICivRepo _civRepo;
    private readonly IHexPlanetHudRepo _hexPlanetHudRepo;
    private readonly IHexGridChunkRepo _hexGridChunkRepo;
    private readonly ISelectTileViewerRepo _selectTileViewerRepo;
    private readonly IChunkLoaderRepo _chunkLoaderRepo;
    private readonly IFeatureMeshManagerRepo _featureMeshManagerRepo;
    private readonly IFeaturePreviewManagerRepo _featurePreviewManagerRepo;
    private readonly IEditPreviewChunkRepo _editPreviewChunkRepo;
    private readonly IUnitManagerRepo _unitManagerRepo;
    private readonly ILodMeshCache _lodMeshCache;

    public HexPlanetManagerCommander(IHexPlanetManagerRepo hexPlanetManagerRepo,
        IHexMapGeneratorService hexMapGeneratorService, ICelestialMotionManagerService celestialMotionManagerService,
        ISelectTileViewerService selectTileViewerService, IUnitManagerService unitManagerService,
        IChunkLoaderService chunkLoaderService, ITileSearchService tileSearchService,
        ITileShaderService tileShaderService, IChunkService chunkService, ITileService tileService,
        IChunkRepo chunkRepo, ITileRepo tileRepo, IPointRepo pointRepo, IFaceRepo faceRepo, ICivRepo civRepo,
        IHexPlanetHudRepo hexPlanetHudRepo, IHexGridChunkRepo hexGridChunkRepo,
        ISelectTileViewerRepo selectTileViewerRepo, IChunkLoaderRepo chunkLoaderRepo,
        IFeatureMeshManagerRepo featureMeshManagerRepo, IFeaturePreviewManagerRepo featurePreviewManagerRepo,
        IEditPreviewChunkRepo editPreviewChunkRepo, IUnitManagerRepo unitManagerRepo, ILodMeshCache lodMeshCache)
    {
        _hexPlanetManagerRepo = hexPlanetManagerRepo;
        _hexPlanetManagerRepo.Ready += OnReady;
        _hexPlanetManagerRepo.TreeExiting += OnTreeExiting;
        _hexPlanetManagerRepo.Processed += OnProcessed;
        _hexPlanetManagerRepo.RadiusChanged += OnRadiusChanged;

        _hexMapGeneratorService = hexMapGeneratorService;
        _celestialMotionManagerService = celestialMotionManagerService;
        _selectTileViewerService = selectTileViewerService;
        _unitManagerService = unitManagerService;
        _chunkLoaderService = chunkLoaderService;
        _tileSearchService = tileSearchService;
        _tileShaderService = tileShaderService;
        _chunkService = chunkService;
        _tileService = tileService;
        _chunkRepo = chunkRepo;
        _tileRepo = tileRepo;
        _pointRepo = pointRepo;
        _faceRepo = faceRepo;
        _civRepo = civRepo;
        _hexPlanetHudRepo = hexPlanetHudRepo;
        _hexGridChunkRepo = hexGridChunkRepo;
        _selectTileViewerRepo = selectTileViewerRepo;
        _chunkLoaderRepo = chunkLoaderRepo;
        _featureMeshManagerRepo = featureMeshManagerRepo;
        _featurePreviewManagerRepo = featurePreviewManagerRepo;
        _editPreviewChunkRepo = editPreviewChunkRepo;
        _unitManagerRepo = unitManagerRepo;
        _lodMeshCache = lodMeshCache;
    }

    public void ReleaseEvents()
    {
        _hexPlanetManagerRepo.Ready -= OnReady;
        _hexPlanetManagerRepo.TreeExiting -= OnTreeExiting;
        _hexPlanetManagerRepo.Processed -= OnProcessed;
        _hexPlanetManagerRepo.RadiusChanged -= OnRadiusChanged;
    }

    private bool NodeReady { get; set; }
    private IHexPlanetManager Self => _hexPlanetManagerRepo.Singleton!;

    private void OnReady()
    {
        NodeReady = true;
        _hexPlanetHudRepo.EditModeChanged += OnEditorEditModeChanged;
        // 在 _ready = true 后初始化相关数据。手动赋值调用 setter 一下
        Self.Radius = Self.Radius;
        Self.Divisions = Self.Divisions;

        Self.NoiseSourceImage = Self.NoiseSource!.GetImage();
        _hexPlanetManagerRepo.InitializeHashGrid(Self.Seed);
        DrawHexSphereMesh();
    }

    private void OnTreeExiting()
    {
        NodeReady = false;
        // 不小心忽视了事件的解绑，会在编辑器下"重载已保存场景"时出问题报错！
        // 【切记】所以这里需要在退出场景树时清理事件监听！！！
        _hexPlanetHudRepo.EditModeChanged -= OnEditorEditModeChanged;
    }

    private void OnProcessed(double delta)
    {
        if (!NodeReady) return;
        var floatDelta = (float)delta;
        _tileShaderService.UpdateData(floatDelta);

        Self.LastUpdated += floatDelta;
        if (Self.LastUpdated < 0.1f) return; // 每 0.1s 更新一次
        if (Mathf.Abs(Self.OldRadius - Self.Radius) > 0.001f
            || Self.OldDivisions != Self.Divisions
            || Self.OldChunkDivisions != Self.ChunkDivisions)
            DrawHexSphereMesh();
        if (!Engine.IsEditorHint())
        {
            UpdateSelectTileViewer();
            UpdateCivTerritory();
        }

        Self.LastUpdated = 0f; // 每一秒检查一次
    }

    private void OnRadiusChanged(float radius)
    {
        _celestialMotionManagerService.UpdateMoonMeshRadius(); // 卫星半径
        _celestialMotionManagerService.UpdateLunarDist(); // 卫星轨道半径
    }

    private void OnEditorEditModeChanged(bool editMode)
    {
        UpdateSelectTileViewer();
        if (editMode)
            _unitManagerRepo.Singleton!.PathFromTileId = 0; // 清除单位移动路径 UI
        else
        {
            // 游戏模式下永远不显示编辑预览网格
            _editPreviewChunkRepo.Singleton!.Hide();
            _selectTileViewerRepo.Singleton!.CleanEditingTile();
        }
    }

    private void UpdateSelectTileViewer()
    {
        var position = Self.GetTileCollisionPositionUnderCursor();
        _selectTileViewerService.Update(_unitManagerRepo.Singleton!.PathFromTileId, position);
    }

    private void DrawHexSphereMesh()
    {
        var time = Time.GetTicksMsec();
        GD.Print($"[===DrawHexSphereMesh===] radius {Self.Radius}, divisions {
            Self.Divisions}, start at: {time}");
        Self.OldRadius = Self.Radius;
        Self.OldDivisions = Self.Divisions;
        Self.OldChunkDivisions = Self.ChunkDivisions;
        Self.LastUpdated = 0f;
        ClearOldData();
        InitHexSphere();
        InitCivilization();
        RefreshAllTiles();
        Self.EmitNewPlanetGenerated(); // 触发事件
        GD.Print($"[===DrawHexSphereMesh===] total cost: {Time.GetTicksMsec() - time} ms");
    }

    private void InitHexSphere()
    {
        _chunkService.InitChunks();
        _tileService.InitTiles();
        _tileShaderService.Initialize();
        _tileSearchService.InitSearchData();
        _hexMapGeneratorService.GenerateMap();

        var time = Time.GetTicksMsec();
        _chunkLoaderService.InitChunkNodes();
        GD.Print($"InitChunkNodes cost: {Time.GetTicksMsec() - time} ms");
    }

    private void ClearOldData()
    {
        // 必须先清理单位，否则相关可见度事件会查询地块，放最后会空引用异常
        _unitManagerService.ClearAllUnits(); // unitManager 不是 [Tool]，在编辑器时会是 null
        _chunkRepo.Truncate();
        _tileRepo.Truncate();
        _pointRepo.Truncate();
        _faceRepo.Truncate();
        _civRepo.Truncate();
        _selectTileViewerService.ClearPath();
        // 清空分块
        _hexGridChunkRepo.ClearOldData();
        _chunkLoaderRepo.Singleton!.ClearOldData();
        _featurePreviewManagerRepo.Singleton!.ClearForData();
        _featureMeshManagerRepo.Singleton!.ClearOldData();
        _lodMeshCache.RemoveAllLodMeshes();
    }

    private void RefreshAllTiles()
    {
        foreach (var tile in _tileRepo.GetAll())
        {
            _tileSearchService.RefreshTileSearchData(tile.Id);
            _tileShaderService.RefreshTerrain(tile.Id);
            _tileShaderService.RefreshVisibility(tile.Id);
        }
    }

    private void InitCivilization()
    {
        // 在可见分块的陆地分块中随机
        var tiles = _chunkRepo.GetAll()
            .Where(c => c.Insight)
            .SelectMany(c => c.TileIds)
            .Select(id => _tileRepo.GetById(id)!)
            .Where(t => !t.Data.IsUnderwater)
            .ToList();
        if (tiles.Count == 0) return;
        for (var i = 0; i < 8; i++)
        {
            var idx = GD.RandRange(0, tiles.Count - 1);
            var tile = tiles[idx];
            var civ = _civRepo.Add(new Color(
                Mathf.Lerp(0.3f, 1f, GD.Randf()),
                Mathf.Lerp(0.3f, 1f, GD.Randf()),
                Mathf.Lerp(0.3f, 1f, GD.Randf())));
            UpdateTileCivId(tile, civ);
            tiles[idx] = tiles[^1];
            tiles.RemoveAt(tiles.Count - 1);
        }
    }

    private void UpdateCivTerritory()
    {
        foreach (var civ in _civRepo.GetAll())
        {
            var tile = _tileRepo.GetById(civ.TileIds[GD.RandRange(0, civ.TileIds.Count - 1)])!;
            var conquerTile = _tileRepo.GetNeighbors(tile).FirstOrDefault(n => !n.Data.IsUnderwater && n.CivId <= 0);
            if (conquerTile == null) continue;
            UpdateTileCivId(conquerTile, civ);
        }
    }

    private void UpdateTileCivId(Tile tile, Civ civ)
    {
        civ.TileIds.Add(tile.Id);
        tile.CivId = civ.Id;
        _tileShaderService.RefreshCiv(tile.Id);
    }
}