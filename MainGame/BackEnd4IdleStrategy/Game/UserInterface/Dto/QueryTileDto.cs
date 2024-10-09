using System.Numerics;
using BackEnd4IdleStrategy.Game.Domain.Entities;

namespace BackEnd4IdleStrategy.Game.UserInterface.Dto;

public record QueryTileDto(
    int Id,
    Vector2 Coord,
    int Population,
    int PlayerId
)
{
    internal static QueryTileDto From(Tile tile) =>
        new(tile.Id, tile.Coord, tile.Population, tile.PlayerId);
}