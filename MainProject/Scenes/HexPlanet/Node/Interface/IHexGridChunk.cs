namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;

public interface IHexGridChunk
{
    int Id { get; }
    IHexMesh Terrain { get; }
    IHexMesh Rivers { get; }
    IHexMesh Roads { get; }
    IHexMesh Water { get; }
    IHexMesh WaterShore { get; }
    IHexMesh Estuary { get; }
    IHexFeatureManager Features { get; }
}