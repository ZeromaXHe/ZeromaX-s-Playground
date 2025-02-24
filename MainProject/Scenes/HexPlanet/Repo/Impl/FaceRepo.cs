using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo.Impl;

public class FaceRepo : Repository<Face>, IFaceRepo
{
    public Face Add(Point p0, Point p1, Point p2) =>
        Add(id =>
        {
            var center = (p0.Position + p1.Position + p2.Position) / 3f;
            List<int> pointIds = Math3dUtil.IsRightVSeq(Vector3.Zero, p0.Position, p1.Position, p2.Position)
                ? [p0.Id, p1.Id, p2.Id]
                : [p0.Id, p2.Id, p1.Id];
            return new Face(center, id) { PointIds = pointIds };
        });

    protected override void AddHook(Face entity)
    {
    }

    protected override void TruncateHook()
    {
    }
}