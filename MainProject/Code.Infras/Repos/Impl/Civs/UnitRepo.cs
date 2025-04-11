using Domains.Models.Entities.Civs;
using Domains.Repos.Civs;
using Infras.Base;

namespace Infras.Repos.Impl.Civs;

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