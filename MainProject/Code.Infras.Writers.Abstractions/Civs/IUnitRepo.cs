using Domains.Models.Entities.Civs;
using Infras.Writers.Abstractions.Bases;

namespace Infras.Writers.Abstractions.Civs;

public interface IUnitRepo: IRepository<Unit>
{
    Unit Add();
}