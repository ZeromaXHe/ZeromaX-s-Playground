using System.Numerics;
using BackEnd4IdleStrategy.Game.Domain.Entities;
using LanguageExt;

namespace BackEnd4IdleStrategy.Game.Domain.Services;

internal interface IGameService
{
    public delegate void TilePopulationChangedHandler(int id);

    public event TilePopulationChangedHandler? TilePopulationChanged;

    public delegate void TileConqueredHandler(int id, int conquerorId, int loserId);

    public event TileConqueredHandler? TileConquered;
    
    IEnumerable<Tile> InitTiles(IEnumerable<Vector2> usedCells);
    void AddPopulationToPlayerTiles(int incr);
    void InitPlayerAndSpawnOnTile(IEnumerable<Vector2> tileCoords);
    void CreateMarchingArmy(int population, int playerId, int fromTileId, int toTileId);
    int MarchingArmyArriveDestination(int marchingArmyId);
    Option<int> RandomSendMarchingArmyFrom(int playerId);
    MarchingArmy RandomSendMarchingArmyTo(int fromTileId, long[] candidateToTileIds);

    /// <summary>
    /// TODO：不应该写在这里暴露出去的，先这样跑通代码吧
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Tile QueryTileById(int id);

    IEnumerable<Tile> QueryTilesByPlayerId(int playerId);
    IEnumerable<Player> QueryAllPlayers();
    Player QueryPlayerById(int id);
}