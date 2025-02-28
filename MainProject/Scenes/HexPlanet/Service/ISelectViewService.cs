using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface ISelectViewService
{
    int SelectViewSize { get; set; }
    Mesh GenerateMeshForEditMode(Vector3 position, float radius);
    Mesh GenerateMeshForPlayMode(int pathFindingFromTileId, Vector3 position, float radius);
}