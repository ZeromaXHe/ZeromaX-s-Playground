using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexGridChunk : Node3D, IHexGridChunk
{
    [Export] private HexMesh _terrain;
    public IHexMesh Terrain => _terrain;
    [Export] private HexMesh _rivers;
    public IHexMesh Rivers => _rivers;
    [Export] private HexMesh _roads;
    public IHexMesh Roads => _roads;
    [Export] private HexMesh _water;
    public IHexMesh Water => _water;
    [Export] private HexMesh _waterShore;
    public IHexMesh WaterShore => _waterShore;
    [Export] private HexMesh _estuary;
    public IHexMesh Estuary => _estuary;
    [Export] private HexFeatureManager _features;
    public IHexFeatureManager Features => _features;

    public int Id { get; set; }

    private float _radius;

    public void Init(int id, float radius)
    {
        Id = id;
        _radius = radius;
        Refresh();
    }

    public override void _Process(double delta)
    {
        if (Id > 0)
        {
            var time = Time.GetTicksMsec();
            _terrain.Clear();
            _rivers.Clear();
            _roads.Clear();
            _water.Clear();
            _waterShore.Clear();
            _estuary.Clear();
            _features.Clear();
            Context.GetBean<IHexMeshService>().Triangulate(_radius, this);
            _terrain.Apply();
            _rivers.Apply();
            _roads.Apply();
            _water.Apply();
            _waterShore.Apply();
            _estuary.Apply();
            _features.Apply();
            GD.Print($"Chunk {Id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void Refresh() => SetProcess(true);
}