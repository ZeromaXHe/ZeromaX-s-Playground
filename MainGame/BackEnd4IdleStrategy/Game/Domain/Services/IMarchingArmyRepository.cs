using BackEnd4IdleStrategy.Common.Base;
using BackEnd4IdleStrategy.Game.Domain.Entities;

namespace BackEnd4IdleStrategy.Game.Domain.Services;

internal interface IMarchingArmyRepository: IRepository<MarchingArmy>
{
    MarchingArmy Init(int population, int playerId, int fromTileId, int toTileId);
    void Remove(int marchingArmyId);
}