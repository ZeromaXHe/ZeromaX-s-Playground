using System;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IPointService
{
    void Truncate();
    void SubdivideIcosahedronForTiles(int divisions);

    void SubdivideIcosahedron(int divisions, Action<Vector3, TileType, int> addPoint, Action<Vector3[]> addFace = null);
}