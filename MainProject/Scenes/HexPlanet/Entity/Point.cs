using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Point(Vector3 position, TileType type, int typeIdx, int id = -1): AEntity(id)
{
    public Vector3 Position { get; } = position;
    public TileType Type { get; } = type;
    public int TypeIdx { get; } = typeIdx; // 类型下的索引
    public List<int> FaceIds;
}