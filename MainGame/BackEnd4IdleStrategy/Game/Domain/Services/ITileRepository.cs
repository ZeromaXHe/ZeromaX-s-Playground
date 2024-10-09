using System.Numerics;
using BackEnd4IdleStrategy.Common.Base;
using BackEnd4IdleStrategy.Game.Domain.Entities;
using LanguageExt;

namespace BackEnd4IdleStrategy.Game.Domain.Services;

internal interface ITileRepository : IRepository<Tile>
{
    Tile Init(Vector2 coord);
    Option<Tile> GetByCoord(Vector2 coord);
    IEnumerable<Tile> GetByPlayerId(int playerId);
}