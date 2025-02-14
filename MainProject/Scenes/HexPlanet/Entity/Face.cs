using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Face(Vector3 center, int id = -1)
{
    public Vector3 Center { get; } = center;
    public int Id { get; } = id;
}