using BackEnd4IdleStrategy.Game.Domain.Services;
using BackEnd4IdleStrategy.Game.UserInterface.Dto;

namespace BackEnd4IdleStrategy.Game.UserInterface.Controller;

// TODO: CQRS 独立服务和数据库待实现
internal class QueryGameController(IGameService gameService)
{
    public QueryTileDto QueryTileById(int tileId)
    {
        return QueryTileDto.From(gameService.QueryTileById(tileId));
    }

    public IEnumerable<QueryTileDto> QueryTilesByPlayerId(int playerId)
    {
        return gameService.QueryTilesByPlayerId(playerId).Select(QueryTileDto.From);
    }

    public IEnumerable<QueryPlayerDto> QueryAllPlayers()
    {
        return gameService.QueryAllPlayers().Select(QueryPlayerDto.From);
    }

    public QueryPlayerDto QueryPlayerById(int id)
    {
        return QueryPlayerDto.From(gameService.QueryPlayerById(id));
    }
}