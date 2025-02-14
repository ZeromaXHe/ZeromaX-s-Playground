using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Point(Vector3 position, int id = -1)
{
    public Vector3 Position { get; } = position;
    public int Id { get; } = id;
}