using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

public interface ITileRepo : IRepository<Tile>
{
    Tile Add(int centerId, int chunkId, Vector3 unitCentroid, List<int> hexFaceIds, List<int> neighborCenterIds);
    Tile GetByCenterId(int centerId);
}