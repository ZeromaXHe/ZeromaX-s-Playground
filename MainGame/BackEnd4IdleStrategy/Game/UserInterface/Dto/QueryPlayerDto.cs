using BackEnd4IdleStrategyFS.Game;

namespace BackEnd4IdleStrategy.Game.UserInterface.Dto;

public record QueryPlayerDto(int Id)
{
    internal static QueryPlayerDto From(DomainT.Player player) => new(player.id.Item);
}