using BackEnd4IdleStrategy.Common.Base;
using BackEnd4IdleStrategy.Game.Domain.Entities;
using BackEnd4IdleStrategy.Game.Domain.Services;

namespace BackEnd4IdleStrategy.Game.Infrastructure.Repositories;

internal class PlayerRepository : Repository<Player>, IPlayerRepository
{
    public Player Init()
    {
        return base.Init(id => new Player(id));
    }
}