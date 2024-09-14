using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using ZeromaXPlayground.game.inGame.map.scripts;
using ZeromaXPlayground.game.inGame.map.scripts.utils.efCoreSqlite;
using Player = ZeromaXPlayground.game.inGame.map.scripts.Player;

public partial class MapBoard : Node2D
{
    private TileMapLayer _baseTerrain;
    private TileMapLayer _feature;
    private TileMapLayer _territory;
    private Timer _tickTimer;

    private readonly DatabaseContext _databaseContext = new();

    [Export(PropertyHint.Range, "2,8")] private int _playerCount = 2;

    private GameMap _gameMap;
    private List<Player> _playerList;

    private const int TerritorySrcId = 0;

    private static readonly Vector2I[] TerritoryAtlasCoords =
    {
        new(0, 2), new(1, 2), new(2, 2), new(3, 2),
        new(0, 3), new(1, 3), new(2, 3), new(3, 3)
    };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // on-ready params
        _baseTerrain = GetNode<TileMapLayer>("BaseTerrain");
        _feature = GetNode<TileMapLayer>("Feature");
        _territory = GetNode<TileMapLayer>("Territory");
        _tickTimer = GetNode<Timer>("TickTimer");

        // 清除用于编辑器中预览的单元格
        _feature.Clear();
        _territory.Clear();

        _gameMap = new GameMap(_baseTerrain);
        _playerList = new List<Player>();

        var usedCells = _baseTerrain.GetUsedCells();
        usedCells.Shuffle();
        for (int i = 0; i < _playerCount; i++)
        {
            GD.Print($"creating player {i}");
            var player = new Player(i);
            _playerList.Add(player);
            player.TerritoryConquered += OnPlayerTerritoryConquered;
            player.TerritoryLost += OnPlayerTerritoryLost;
            player.ConquerTerritory(usedCells[i]);
        }

        _tickTimer.Timeout += OnTickTimerTimeout;
        _tickTimer.Start();

        InitDatabase();
    }

    private void InitDatabase()
    {
        GD.Print("创建数据库开始");
        // 检查并创建中间目录
        EnsureDirectoryExists(_databaseContext.DbFilePath);

        if (!File.Exists(_databaseContext.DbFilePath))
        {
            if (_databaseContext.Database.EnsureCreated())
            {
                GD.Print("数据库创建成功");
            }
            else
            {
                GD.Print("数据库创建失败");
            }
        }
        else
        {
            GD.Print("数据库已经存在，无需创建");
        }
    }

    /**
     * 确保路径所在目录存在，支持递归创建路径上的多个目录
     */
    private static void EnsureDirectoryExists(string path)
    {
        string dir = Path.GetDirectoryName(path);
        if (dir != null && !Directory.Exists(dir))
        {
            EnsureDirectoryExists(dir);
        }

        Directory.CreateDirectory(path);
    }

    private void OnPlayerTerritoryConquered(Player player, Vector2I vec)
    {
        GD.Print($"player id: {player.Id} conquered territory at vec: {vec}");
        _territory.SetCell(vec, TerritorySrcId, TerritoryAtlasCoords[player.Id]);
    }

    private void OnPlayerTerritoryLost(Player player, Vector2I vec)
    {
        GD.Print($"player id: {player.Id} lost territory at vec: {vec}");
        // TODO: 可能存在的异步问题？比如 A 占领了 B 的领土的时候，会不会 B 丢失的事件后执行？
        _territory.EraseCell(vec);
    }

    private void OnTickTimerTimeout()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}