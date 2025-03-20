using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;

public interface IChunkRepo : IRepository<Chunk>
{
    Chunk Add(int centerId, Vector3 pos, List<int> hexFaceIds, List<int> neighborCenterIds);
    Chunk GetByCenterId(int centerId);
}