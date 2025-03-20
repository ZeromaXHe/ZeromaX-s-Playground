using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;

public interface ITileRepo : IRepository<Tile>
{
    Tile Add(int centerId, int chunkId, Vector3 unitCentroid, List<int> hexFaceIds, List<int> neighborCenterIds);
    Tile GetByCenterId(int centerId);
}