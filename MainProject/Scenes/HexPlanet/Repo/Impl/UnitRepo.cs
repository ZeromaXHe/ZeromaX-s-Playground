using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo.Impl;

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