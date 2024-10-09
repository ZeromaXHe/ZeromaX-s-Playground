using System.Numerics;
using BackEnd4IdleStrategy.Game.Domain.Services;
using BackEnd4IdleStrategy.Game.UserInterface.Adapter;
using BackEnd4IdleStrategy.Game.UserInterface.Dto;

namespace BackEnd4IdleStrategy.Game.UserInterface.Controller;

internal class CommandGameController(IGameService gameService)
{
    public IEnumerable<InitTileDto> InitTiles(IEnumerable<Vector2> usedCells, INavigationService navService)
    {
        var initTileDtos = gameService.InitTiles(usedCells)
            .Select(InitTileDto.From);
        var initTiles = initTileDtos.ToList();
        foreach (var (id, coord) in initTiles)
        {
            navService.AddPoint(id, coord);
        }
        return initTiles;
    }

    public void AddPopulationToPlayerTiles(int incr)
    {
        gameService.AddPopulationToPlayerTiles(incr);
    }

    public void InitPlayerAndSpawnOnTile(IEnumerable<Vector2> tileCoords)
    {
        gameService.InitPlayerAndSpawnOnTile(tileCoords);
    }

    public int MarchingArmyArriveDestination(int marchingArmyId)
    {
        return gameService.MarchingArmyArriveDestination(marchingArmyId);
    }

    public MarchingArmyDto RandomSendMarchingArmy(int playerId, INavigationService navService)
    {
        return gameService.RandomSendMarchingArmyFrom(playerId)
            .Select(fromTileId =>
            {
                var candidateToTileIds = navService.GetPointConnections(fromTileId);
                var marchingArmy = gameService.RandomSendMarchingArmyTo(fromTileId, candidateToTileIds);
                return MarchingArmyDto.From(marchingArmy,
                    gameService.QueryTileById(fromTileId),
                    gameService.QueryTileById(marchingArmy.ToTileId));
            })
            .IfNone(new MarchingArmyDto());
    }
}