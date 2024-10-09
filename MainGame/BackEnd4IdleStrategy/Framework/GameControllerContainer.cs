using System.Numerics;
using BackEnd4IdleStrategy.Game.Domain.Services;
using BackEnd4IdleStrategy.Game.Infrastructure.Repositories;
using BackEnd4IdleStrategy.Game.UserInterface.Adapter;
using BackEnd4IdleStrategy.Game.UserInterface.Controller;
using BackEnd4IdleStrategy.Game.UserInterface.Dto;

namespace BackEnd4IdleStrategy.Framework;

public class GameControllerContainer
{
    public delegate void TilePopulationChangedHandler(int id);

    public event TilePopulationChangedHandler? TilePopulationChanged;

    public delegate void TileConqueredHandler(int id, int conquerorId, int loserId);

    public event TileConqueredHandler? TileConquered;
    
    private readonly CommandGameController _commandGameController;
    private readonly QueryGameController _queryGameController;

    public GameControllerContainer()
    {
        var gameService = new GameService(
            new PlayerRepository(),
            new TileRepository(),
            new MarchingArmyRepository());
        gameService.TileConquered += (id, conquerorId, loserId) => TileConquered?.Invoke(id, conquerorId, loserId);
        gameService.TilePopulationChanged += id => TilePopulationChanged?.Invoke(id);

        _commandGameController = new CommandGameController(gameService);
        _queryGameController = new QueryGameController(gameService);
    }

    public IEnumerable<InitTileDto> InitTiles(IEnumerable<Vector2> usedCells, INavigationService navService)
    {
        return _commandGameController.InitTiles(usedCells, navService);
    }

    public void AddPopulationToPlayerTiles(int incr)
    {
        _commandGameController.AddPopulationToPlayerTiles(incr);
    }

    public void InitPlayerAndSpawnOnTile(IEnumerable<Vector2> tileCoords)
    {
        _commandGameController.InitPlayerAndSpawnOnTile(tileCoords);
    }

    public int MarchingArmyArriveDestination(int marchingArmyId)
    {
        return _commandGameController.MarchingArmyArriveDestination(marchingArmyId);
    }

    public MarchingArmyDto RandomSendMarchingArmy(int playerId, INavigationService navService)
    {
        return _commandGameController.RandomSendMarchingArmy(playerId, navService);
    }
    
    public  QueryTileDto QueryTileById(int tileId)
    {
        return _queryGameController.QueryTileById(tileId);
    }

    public IEnumerable<QueryTileDto> QueryTilesByPlayerId(int playerId)
    {
        return _queryGameController.QueryTilesByPlayerId(playerId);
    }

    public IEnumerable<QueryPlayerDto> QueryAllPlayers()
    {
        return _queryGameController.QueryAllPlayers();
    }

    public QueryPlayerDto QueryPlayerById(int id)
    {
        return _queryGameController.QueryPlayerById(id);
    }
}