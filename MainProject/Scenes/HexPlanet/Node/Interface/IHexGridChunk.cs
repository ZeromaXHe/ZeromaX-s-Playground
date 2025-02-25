namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;

public interface IHexGridChunk
{
    int Id { get; set; }
    HexMesh Terrain { get; set; }
    HexMesh Rivers { get; set; }
    HexMesh Roads { get; set; }
    HexMesh Water { get; set; }
    HexMesh WaterShore { get; set; }
    HexMesh Estuary { get; set; }
}