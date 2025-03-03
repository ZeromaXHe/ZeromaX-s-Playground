using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo.Impl;

public class FaceRepo : Repository<Face>, IFaceRepo
{
    public Face Add(Vector3[] triVertices) =>
        Add(id =>
        {
            var center = (triVertices[0] + triVertices[1] + triVertices[2]) / 3f;
            return new Face(center, id) { TriVertices = triVertices };
        });

    protected override void AddHook(Face entity)
    {
    }

    protected override void TruncateHook()
    {
    }
}