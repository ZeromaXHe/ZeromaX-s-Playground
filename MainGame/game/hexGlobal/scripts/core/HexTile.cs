using System.Collections.Generic;
using Godot;

namespace HexGlobalGame.game.hexGlobal.scripts.core;

public class HexTile
{
    public delegate void OnTileChangeHandler(HexTile tile);

    public event OnTileChangeHandler OnTileChange;

    public int Id;
    public List<int> NeighborIDs;
    public int ChunkId;
    public Vector3 Center;
    public List<Vector3> Vertices;
    public float Height;
    [System.NonSerialized] public HexPlanet Planet;
    public Color Color;

    [System.NonSerialized] private List<HexTile> _neighbors;

    public HexTile(int id, HexPlanet planet, Vector3 center, List<Vector3> verts)
    {
        this.Id = id;
        this.Planet = planet;
        this.Center = center;
        this.Vertices = verts;
        this.Height = 0.0f;

        NeighborIDs = new List<int>();
    }

    public virtual void AppendToMesh(List<Vector3> meshVerts, List<int> meshIndices, List<Color> meshColors)
    {
        // Generate hexagon base
        // 生成六边形基础
        var baseIndex = meshVerts.Count;
        meshVerts.Add(TransformPoint(Center, Height));
        meshColors.Add(Color);
        for (var j = 0; j < Vertices.Count; j++)
        {
            meshVerts.Add(TransformPoint(Vertices[j], Height));
            meshColors.Add(Color);

            meshIndices.Add(baseIndex);
            meshIndices.Add(baseIndex + ((j + 1) % Vertices.Count) + 1);
            meshIndices.Add(baseIndex + j + 1);
        }

        // Generate walls if needed
        var neighbors = GetNeighbors();
        for (var j = 0; j < neighbors.Count; j++)
        {
            var thisHeight = Height;
            var otherHeight = neighbors[j].Height;

            if (!(otherHeight < thisHeight)) continue;

            baseIndex = meshVerts.Count;
            // Add barrier
            meshVerts.Add(TransformPoint(Vertices[(j + 1) % Vertices.Count], otherHeight));
            meshVerts.Add(TransformPoint(Vertices[j], otherHeight));
            meshVerts.Add(TransformPoint(Vertices[(j + 1) % Vertices.Count], thisHeight));
            meshVerts.Add(TransformPoint(Vertices[j], thisHeight));

            meshColors.Add(Color);
            meshColors.Add(Color);
            meshColors.Add(Color);
            meshColors.Add(Color);

            meshIndices.Add(baseIndex);
            meshIndices.Add(baseIndex + 1);
            meshIndices.Add(baseIndex + 2);

            meshIndices.Add(baseIndex + 2);
            meshIndices.Add(baseIndex + 1);
            meshIndices.Add(baseIndex + 3);
        }
    }


    private Vector3 TransformPoint(Vector3 input, float height)
    {
        return input * (1 + (height / Planet.Radius));
    }

    public void SetChunk(int chunkId)
    {
        this.ChunkId = chunkId;
    }

    public void AddNeighbors(List<HexTile> nbrs)
    {
        foreach (var nbr in nbrs)
        {
            NeighborIDs.Add(nbr.Id);
        }
    }

    public void SetHeight(float newHeight)
    {
        Height = newHeight;
        TriggerMeshRecompute();
    }

    public List<HexTile> GetNeighbors()
    {
        if (_neighbors != null) return _neighbors;

        _neighbors = new List<HexTile>();
        foreach (var nid in NeighborIDs)
        {
            _neighbors.Add(Planet.GetTile(nid));
        }

        return _neighbors;
    }

    protected void TriggerMeshRecompute()
    {
        OnTileChange?.Invoke(this);
    }
}