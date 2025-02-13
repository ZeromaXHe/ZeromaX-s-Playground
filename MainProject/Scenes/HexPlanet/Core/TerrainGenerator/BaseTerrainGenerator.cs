using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Core.TerrainGenerator;

public partial class BaseTerrainGenerator : Node3D
{
    public virtual HexTile CreateHexTile(int id, HexPlanetNode planet, Vector3 centerPosition, List<Vector3> verts) {
        return new HexTile(id, planet, centerPosition, verts);
    }

    public virtual void AfterTileCreation(HexTile newTile) { }
}