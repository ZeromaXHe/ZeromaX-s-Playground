using System.Numerics;
using BackEnd4IdleStrategy.Common.Util;
using BackEnd4IdleStrategyFS.Game;

namespace BackEnd4IdleStrategy.Game.UserInterface.Dto;

public record InitTileDto(
    int Id,
    Vector2 Coord
)
{
    internal static InitTileDto From(DomainT.Tile tile)
        => new(tile.id.Item, FSharpUtil.ToVector2(tile.coord));
}