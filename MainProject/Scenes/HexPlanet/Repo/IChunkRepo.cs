using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

public interface IChunkRepo: IRepository<Chunk>
{
    Chunk Add(Vector3 pos);
    Chunk GetByPos(Vector3 position);
}