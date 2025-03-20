using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Impl;

public class FaceRepo : Repository<Face>, IFaceRepo
{
    public Face Add(bool chunky, Vector3[] triVertices) =>
        Add(id =>
        {
            var center = (triVertices[0] + triVertices[1] + triVertices[2]) / 3f;
            return new Face(chunky, center, id) { TriVertices = triVertices };
        });

    public IEnumerable<Face> GetAllByChunky(bool chunky) => GetAll().Where(x => x.Chunky == chunky);

    protected override void AddHook(Face entity)
    {
    }

    protected override void TruncateHook()
    {
    }
}