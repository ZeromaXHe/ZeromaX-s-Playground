using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IWallMeshService
{
    void AddWall(IHexFeatureManager feature, EdgeVertices near, Tile nearTile, EdgeVertices far, Tile farTile,
        bool hasRiver, bool hasRoad);

    void AddWall(IHexFeatureManager feature, Vector3 c1, Tile tile1, Vector3 c2, Tile tile2, Vector3 c3, Tile tile3);
}