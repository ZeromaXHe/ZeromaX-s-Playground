using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos.Impl;

public class UnitRepo : Repository<Unit>, IUnitRepo
{
    protected override void AddHook(Unit entity)
    {
    }

    protected override void TruncateHook()
    {
    }

    public Unit Add() => Add(id => new Unit(id));
}