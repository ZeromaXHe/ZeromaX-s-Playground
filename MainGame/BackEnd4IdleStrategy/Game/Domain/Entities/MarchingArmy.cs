using BackEnd4IdleStrategy.Common.Base;

namespace BackEnd4IdleStrategy.Game.Domain.Entities;

internal class MarchingArmy(
    int id,
    int population,
    int playerId,
    int fromTileId,
    int toTileId) : Entity(id)
{
    public int Population { get; } = population;

    public int PlayerId { get; } = playerId;

    public int FromTileId { get; } = fromTileId;

    public int ToTileId { get; } = toTileId;
}