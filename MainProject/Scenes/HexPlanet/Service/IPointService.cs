using System;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IPointService
{
    void Truncate();
    void SubdivideIcosahedronForTiles(int divisions);
    void SubdivideIcosahedron(int divisions, Action<Vector3, SphereAxial> addPoint, Action<Vector3[]> addFace = null);
}