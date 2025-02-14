using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Tile(
    int centerId,
    List<int> hexFaceIds,
    List<int> neighborCenterIds,
    float radius,
    float size,
    int id = -1)
{
    public int Id { get; } = id;
    public int CenterId { get; } = centerId;
    public List<int> HexFaceIds { get; } = hexFaceIds;
    public List<int> NeighborCenterIds { get; } = neighborCenterIds;
    public float Radius { get; } = radius;
    public float Size { get; } = size;
}