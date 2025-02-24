using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexGridChunk : Node3D
{
    private HexMesh _hexMesh;
    private int _id;
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
        if (_id > 0)
        {
            var time = Time.GetTicksMsec();
            _hexMesh.BuildMesh(_radius, _id);
            GD.Print($"Chunk {_id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void BuildMesh() => SetProcess(true);
}