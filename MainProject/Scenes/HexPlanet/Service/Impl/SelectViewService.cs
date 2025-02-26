using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class SelectViewService(ITileService tileService) : ISelectViewService
{
    private int? _selectTileCenterId;

    public int SelectViewSize { get; set; }

    public Mesh GenerateMesh(Vector3 position, float radius)
    {
        var centerId = tileService.SearchNearestTileId(position.Normalized());
        if (centerId != null)
        {
            if (centerId == _selectTileCenterId)
            {
                // GD.Print($"Same tile! centerId: {centerId}, position: {position}");
                return null;
            }

            // GD.Print($"Generating New _selectTileViewer Mesh! {centerId}, position: {position}");
            _selectTileCenterId = centerId;
            var tile = tileService.GetByCenterId((int)_selectTileCenterId);
            var surfaceTool = new SurfaceTool();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            surfaceTool.SetSmoothGroup(uint.MaxValue);
            var color = Colors.DarkGreen;
            color.A = 0.8f;
            surfaceTool.SetColor(color);
            var tiles = tileService.GetTilesInDistance(tile, SelectViewSize);
            var vi = 0;
            var viewRadius = radius * (1f + HexMetrics.MaxHeightRadiusRatio);
            foreach (var t in tiles)
            {
                var centroid = t.GetCentroid(viewRadius);
                var points = tileService.GetCorners(t, viewRadius).ToList();
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
                vi += 2 * points.Count;
            }

            return surfaceTool.Commit();
        }

        GD.PrintErr($"centerId not found! position: {position}");
        return null;
    }
}