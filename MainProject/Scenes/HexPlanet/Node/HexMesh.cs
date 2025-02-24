using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexMesh : MeshInstance3D
{
    public void BuildMesh(float radius, int chunkId)
    {
        // 清理之前的碰撞体
        foreach (var child in GetChildren())
            child.QueueFree();
        Mesh = Context.GetBean<IHexMeshService>().BuildMesh(radius, chunkId);
        CreateTrimeshCollision();
    }
}