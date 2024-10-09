using BackEnd4IdleStrategy.Common.Base;
using BackEnd4IdleStrategy.Game.Domain.Entities;
using BackEnd4IdleStrategy.Game.Domain.Services;

namespace BackEnd4IdleStrategy.Game.Infrastructure.Repositories;

internal class MarchingArmyRepository : Repository<MarchingArmy>, IMarchingArmyRepository
{
    public MarchingArmy Init(int population, int playerId, int fromTileId, int toTileId)
    {
        return base.Init(id => new MarchingArmy(id, population, playerId, fromTileId, toTileId));
    }

    public new void Remove(int marchingArmyId)
    {
        base.Remove(marchingArmyId);
    }
}