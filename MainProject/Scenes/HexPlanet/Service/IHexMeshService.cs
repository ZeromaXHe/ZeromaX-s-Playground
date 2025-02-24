using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IHexMeshService
{
    void Triangulate(float radius, int chunkId, IHexMesh terrain, IHexMesh rivers);
}