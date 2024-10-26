using System.Numerics;
using BackEnd4IdleStrategy.Common.Constants;
using BackEnd4IdleStrategy.Common.Util;
using BackEnd4IdleStrategyFS.Game;

namespace BackEnd4IdleStrategy.Game.UserInterface.Dto;

public record QueryTileDto(
    int Id,
    Vector2 Coord,
    int Population,
    int PlayerId
)
{
    internal static QueryTileDto From(DomainT.Tile tile)
    {
        var playerId = LanguageExt.FSharp.fs(tile.playerId).IsSome
            ? tile.playerId.Value.Item
            : Constant.NullId;
        return new QueryTileDto(tile.id.Item, FSharpUtil.ToVector2(tile.coord), tile.population, playerId);
    }
}