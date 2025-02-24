using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexGridChunk : Node3D
{
    [Export] private HexMesh _terrain;
    [Export] private HexMesh _rivers;
    private int _id;
    private float _radius;

    public void Init(int id, float radius)
    {
        _id = id;
        _radius = radius;
        Refresh();
    }

    public override void _Process(double delta)
    {
        if (_id > 0)
        {
            var time = Time.GetTicksMsec();
            _terrain.Clear();
            _rivers.Clear();
            Context.GetBean<IHexMeshService>().Triangulate(_radius, _id, _terrain, _rivers);
            _terrain.Apply();
            _rivers.Apply();
            GD.Print($"Chunk {_id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void Refresh() => SetProcess(true);
}