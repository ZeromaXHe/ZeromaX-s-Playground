using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Core;

public class HexChunk
{
    public int Id;
    public Vector3 Origin;
    public List<int> TileIds;
    public bool IsDirty;

    public delegate void OnChunkChangeHandler(HexTile tile);

    public event OnChunkChangeHandler OnChunkChange;

    [System.NonSerialized] public HexPlanetNode Planet;
    [System.NonSerialized] private List<HexTile> _tiles;

    public HexChunk(int id, HexPlanetNode planet, Vector3 origin)
    {
        TileIds = new List<int>();
        this.Origin = origin;
        this.Planet = planet;
        this.Id = id;
    }

    public void AddTile(int tileID)
    {
        TileIds.Add(tileID);
    }

    private void OnChunkTileChange(HexTile tile)
    {
        OnChunkChange?.Invoke(tile);
    }

    public List<HexTile> GetTiles()
    {
        if (_tiles == null)
        {
            _tiles = new List<HexTile>();
            for (int i = 0; i < TileIds.Count; i++)
            {
                _tiles.Add(Planet.GetTile(TileIds[i]));
            }
        }

        return _tiles;
    }

    public Mesh GetMesh()
    {
        List<HexTile> tiles = GetTiles();

        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> indices = new List<int>();
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].AppendToMesh(vertices, indices, colors);
        }

        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        foreach (var color in colors)
        {
            surfaceTool.SetColor(color);
        }

        foreach (var vertex in vertices)
        {
            surfaceTool.AddVertex(vertex);
        }

        foreach (var index in indices)
        {
            surfaceTool.AddIndex(index);
        }

        var material = new StandardMaterial3D();
        material.VertexColorUseAsAlbedo = true;
        surfaceTool.SetMaterial(material);
        // surfaceTool.RecalculateNormals();
        // surfaceTool.RecalculateBounds();
        surfaceTool.GenerateNormals();
        var mesh = surfaceTool.Commit();
        return mesh;
    }

    public HexTile GetClosestTileAngle(Vector3 input)
    {
        List<HexTile> tiles = GetTiles();

        HexTile ret = null;
        float closeness = -10000.0f;
        for (int i = 0; i < tiles.Count; i++)
        {
            float similarity = tiles[i].Center.Normalized().Dot(input.Normalized());
            if (similarity > closeness)
            {
                closeness = similarity;
                ret = tiles[i];
            }
        }

        return ret;
    }

    private Vector3 TransformPoint(Vector3 input, float height)
    {
        return input * (1 + (height / Planet.Radius));
    }

    public void MakeDirty()
    {
        IsDirty = true;
    }

    public void SetupEvents()
    {
        List<HexTile> tiles = GetTiles();
        foreach (HexTile tile in tiles)
        {
            tile.OnTileChange += OnChunkTileChange;
        }
    }
}