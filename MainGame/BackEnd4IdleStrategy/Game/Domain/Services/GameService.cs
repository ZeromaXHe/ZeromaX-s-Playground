using System.Numerics;
using BackEnd4IdleStrategy.Common.Constants;
using BackEnd4IdleStrategy.Game.Domain.Entities;
using LanguageExt;
using LanguageExt.SomeHelp;
using Microsoft.VisualBasic;

namespace BackEnd4IdleStrategy.Game.Domain.Services;

internal class GameService(
    IPlayerRepository playerRepository,
    ITileRepository tileRepository,
    IMarchingArmyRepository marchingArmyRepository
) : IGameService
{
    public event IGameService.TilePopulationChangedHandler? TilePopulationChanged;

    public event IGameService.TileConqueredHandler? TileConquered;

    public IEnumerable<Tile> InitTiles(IEnumerable<Vector2> usedCells)
    {
        foreach (var usedCell in usedCells)
        {
            var tile = tileRepository.Init(usedCell);
            tile.Conquered += (id, conquerorId, loserId) => TileConquered?.Invoke(id, conquerorId, loserId);
            tile.PopulationChanged += id => TilePopulationChanged?.Invoke(id);
            yield return tile;
        }
    }

    public void AddPopulationToPlayerTiles(int incr)
    {
        foreach (var tile in tileRepository.GetAll())
        {
            tile.AddPopulationIfOwnedByPlayer(incr);
        }
    }

    public void InitPlayerAndSpawnOnTile(IEnumerable<Vector2> tileCoords)
    {
        foreach (var tileCoord in tileCoords)
        {
            var player = playerRepository.Init();
            tileRepository.GetByCoord(tileCoord)
                // 副作用
                .Do(t => t.ConqueredBy(player.Id));
        }
    }

    public void CreateMarchingArmy(int population, int playerId, int fromTileId, int toTileId)
    {
        marchingArmyRepository.Init(population, playerId, fromTileId, toTileId);
        tileRepository.GetById(fromTileId)
            // 副作用
            .Do(t => t.Population -= population);
    }

    public int MarchingArmyArriveDestination(int marchingArmyId)
    {
        int playerId = Constant.NullId;
        marchingArmyRepository.GetById(marchingArmyId)
            // 副作用
            .Do(army =>
            {
                tileRepository.GetById(army.ToTileId)
                    // TODO：这里副作用过分了，必须改改~
                    .Do(destinationTile =>
                    {
                        if (destinationTile.PlayerId == Constant.NullId)
                        {
                            destinationTile.ConqueredBy(army.PlayerId);
                        }

                        if (army.PlayerId == destinationTile.PlayerId)
                        {
                            // 自己领土上移动部队
                            destinationTile.Population += army.Population;
                        }
                        else if (destinationTile.Population >= army.Population)
                        {
                            destinationTile.Population -= army.Population;
                        }
                        else
                        {
                            destinationTile.ConqueredBy(army.PlayerId);
                            destinationTile.Population = army.Population - destinationTile.Population;
                        }
                    });

                playerId = army.PlayerId;
                // 清除掉本条数据，避免内存泄露
                marchingArmyRepository.Remove(marchingArmyId);
            });
        return playerId;
    }

    public Option<int> RandomSendMarchingArmyFrom(int playerId)
    {
        // TODO: 待函数式编程改造
        // 随机出兵地块、目标地块、出动兵力
        var tileInfos = tileRepository.GetByPlayerId(playerId).ToList();
        if (tileInfos.Count == 0)
        {
            // 玩家已经没有领土了，出不了兵
            return Option<int>.None;
        }

        var random = new Random();
        // TODO：为啥去掉 ToSome() 也不会报错，有点离谱
        return tileInfos[random.Next(tileInfos.Count)].Id.ToSome();
    }

    public MarchingArmy RandomSendMarchingArmyTo(int fromTileId, long[] candidateToTileIds)
    {
        var random = new Random();
        var toTileId = candidateToTileIds[random.Next(candidateToTileIds.Length)];
        var toTile = tileRepository.GetById((int)toTileId);
        var fromTile = tileRepository.GetById(fromTileId);
        // TODO: 好丑的副作用写法…… 先跑起来再说吧
        var fromPopulation = 1;
        var playerId = Constant.NullId;
        fromTile.Do(t =>
        {
            fromPopulation = t.Population;
            playerId = t.PlayerId;
        });
        var population = random.Next(1, fromPopulation + 1);
        // 初始化一次行军部队
        return marchingArmyRepository.Init(population, playerId, fromTileId, (int)toTileId);
    }

    public Tile QueryTileById(int id)
    {
        return tileRepository.GetById(id)
            .IfNone(() => throw new Exception("TileById：叫你乱写临时代码，报错了吧！"));
    }

    public IEnumerable<Tile> QueryTilesByPlayerId(int playerId)
    {
        return tileRepository.GetByPlayerId(playerId);
    }

    public IEnumerable<Player> QueryAllPlayers()
    {
        return playerRepository.GetAll();
    }

    public Player QueryPlayerById(int id)
    {
        return playerRepository.GetById(id)
            .IfNone(() => throw new Exception("PlayerById：叫你乱写临时代码，报错了吧！"));
    }
}