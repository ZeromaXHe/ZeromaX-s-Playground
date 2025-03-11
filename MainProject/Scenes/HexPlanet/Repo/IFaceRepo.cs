using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;

public interface IFaceRepo : IRepository<Face>
{
    Face Add(bool chunky, Vector3[] triVertices);
    IEnumerable<Face> GetAllByChunky(bool chunky);
}