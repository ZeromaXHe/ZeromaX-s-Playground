using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface ISelectViewService
{
    void ClearPath();
    int SelectViewSize { get; set; }
    Mesh GenerateMeshForEditMode(int editingTileId, Vector3 position);
    Mesh GenerateMeshForPlayMode(int pathFindingFromTileId, Vector3 position);
}