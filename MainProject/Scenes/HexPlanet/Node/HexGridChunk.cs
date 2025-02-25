using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexGridChunk : Node3D, IHexGridChunk
{
    [Export] public HexMesh Terrain { get; set; }
    [Export] public HexMesh Rivers { get; set; }
    [Export] public HexMesh Roads { get; set; }
    [Export] public HexMesh Water { get; set; }
    [Export] public HexMesh WaterShore { get; set; }
    [Export] public HexMesh Estuary { get; set; }
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
            Terrain.Clear();
            Rivers.Clear();
            Roads.Clear();
            Water.Clear();
            WaterShore.Clear();
            Estuary.Clear();
            Context.GetBean<IHexMeshService>().Triangulate(_radius, this);
            Terrain.Apply();
            Rivers.Apply();
            Roads.Apply();
            Water.Apply();
            WaterShore.Apply();
            Estuary.Apply();
            GD.Print($"Chunk {Id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void Refresh() => SetProcess(true);
}