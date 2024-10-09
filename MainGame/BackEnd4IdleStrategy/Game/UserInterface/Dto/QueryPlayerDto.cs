using BackEnd4IdleStrategy.Game.Domain.Entities;

namespace BackEnd4IdleStrategy.Game.UserInterface.Dto;

public record QueryPlayerDto(int Id)
{
    internal static QueryPlayerDto From(Player player) => new(player.Id);
}