using System.Numerics;
using BackEnd4IdleStrategy.Common.Constants;
using BackEnd4IdleStrategy.Common.Util;
using BackEnd4IdleStrategy.Game.Domain.Services;
using BackEnd4IdleStrategy.Game.Infrastructure.Repositories;
using BackEnd4IdleStrategy.Game.UserInterface.Adapter;
using BackEnd4IdleStrategy.Game.UserInterface.Controller;
using BackEnd4IdleStrategy.Game.UserInterface.Dto;
using BackEnd4IdleStrategyFS.Game;
using LanguageExt;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace BackEnd4IdleStrategy.Framework;

public class GameControllerContainer
{
    public delegate void TilePopulationChangedHandler(int id);

    public event TilePopulationChangedHandler? TilePopulationChanged;

    public delegate void TileConqueredHandler(int id, int conquerorId, int loserId);

    public event TileConqueredHandler? TileConquered;

    private RepositoryT.GameState _gameState = MainEntry.emptyGameState;

    // private readonly CommandGameController _commandGameController;
    // private readonly QueryGameController _queryGameController;

    // public GameControllerContainer()
    // {
    //     var gameService = new GameService(
    //         new PlayerRepository(),
    //         new TileRepository(),
    //         new MarchingArmyRepository());
    //     gameService.TileConquered += (id, conquerorId, loserId) => TileConquered?.Invoke(id, conquerorId, loserId);
    //     gameService.TilePopulationChanged += id => TilePopulationChanged?.Invoke(id);
    //
    //     _commandGameController = new CommandGameController(gameService);
    //     _queryGameController = new QueryGameController(gameService);
    // }

    public IEnumerable<InitTileDto> InitTiles(IEnumerable<Vector2> usedCells, INavigationService navService)
    {
        // return _commandGameController.InitTiles(usedCells, navService);

        var (gameState, tiles, navServiceAddEvents) =
            MainEntry.initTiles(_gameState, usedCells.Select(FSharpUtil.ToTupleIntInt));
        foreach (var navServiceAddEvent in navServiceAddEvents)
        {
            navService.AddPoint(navServiceAddEvent.tileId.Item, FSharpUtil.ToVector2(navServiceAddEvent.coord));
        }

        _gameState = gameState;
        return tiles.Select(InitTileDto.From);
    }

    public void AddPopulationToPlayerTiles(int incr)
    {
        // _commandGameController.AddPopulationToPlayerTiles(incr);

        var (gameState, eventSeq) = MainEntry.addPopulationToPlayerTiles(_gameState, incr);
        _gameState = gameState;
        foreach (var tilePopulationChangedEvent in eventSeq)
        {
            TilePopulationChanged?.Invoke(tilePopulationChangedEvent.id.Item);
        }
    }

    public void InitPlayerAndSpawnOnTile(IEnumerable<Vector2> tileCoords)
    {
        // _commandGameController.InitPlayerAndSpawnOnTile(tileCoords);

        var (gameState, eventSeq) =
            MainEntry.initPlayerAndSpawnOnTile(_gameState, tileCoords.Select(FSharpUtil.ToTupleIntInt));
        _gameState = gameState;
        foreach (var e in eventSeq)
        {
            var loserId = LanguageExt.FSharp.fs(e.loserId).IsSome ? e.loserId.Value.Item : Constant.NullId;
            TileConquered?.Invoke(e.id.Item, e.conquerorId.Item, loserId);
        }
    }

    public int MarchingArmyArriveDestination(int marchingArmyId)
    {
        // return _commandGameController.MarchingArmyArriveDestination(marchingArmyId);

        var (gameState, playerId, tileConqueredEventOpt) =
            MainEntry.marchingArmyArriveDestination(_gameState, marchingArmyId);
        // TODO: F# 的 option 转到 C# 就这么麻烦吗？还必须引 LanguageExt.FSharp 包？
        var conqueredEventOpt = LanguageExt.FSharp.fs(tileConqueredEventOpt);
        _gameState = gameState;

        if (conqueredEventOpt.IsNone) return playerId;

        var loserIdOpt = tileConqueredEventOpt.Value.loserId;
        var loserId = LanguageExt.FSharp.fs(loserIdOpt).IsSome ? loserIdOpt.Value.Item : Constant.NullId;
        TileConquered?.Invoke(tileConqueredEventOpt.Value.id.Item, tileConqueredEventOpt.Value.conquerorId.Item,
            loserId);

        return playerId;
    }

    public MarchingArmyDto RandomSendMarchingArmy(int playerId, INavigationService navService)
    {
        // return _commandGameController.RandomSendMarchingArmy(playerId, navService);

        // TODO: 这个函数转换搞死人啊！！！
        var navServiceQueryConverter = new Converter<int, FSharpList<int>>(NavServiceQuery);
        var navFSharpFunc = FSharpFunc<int, FSharpList<int>>.FromConverter(navServiceQueryConverter);
        var (gameState, marchingArmy) = MainEntry.randomSendMarchingArmy(_gameState, playerId, navFSharpFunc);
        _gameState = gameState;
        return MarchingArmyDto.From(marchingArmy,
            InitTileDto.From(MainEntry.queryTileById(_gameState, marchingArmy.fromTileId.Item)),
            InitTileDto.From(MainEntry.queryTileById(_gameState, marchingArmy.toTileId.Item)));

        FSharpList<int> NavServiceQuery(int navId)
        {
            var connectNavIdArr = navService.GetPointConnections(navId);
            return new Lst<int>(connectNavIdArr.Select(l => (int)l)).ToFSharp();
        }
    }

    public QueryTileDto QueryTileById(int tileId)
    {
        // return _queryGameController.QueryTileById(tileId);

        return QueryTileDto.From(MainEntry.queryTileById(_gameState, tileId));
    }

    public IEnumerable<QueryTileDto> QueryTilesByPlayerId(int playerId)
    {
        // return _queryGameController.QueryTilesByPlayerId(playerId);

        return MainEntry.queryTilesByPlayerId(_gameState, playerId)
            .Select(QueryTileDto.From);
    }

    public IEnumerable<QueryPlayerDto> QueryAllPlayers()
    {
        // return _queryGameController.QueryAllPlayers();

        return MainEntry.queryAllPlayers(_gameState).Select(QueryPlayerDto.From);
    }

    public QueryPlayerDto QueryPlayerById(int id)
    {
        // return _queryGameController.QueryPlayerById(id);

        return QueryPlayerDto.From(MainEntry.queryPlayerById(_gameState, id));
    }
}