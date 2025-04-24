using Commons.Utils;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.Singletons.Planets;
using Domains.Services.Abstractions.PlanetGenerates;
using Domains.Services.Abstractions.Searches;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions.Planets;

namespace Domains.Services.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:18:24
public class SelectTileViewerService(
    ISelectTileViewerRepo selectTileViewerRepo,
    IHexPlanetHudRepo hexPlanetHudRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    IChunkRepo chunkRepo,
    ITileRepo tileRepo,
    ITileService tileService,
    ITileSearchService tileSearchService)
    : ISelectTileViewerService
{
    private ISelectTileViewer Self => selectTileViewerRepo.Singleton!;

    public void Update(int pathFromTileId, Vector3 position)
    {
        if (hexPlanetHudRepo.GetEditMode())
            UpdateInEditMode(position);
        else
            UpdateInPlayMode(pathFromTileId, position);
    }

    private void UpdateInPlayMode(int pathFromTileId, Vector3 position)
    {
        if (pathFromTileId == 0)
        {
            Self.Hide();
            return;
        }

        Self.Show();
        var mesh = GenerateMeshForPlayMode(pathFromTileId, position);
        if (mesh != null)
            Self.Mesh = mesh;
    }

    private void UpdateInEditMode(Vector3 position)
    {
        if (position != Vector3.Zero || Self.EditingTileId > 0)
        {
            // 更新选择地块框
            Self.Show();
            var mesh = GenerateMeshForEditMode(Self.EditingTileId, position);
            if (mesh != null)
                Self.Mesh = mesh;
        }
        else
        {
            // GD.Print("No tile under cursor, _selectTileViewer not visible");
            Self.Hide();
        }
    }

    // TODO: 领域服务最好没有状态，待重构；还有这里下面依赖并调用了其他平级服务，也需要重构
    private int? _hoverTileId;
    private int _selectedTileId;
    public void ClearPath() => tileSearchService.ClearPath();

    private Mesh? GenerateMeshForEditMode(int editingTileId, Vector3 position)
    {
        var hoverTileId = position == Vector3.Zero ? null : tileService.SearchNearestTileId(position);
        if (hoverTileId != null || editingTileId > 0)
        {
            // 编辑选取点和鼠标悬浮点都没变
            if (editingTileId == _selectedTileId && hoverTileId == _hoverTileId)
                return null;
            // GD.Print($"Generating New _selectTileViewer Mesh! {centerId}, position: {position}");
            _hoverTileId = hoverTileId;
            _selectedTileId = editingTileId;

            var surfaceTool = new SurfaceTool();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            surfaceTool.SetSmoothGroup(uint.MaxValue);
            var vi = 0;

            if (_selectedTileId > 0)
            {
                var selectedTile = tileRepo.GetById(_selectedTileId)!;
                vi += AddHexFrame(selectedTile, Colors.Aquamarine,
                    1.01f * (hexPlanetManagerRepo.Radius + hexPlanetManagerRepo.GetHeight(selectedTile)),
                    surfaceTool, vi); // 选择地块为蓝色框
            }

            if (_hoverTileId != null)
            {
                var hoverTile = tileRepo.GetById((int)_hoverTileId)!;

                var color = Colors.DarkGreen with { A = 0.8f };
                var tiles = tileRepo.GetTilesInDistance(hoverTile,
                    hexPlanetHudRepo.GetTileOverrider().BrushSize);
                var viewRadius = hexPlanetManagerRepo.Radius + hexPlanetManagerRepo.MaxHeight;
                foreach (var t in tiles)
                    vi += AddHexFrame(t, color, viewRadius, surfaceTool, vi);
            }

            return surfaceTool.Commit();
        }

        GD.PrintErr($"centerId not found and no editing Tile! position: {position}");
        return null;
    }

    public Mesh? GenerateMeshForPlayMode(int pathFindingFromTileId, Vector3 position)
    {
        if (position != Vector3.Zero)
        {
            // 有寻路目标时的情况
            var hoverTileId = tileService.SearchNearestTileId(position);
            if (hoverTileId != null)
            {
                if (pathFindingFromTileId == _selectedTileId && hoverTileId == _hoverTileId)
                    return null; // 寻路出发点和目标点都没变
                _selectedTileId = pathFindingFromTileId;
                _hoverTileId = hoverTileId;
                ClearPath();
                var fromTile = tileRepo.GetById(pathFindingFromTileId)!;
                var toTileId = (int)_hoverTileId;
                var toTile = tileRepo.GetById(toTileId)!;
                var surfaceTool = new SurfaceTool();
                surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
                surfaceTool.SetSmoothGroup(uint.MaxValue);
                var vi = 0;
                vi += AddHexFrame(fromTile, Colors.Blue,
                    1.01f * (hexPlanetManagerRepo.Radius + hexPlanetManagerRepo.GetHeight(fromTile)),
                    surfaceTool, vi); // 出发点为蓝色框
                vi += AddHexFrame(toTile, Colors.Red,
                    1.01f * (hexPlanetManagerRepo.Radius + hexPlanetManagerRepo.GetHeight(toTile)),
                    surfaceTool, vi); // 目标点为红色框
                var tiles = tileSearchService.FindPath(fromTile, toTile);
                if (tiles.Count > 0)
                {
                    var cost = 0;
                    var preTile = fromTile;
                    for (var i = 1; i < tiles.Count; i++)
                    {
                        var nextTile = tiles[i];
                        if (i != tiles.Count - 1)
                            vi += AddHexFrame(nextTile, Colors.White,
                                1.01f * (hexPlanetManagerRepo.Radius + hexPlanetManagerRepo.GetHeight(nextTile)),
                                surfaceTool, vi); // 路径点为白色框
                        cost += tileSearchService.GetMoveCost(preTile, nextTile);
                        chunkRepo.RefreshTileLabel(nextTile, cost.ToString());
                        preTile = nextTile;
                    }
                }

                return surfaceTool.Commit();
            }

            GD.PrintErr($"centerId not found! position: {position}");
            return null;
        }

        // 没有寻路目标时的情况
        if (pathFindingFromTileId == _selectedTileId) return null; // 寻路出发点没变
        _selectedTileId = pathFindingFromTileId;
        ClearPath();
        var tile = tileRepo.GetById(pathFindingFromTileId)!;
        var surfaceTool2 = new SurfaceTool();
        surfaceTool2.Begin(Mesh.PrimitiveType.Triangles);
        surfaceTool2.SetSmoothGroup(uint.MaxValue);
        var viewRadius2 = hexPlanetManagerRepo.MaxHeight;
        AddHexFrame(tile, Colors.Blue, viewRadius2, surfaceTool2, 0);
        return surfaceTool2.Commit();
    }

    // vi 对应此前 surfaceTool.AddVertex 的次数
    private int AddHexFrame(Tile tile, Color color, float viewRadius, SurfaceTool surfaceTool, int vi)
    {
        surfaceTool.SetColor(color);
        var centroid = tile.GetCentroid(viewRadius);
        var points = tile.GetCorners(viewRadius).ToList();
        foreach (var p in points)
        {
            surfaceTool.AddVertex(p);
            surfaceTool.AddVertex(centroid.Lerp(p, 0.85f));
        }

        for (var i = 0; i < points.Count; i++)
        {
            var nextIdx = (i + 1) % points.Count;
            if (Math3dUtil.IsRightVSeq(Vector3.Zero, centroid, points[i], points[nextIdx]))
            {
                surfaceTool.AddIndex(vi + 2 * i + 1);
                surfaceTool.AddIndex(vi + 2 * i);
                surfaceTool.AddIndex(vi + 2 * nextIdx);
                surfaceTool.AddIndex(vi + 2 * nextIdx);
                surfaceTool.AddIndex(vi + 2 * nextIdx + 1);
                surfaceTool.AddIndex(vi + 2 * i + 1);
            }
            else
            {
                surfaceTool.AddIndex(vi + 2 * nextIdx + 1);
                surfaceTool.AddIndex(vi + 2 * nextIdx);
                surfaceTool.AddIndex(vi + 2 * i);
                surfaceTool.AddIndex(vi + 2 * i);
                surfaceTool.AddIndex(vi + 2 * i + 1);
                surfaceTool.AddIndex(vi + 2 * nextIdx + 1);
            }
        }

        return 2 * points.Count;
    }
}