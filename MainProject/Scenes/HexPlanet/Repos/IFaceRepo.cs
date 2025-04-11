using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;

public interface IFaceRepo : IRepository<Face>
{
    Face Add(bool chunky, Vector3[] triVertices);
    IEnumerable<Face> GetAllByChunky(bool chunky);
    List<Face> GetOrderedFaces(Point center);
}