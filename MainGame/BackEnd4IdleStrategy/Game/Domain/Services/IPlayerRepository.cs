using BackEnd4IdleStrategy.Common.Base;
using BackEnd4IdleStrategy.Game.Domain.Entities;

namespace BackEnd4IdleStrategy.Game.Domain.Services;

internal interface IPlayerRepository: IRepository<Player>
{
    Player Init();
}