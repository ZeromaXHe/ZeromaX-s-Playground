using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd4IdleStrategyFS.Game;
using FrontEnd4IdleStrategyFS.Common;
using ZeromaXPlayground.game.inGame.map.scripts.service;

public partial class GlobalNode : Node
{
    public RepositoryT.GameState GameState = null;

    public EventT.EventSubject EventSubject = null;

    public void InitIdleStrategyGame()
    {
        GameState = MainEntry.emptyGameState;
        EventSubject = MainEntry.initEventSubject();

        EventSubject.tileAdded.Subscribe(e =>
            NavigationService.Instance.AddPoint(e.tileId.Item, BackEndUtil.fromBackEnd(e.coord.Item1, e.coord.Item2)));
    }

    public IEnumerable<DomainT.Tile> InitTiles(IEnumerable<Vector2I> usedCells)
    {
        var (gameState, tiles) =
            MainEntry.initTiles(EventSubject, GameState, usedCells.Select(BackEndUtil.toBackEnd));

        GameState = gameState;
        return tiles;
    }

    public void InitPlayerAndSpawnOnTile(IEnumerable<Vector2I> tileCoords) =>
        GameState = MainEntry.initPlayerAndSpawnOnTile(
            EventSubject, GameState, tileCoords.Select(BackEndUtil.toBackEnd));

    public DomainT.MarchingArmy RandomSendMarchingArmy(int playerId)
    {
        var (gameState, marchingArmy) = MainEntry.randomSendMarchingArmy(GameState, playerId, NavServiceQuery);
        GameState = gameState;
        return marchingArmy;

        IEnumerable<int> NavServiceQuery(int navId)
        {
            var connectNavIdArr = NavigationService.Instance.GetPointConnections(navId);
            return connectNavIdArr.Select(l => (int)l);
        }
    }

    public int MarchingArmyArriveDestination(int marchingArmyId)
    {
        var (gameState, playerId) = MainEntry.marchingArmyArriveDestination(
            EventSubject, GameState, marchingArmyId);
        GameState = gameState;
        return playerId;
    }

    public void AddPopulationToPlayerTiles(int incr) =>
        GameState = MainEntry.addPopulationToPlayerTiles(EventSubject, GameState, incr);

    public IEnumerable<DomainT.Player> QueryAllPlayers() =>
        MainEntry.queryAllPlayers(GameState);

    public DomainT.Player QueryPlayerById(int id) =>
        MainEntry.queryPlayerById(GameState, id);

    public DomainT.Tile QueryTileById(int id) =>
        MainEntry.queryTileById(GameState, id);

    public IEnumerable<DomainT.Tile> QueryTilesByPlayerId(int playerId) =>
        MainEntry.queryTilesByPlayerId(GameState, playerId);
}