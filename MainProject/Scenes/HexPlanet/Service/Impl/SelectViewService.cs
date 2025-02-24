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
            var tiles = tileService.GetTilesInDistance(tile, SelectViewSize);
            var vi = 0;
            foreach (var t in tiles)
            {
                var points = tileService.GetCorners(t, radius * (1f + HexMetrics.MaxHeightRadiusRatio)).ToList();
                foreach (var p in points)
                    surfaceTool.AddVertex(p);
                for (var i = 1; i < points.Count - 1; i++)
                    if (Math3dUtil.IsRightVSeq(Vector3.Zero, points[0], points[i], points[i + 1]))
                    {
                        surfaceTool.AddIndex(vi);
                        surfaceTool.AddIndex(vi + i);
                        surfaceTool.AddIndex(vi + i + 1);
                    }
                    else
                    {
                        surfaceTool.AddIndex(vi + 0);
                        surfaceTool.AddIndex(vi + i + 1);
                        surfaceTool.AddIndex(vi + i);
                    }

                vi += points.Count;
            }

            return surfaceTool.Commit();
        }

        GD.PrintErr($"centerId not found! position: {position}");
        return null;
    }
}