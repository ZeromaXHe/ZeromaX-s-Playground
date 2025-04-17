using Apps.Contexts;
using Apps.Nodes;
using Apps.Nodes.Planets;
using Domains.Models.Entities.Civs;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Caches;
using Domains.Models.Singletons.Planets;
using Domains.Repos.Civs;
using Domains.Repos.PlanetGenerates;
using Domains.Services.Navigations;
using Domains.Services.PlanetGenerates;
using Domains.Services.Shaders;
using Domains.Services.Uis;
using Godot;

namespace Apps.Applications.Planets.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-14 16:00:57
public class HexPlanetManagerApp(
    ILodMeshCache lodMeshCache,
    INoiseConfig noiseConfig,
    IChunkRepo chunkRepo,
    ITileRepo tileRepo,
    IPointRepo pointRepo,
    IFaceRepo faceRepo,
    ICivRepo civRepo,
    ITileSearchService tileSearchService,
    ITileShaderService tileShaderService,
    ISelectViewService selectViewService,
    IChunkService chunkService,
    ITileService tileService,
    IEditorService editorService) : IHexPlanetManagerApp
{
    #region 上下文节点

    private IHexPlanetManager? _hexPlanetManager;
    private IChunkManager? _chunkManager;
    private IUnitManager? _unitManager; // 单位管理节点
    private ISelectTileViewer? _selectTileViewer;
    private IEditPreviewChunk? _editPreviewChunk;
    private IHexMapGenerator? _hexMapGenerator;

    public bool NodeReady { get; set; }
    
    public void OnReady()
    {
        NodeReady = true;
        _hexPlanetManager = NodeContext.Instance.GetSingleton<IHexPlanetManager>()!;
        _chunkManager = NodeContext.Instance.GetSingleton<IChunkManager>()!;
        _hexMapGenerator = NodeContext.Instance.GetSingleton<IHexMapGenerator>()!;
        if (!Engine.IsEditorHint())
        {
            _unitManager = NodeContext.Instance.GetSingleton<IUnitManager>()!;
            _selectTileViewer = NodeContext.Instance.GetSingleton<ISelectTileViewer>()!;
            _editPreviewChunk = NodeContext.Instance.GetSingleton<IEditPreviewChunk>()!;
        }

        editorService.EditModeChanged += OnEditorEditModeChanged;
        // 在 _ready = true 后初始化相关数据。手动赋值调用 setter 一下
        _hexPlanetManager.Radius = _hexPlanetManager.Radius;
        _hexPlanetManager.Divisions = _hexPlanetManager.Divisions;

        noiseConfig.NoiseSource = _hexPlanetManager.NoiseSource!.GetImage();
        noiseConfig.InitializeHashGrid(_hexPlanetManager.Seed);
        DrawHexSphereMesh();
    }

    private void OnEditorEditModeChanged(bool editMode)
    {
        UpdateSelectTileViewer();
        if (editMode)
            _unitManager!.PathFromTileId = 0; // 清除单位移动路径 UI
        else
        {
            // 游戏模式下永远不显示编辑预览网格
            _editPreviewChunk!.Hide();
            _selectTileViewer!.CleanEditingTile();
        }
    }

    public void OnProcess(double delta)
    {
        if (!NodeReady) return;
        var floatDelta = (float)delta;
        tileShaderService.UpdateData(floatDelta);

        _hexPlanetManager!.LastUpdated += floatDelta;
        if (_hexPlanetManager.LastUpdated < 0.1f) return; // 每 0.1s 更新一次
        if (Mathf.Abs(_hexPlanetManager.OldRadius - _hexPlanetManager.Radius) > 0.001f
            || _hexPlanetManager.OldDivisions != _hexPlanetManager.Divisions
            || _hexPlanetManager.OldChunkDivisions != _hexPlanetManager.ChunkDivisions)
            DrawHexSphereMesh();
        if (!Engine.IsEditorHint())
        {
            UpdateSelectTileViewer();
            UpdateCivTerritory();
        }

        _hexPlanetManager.LastUpdated = 0f; // 每一秒检查一次
    }

    private void UpdateSelectTileViewer()
    {
        var position = _hexPlanetManager!.GetTileCollisionPositionUnderCursor();
        _selectTileViewer!.Update(_unitManager!.PathFromTileId, position);
    }

    public void OnExitTree()
    {
        NodeReady = false;
        editorService.EditModeChanged -= OnEditorEditModeChanged;

        _hexPlanetManager = null;
        _chunkManager = null;
        _unitManager = null;
        _selectTileViewer = null;
        _editPreviewChunk = null;
        _hexMapGenerator = null;
    }

    #endregion

    public Tile? GetTileUnderCursor()
    {
        var pos = _hexPlanetManager!.GetTileCollisionPositionUnderCursor();
        if (pos == Vector3.Zero) return null;
        var tileId = tileService.SearchNearestTileId(pos);
        return tileId == null ? null : tileRepo.GetById((int)tileId);
    }

    private void DrawHexSphereMesh()
    {
        var time = Time.GetTicksMsec();
        GD.Print($"[===DrawHexSphereMesh===] radius {_hexPlanetManager!.Radius}, divisions {
            _hexPlanetManager.Divisions}, start at: {time}");
        _hexPlanetManager.OldRadius = _hexPlanetManager.Radius;
        _hexPlanetManager.OldDivisions = _hexPlanetManager.Divisions;
        _hexPlanetManager.OldChunkDivisions = _hexPlanetManager.ChunkDivisions;
        _hexPlanetManager.LastUpdated = 0f;
        ClearOldData();
        InitHexSphere();
        InitCivilization();
        RefreshAllTiles();
        _hexPlanetManager.EmitNewPlanetGenerated(); // 触发事件
        GD.Print($"[===DrawHexSphereMesh===] total cost: {Time.GetTicksMsec() - time} ms");
    }

    private void InitHexSphere()
    {
        chunkService.InitChunks();
        tileService.InitTiles();
        tileShaderService.Initialize();
        tileSearchService.InitSearchData();
        _hexMapGenerator!.GenerateMap();
        _chunkManager!.InitChunkNodes();
    }

    public void ClearOldData()
    {
        // 必须先清理单位，否则相关可见度事件会查询地块，放最后会空引用异常
        _unitManager?.ClearAllUnits(); // unitManager 不是 [Tool]，在编辑器时会是 null
        chunkRepo.Truncate();
        tileRepo.Truncate();
        pointRepo.Truncate();
        faceRepo.Truncate();
        civRepo.Truncate();
        selectViewService.ClearPath();
        _chunkManager!.ClearOldData();
        lodMeshCache.RemoveAllLodMeshes();
    }

    public void RefreshAllTiles()
    {
        foreach (var tile in tileRepo.GetAll())
        {
            tileSearchService.RefreshTileSearchData(tile.Id);
            tileShaderService.RefreshTerrain(tile.Id);
            tileShaderService.RefreshVisibility(tile.Id);
        }
    }

    public void InitCivilization()
    {
        // 在可见分块的陆地分块中随机
        var tiles = chunkRepo.GetAll()
            .Where(c => c.Insight)
            .SelectMany(c => c.TileIds)
            .Select(id => tileRepo.GetById(id)!)
            .Where(t => !t.Data.IsUnderwater)
            .ToList();
        for (var i = 0; i < 8; i++)
        {
            var idx = GD.RandRange(0, tiles.Count - 1);
            var tile = tiles[idx];
            var civ = civRepo.Add(new Color(
                Mathf.Lerp(0.3f, 1f, GD.Randf()),
                Mathf.Lerp(0.3f, 1f, GD.Randf()),
                Mathf.Lerp(0.3f, 1f, GD.Randf())));
            UpdateTileCivId(tile, civ);
            tiles[idx] = tiles[^1];
            tiles.RemoveAt(tiles.Count - 1);
        }
    }

    public void UpdateCivTerritory()
    {
        foreach (var civ in civRepo.GetAll())
        {
            var tile = tileRepo.GetById(civ.TileIds[GD.RandRange(0, civ.TileIds.Count - 1)])!;
            var conquerTile = tileRepo.GetNeighbors(tile).FirstOrDefault(n => !n.Data.IsUnderwater && n.CivId <= 0);
            if (conquerTile == null) continue;
            UpdateTileCivId(conquerTile, civ);
        }
    }

    private void UpdateTileCivId(Tile tile, Civ civ)
    {
        civ.TileIds.Add(tile.Id);
        tile.CivId = civ.Id;
        tileShaderService.RefreshCiv(tile.Id);
    }

    public bool UpdateUiInEditMode()
    {
        if (!editorService.TileOverrider.EditMode) return false;
        // 编辑模式下更新预览网格
        UpdateEditPreviewChunk();
        if (Input.IsActionJustPressed("destroy_unit"))
        {
            DestroyUnit();
            return true;
        }

        if (Input.IsActionJustPressed("create_unit"))
        {
            CreateUnit();
            return true;
        }

        return false;
    }

    private void UpdateEditPreviewChunk()
    {
        var tile = GetTileUnderCursor();
        // 更新地块预览
        _editPreviewChunk!.Update(tile);
    }

    private void CreateUnit()
    {
        var tile = GetTileUnderCursor();
        if (tile == null || tile.UnitId > 0)
        {
            GD.Print($"CreateUnit failed: tile {tile}, unitId: {tile?.UnitId}");
            return;
        }

        GD.Print($"CreateUnit at tile {tile.Id}");
        _unitManager!.AddUnit(tile.Id, GD.Randf() * Mathf.Tau);
    }

    private void DestroyUnit()
    {
        var tile = GetTileUnderCursor();
        if (tile is not { UnitId: > 0 })
            return;
        _unitManager!.RemoveUnit(tile.UnitId);
    }
}