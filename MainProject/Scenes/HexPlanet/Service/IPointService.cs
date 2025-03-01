namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IPointService
{
    void Truncate();
    void SubdivideIcosahedron(int divisions);
}