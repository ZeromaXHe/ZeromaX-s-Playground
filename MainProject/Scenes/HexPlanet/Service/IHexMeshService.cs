using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IHexMeshService
{
    Mesh BuildMesh(float radius, int chunkId);
}