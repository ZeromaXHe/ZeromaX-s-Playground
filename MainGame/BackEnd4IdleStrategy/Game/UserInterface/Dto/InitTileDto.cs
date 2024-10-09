using System.Numerics;
using BackEnd4IdleStrategy.Game.Domain.Entities;

namespace BackEnd4IdleStrategy.Game.UserInterface.Dto;

public record InitTileDto(
    int Id,
    Vector2 Coord
)
{
    internal static InitTileDto From(Tile tile)
        => new(tile.Id, tile.Coord);
}