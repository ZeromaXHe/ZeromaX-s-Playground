using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class EditPreviewChunk : Node3D, IChunk
{
    public EditPreviewChunk() =>
        _chunkTriangulation = new ChunkTriangulation(this);

    [Export] public HexMesh Terrain { get; set; }
    [Export] public HexMesh Rivers { get; set; }
    [Export] public HexMesh Roads { get; set; }
    [Export] public HexMesh Water { get; set; }
    [Export] public HexMesh WaterShore { get; set; }
    [Export] public HexMesh Estuary { get; set; }
    [Export] public HexFeatureManager Features { get; set; }

    private readonly ChunkTriangulation _chunkTriangulation;
    public HexTileDataOverrider TileDataOverrider { get; set; } = new();

    public override void _Process(double delta)
    {
        if (TileDataOverrider.OverrideTiles.Count > 0)
        {
            // var time = Time.GetTicksMsec();
            Terrain.Clear();
            Rivers.Clear();
            Roads.Clear();
            Water.Clear();
            WaterShore.Clear();
            Estuary.Clear();
            Features.Clear();
            foreach (var tile in TileDataOverrider.OverrideTiles)
                _chunkTriangulation.Triangulate(tile);
            Terrain.Apply();
            Rivers.Apply();
            Roads.Apply();
            Water.Apply();
            WaterShore.Apply();
            Estuary.Apply();
            Features.Apply();
            // GD.Print($"EditPreviewChunk BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void Refresh(HexTileDataOverrider tileDataOverrider, IEnumerable<Tile> tiles)
    {
        TileDataOverrider = tileDataOverrider with { OverrideTiles = tiles.ToHashSet() };
        SetProcess(true);
    }
}