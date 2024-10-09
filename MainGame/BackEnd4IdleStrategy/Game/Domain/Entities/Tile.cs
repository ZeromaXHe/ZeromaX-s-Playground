using System.Numerics;
using BackEnd4IdleStrategy.Common.Base;
using BackEnd4IdleStrategy.Common.Constants;

namespace BackEnd4IdleStrategy.Game.Domain.Entities;

internal class Tile(int id, Vector2 coord) : Entity(id)
{
    public delegate void TilePopulationChangedHandler(int id);

    public event TilePopulationChangedHandler? PopulationChanged;

    public delegate void TilePlayerIdChangedHandler(int id, int prePlayerId, int newPlayerId);

    public event TilePlayerIdChangedHandler? PlayerIdChanged;

    public delegate void TileConqueredHandler(int id, int conquerorId, int loserId);

    public event TileConqueredHandler? Conquered;

    /**
     * 坐标
     */
    public Vector2 Coord { get; } = coord;

    /**
     * 人口
     */
    public int Population
    {
        get => _population;
        set
        {
            _population = value;
            PopulationChanged?.Invoke(Id);
        }
    }

    private int _population;

    /**
     * 所属的玩家
     */
    public int PlayerId
    {
        get => _playerId;
        private set
        {
            PlayerIdChanged?.Invoke(Id, _playerId, value);
            _playerId = value;
        }
    }

    private int _playerId = Constant.NullId;


    public void AddPopulationIfOwnedByPlayer(int incr)
    {
        if (PlayerId != Constant.NullId && Population < 1000)
        {
            Population += incr;
        }
    }

    public void ConqueredBy(int conquerorId)
    {
        Conquered?.Invoke(Id, conquerorId, PlayerId);
        PlayerId = conquerorId;
    }
}