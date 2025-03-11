using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class SelectViewService(ITileService tileService, ITileSearchService tileSearchService) : ISelectViewService
{
    private int? _hoverTileId;
    private int _selectedTileId;

    public void ClearPath() => tileSearchService.ClearPath();

    public int SelectViewSize { get; set; }

    public Mesh GenerateMeshForEditMode(int editingTileId, Vector3 position)
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
                var selectedTile = tileService.GetById(_selectedTileId);
                vi += AddHexFrame(selectedTile, Colors.Aquamarine,
                    1.01f * (HexMetrics.Radius + tileService.GetHeight(selectedTile)),
                    surfaceTool, vi); // 选择地块为蓝色框
            }

            if (hoverTileId != null)
            {
                var hoverTile = tileService.GetById((int)_hoverTileId);

                var color = Colors.DarkGreen with { A = 0.8f };
                var tiles = tileService.GetTilesInDistance(hoverTile, SelectViewSize);
                var viewRadius = HexMetrics.Radius + HexMetrics.MaxHeight;
                foreach (var t in tiles)
                    vi += AddHexFrame(t, color, viewRadius, surfaceTool, vi);
            }

            return surfaceTool.Commit();
        }

        GD.PrintErr($"centerId not found and no editing Tile! position: {position}");
        return null;
    }

    public Mesh GenerateMeshForPlayMode(int pathFindingFromTileId, Vector3 position)
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
                var fromTile = tileService.GetById(pathFindingFromTileId);
                var toTileId = (int)_hoverTileId;
                var toTile = tileService.GetById(toTileId);
                var surfaceTool = new SurfaceTool();
                surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
                surfaceTool.SetSmoothGroup(uint.MaxValue);
                var vi = 0;
                vi += AddHexFrame(fromTile, Colors.Blue,
                    1.01f * (HexMetrics.Radius + tileService.GetHeight(fromTile)),
                    surfaceTool, vi); // 出发点为蓝色框
                vi += AddHexFrame(toTile, Colors.Red,
                    1.01f * (HexMetrics.Radius + tileService.GetHeight(toTile)),
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
                                1.01f * (HexMetrics.Radius + tileService.GetHeight(nextTile)),
                                surfaceTool, vi); // 路径点为白色框
                        cost += tileSearchService.GetMoveCost(preTile, nextTile);
                        tileService.UpdateTileLabel(nextTile, cost.ToString());
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
        var tile = tileService.GetById(pathFindingFromTileId);
        var surfaceTool2 = new SurfaceTool();
        surfaceTool2.Begin(Mesh.PrimitiveType.Triangles);
        surfaceTool2.SetSmoothGroup(uint.MaxValue);
        var viewRadius2 = HexMetrics.MaxHeight;
        AddHexFrame(tile, Colors.Blue, viewRadius2, surfaceTool2, 0);
        return surfaceTool2.Commit();
    }

    // vi 对应此前 surfaceTool.AddVertex 的次数
    private int AddHexFrame(Tile tile, Color color, float viewRadius, SurfaceTool surfaceTool, int vi)
    {
        surfaceTool.SetColor(color);
        var centroid = tile.GetCentroid(viewRadius);
        var points = tileService.GetCorners(tile, viewRadius).ToList();
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