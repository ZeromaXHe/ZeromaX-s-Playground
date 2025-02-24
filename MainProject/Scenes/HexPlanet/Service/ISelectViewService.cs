using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface ISelectViewService
{
    int SelectViewSize { get; set; }
    Mesh GenerateMesh(Vector3 position, float radius);
}