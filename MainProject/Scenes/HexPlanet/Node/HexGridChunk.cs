using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexGridChunk : Node3D
{
    private HexMesh _hexMesh;
    private int _id = -1;
    private float _radius;

    public override void _Ready()
    {
        _hexMesh = GetNode<HexMesh>("%HexMesh");
    }

    public void Init(int id, float radius)
    {
        _id = id;
        _radius = radius;
        BuildMesh();
    }

    public override void _Process(double delta)
    {
        if (_id > -1)
        {
            var time = Time.GetTicksMsec();
            var tileIds = Chunk.GetById(_id).TileIds;
            _hexMesh.BuildMesh(_radius, tileIds.Select(Tile.GetById));
            GD.Print($"Chunk {_id} BuildMesh with {tileIds.Count} tiles cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void BuildMesh() => SetProcess(true);
}