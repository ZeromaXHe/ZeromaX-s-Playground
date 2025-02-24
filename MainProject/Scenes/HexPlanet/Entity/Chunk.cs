using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Chunk(Vector3 pos, int id = -1): AEntity(id)
{
    public Vector3 Pos { get; } = pos;
    public List<int> TileIds { get; } = [];
}