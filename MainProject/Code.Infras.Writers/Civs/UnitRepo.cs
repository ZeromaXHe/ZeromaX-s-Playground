using Domains.Models.Entities.Civs;
using Infras.Writers.Abstractions.Civs;
using Infras.Writers.Base;

namespace Infras.Writers.Civs;

public class UnitRepo : Repository<Unit>, IUnitRepo
{
    protected override void AddHook(Unit entity)
    {
    }

    protected override void DeleteHook(Unit entity)
    {
    }

    protected override void TruncateHook()
    {
    }

    public Unit Add() => Add(id => new Unit(id));
}