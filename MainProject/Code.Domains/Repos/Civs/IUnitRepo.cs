using Domains.Bases;
using Domains.Models.Entities.Civs;

namespace Domains.Repos.Civs;

public interface IUnitRepo: IRepository<Unit>
{
    Unit Add();
}