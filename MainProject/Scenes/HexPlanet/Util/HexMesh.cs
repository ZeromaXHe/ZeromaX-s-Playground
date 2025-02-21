using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public class HexMesh
{
    private readonly SurfaceTool _surfaceTool = new();
    private int _vIdx;
    private static readonly Material DefaultMaterial = new StandardMaterial3D { VertexColorUseAsAlbedo = true };

    private float _radius;

    public Mesh BuildMesh(float radius)
    {
        _radius = radius;
        _vIdx = 0;
        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        _surfaceTool.SetSmoothGroup(uint.MaxValue);

        foreach (var tile in Tile.GetAll())
        {
            Triangulate(tile);
            BuildCliffFaces(tile);
        }

        _surfaceTool.GenerateNormals();
        _surfaceTool.SetMaterial(DefaultMaterial);
        return _surfaceTool.Commit();
    }

    private void BuildCliffFaces(Tile tile)
    {
        var scale = (_radius + tile.Height) / _radius;
        var tileCenter = tile.GetCenter(_radius);
        var lowerNeighbors = tile.GetNeighbors()
            .Where(t => t.Height < tile.Height);
        foreach (var lower in lowerNeighbors)
        {
            var commonPoints = tile.GetNeighborCommonCorners(lower, _radius);
            if (commonPoints == null)
                continue;
            var lowerScale = (_radius + lower.Height) / _radius;
            var v0 = commonPoints[0] * lowerScale;
            var v1 = commonPoints[1] * lowerScale;
            var v2 = commonPoints[0] * scale;
            var v3 = commonPoints[1] * scale;
            AddTriangle(Math3dUtil.SortVertices(tileCenter, v0, v1, v2));
            AddTriangle(Math3dUtil.SortVertices(tileCenter, v1, v2, v3));
        }
    }

    private void Triangulate(Tile tile)
    {
        var center = tile.GetCenter(_radius);
        var scale = (_radius + tile.Height) / _radius;
        var solidCorners = tile.GetCorners(_radius * scale, 1f/*HexMetrics.solidFactor*/).ToList();
        var cs = new[] { tile.Color, tile.Color, tile.Color };
        for (var i = 1; i < solidCorners.Count - 1; i++)
        {
            var vs = Math3dUtil.SortVertices(Vector3.Zero, solidCorners[0], solidCorners[i], solidCorners[i + 1]);
            AddTriangle(vs, cs);
        }

        // for (var i = 0; i < solidCorners.Count - 1; i++)
        // {
        //     var v1 = solidCorners[i] - center;
        //     var v2 = solidCorners[i + 1] - center;
        //     var bridge = HexMetrics.GetBridge(v1, v2);
        //     var v3 = v1 + bridge;
        //     var v4 = v2 + bridge;
        // }
    }

    private void AddTriangle(Vector3[] vs, Color[] cs = null)
    {
        for (var i = 0; i < 3; i++)
        {
            if (cs != null)
                _surfaceTool.SetColor(cs[i]);
            _surfaceTool.AddVertex(vs[i]);
        }
        _surfaceTool.AddIndex(_vIdx);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 2);
        _vIdx += 3;
    }

    private void AddQuad(Vector3[] vs, Color[] cs = null)
    {
        for (var i = 0; i < 4; i++)
        {
            if (cs != null)
                _surfaceTool.SetColor(cs[i]);
            _surfaceTool.AddVertex(vs[i]);
        }

        _surfaceTool.AddIndex(_vIdx);
        _surfaceTool.AddIndex(_vIdx + 2);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 2);
        _surfaceTool.AddIndex(_vIdx + 3);
        _vIdx += 4;
    }
}