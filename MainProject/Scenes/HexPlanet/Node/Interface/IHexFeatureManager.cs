using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;

public interface IHexFeatureManager
{
    IHexMesh Walls { get; }
    void AddFeature(Tile tile, Vector3 position);
}