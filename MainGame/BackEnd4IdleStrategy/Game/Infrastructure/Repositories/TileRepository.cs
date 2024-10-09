using System.Numerics;
using BackEnd4IdleStrategy.Common.Base;
using BackEnd4IdleStrategy.Common.Constants;
using BackEnd4IdleStrategy.Game.Domain.Entities;
using BackEnd4IdleStrategy.Game.Domain.Services;
using LanguageExt;
using LanguageExt.SomeHelp;

namespace BackEnd4IdleStrategy.Game.Infrastructure.Repositories;

internal class TileRepository : Repository<Tile>, ITileRepository
{
    private readonly Dictionary<Vector2, int> _coordMap = new();
    private readonly Dictionary<int, List<int>> _playerIdMap = new();

    public Tile Init(Vector2 coord)
    {
        var tile = base.Init(id => new Tile(id, coord));
        tile.PlayerIdChanged += OnTilePlayerIdChanged;
        _coordMap.Add(coord, tile.Id);
        return tile;
    }

    private void OnTilePlayerIdChanged(int id, int prePlayerId, int newPlayerId)
    {
        if (prePlayerId != Constant.NullId)
        {
            _playerIdMap[prePlayerId].Remove(id);
            if (_playerIdMap[prePlayerId].Count == 0)
            {
                _playerIdMap.Remove(prePlayerId);
            }
        }

        if (newPlayerId != Constant.NullId)
        {
            if (_playerIdMap.TryGetValue(newPlayerId, out var getVal))
            {
                getVal.Add(id);
            }
            else
            {
                _playerIdMap.Add(newPlayerId, new List<int> { id });
            }
        }
    }

    public Option<Tile> GetByCoord(Vector2 coord)
    {
        return (_coordMap.TryGetValue(coord, out var result)
                ? result.ToSome()
                : Option<int>.None)
            .Bind(GetById);
    }

    public IEnumerable<Tile> GetByPlayerId(int playerId)
    {
        return (_playerIdMap.TryGetValue(playerId, out var result)
                ? result
                : new List<int>())
            .Select(GetById)
            // Sequence 的作用：IEnumerable<Option> -> Option<IEnumerable>
            // 当 IEnumerable 中有任何值为 None 时，返回 None；否则为 Some
            .Sequence()
            .IfNone(new List<Tile>());
    }
}