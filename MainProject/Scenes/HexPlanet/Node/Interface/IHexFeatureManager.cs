using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;

public interface IHexFeatureManager
{
    IHexMesh Walls { get; }
    void AddTower(Vector3 left, Vector3 right);
    void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2);
    void AddSpecialFeature(Tile tile, Vector3 position);
    void AddFeature(Tile tile, Vector3 position);
}