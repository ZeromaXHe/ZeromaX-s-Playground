using System;
using System.Linq;
using System.Reactive.Linq;
using BackEnd4IdleStrategyFS.Game;
using Godot;
using ZeromaXPlayground.game.Global.Common;
using ZeromaXPlayground.game.inGame.map.scenes;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;
using ZeromaXPlayground.game.inGame.map.scripts.service;

namespace ZeromaXPlayground.game.inGame.map;

public partial class MapBoard : Node2D
{
    #region on-ready nodes

    private GlobalNode _globalNode;

    private TileMapLayer _baseTerrain;
    private TileMapLayer _feature;
    private TileMapLayer _territory;
    private Timer _tickTimer;
    private Node2D _marchingLines;
    private Control _tileGuis;

    #endregion

    private PackedScene _tileGuiScene = GD.Load("res://game/inGame/map/scenes/TileGUI.tscn") as PackedScene;
    private PackedScene _marchingLineScene = GD.Load("res://game/inGame/map/scenes/MarchingLine.tscn") as PackedScene;

    [Export(PropertyHint.Range, "2,8")] private int _playerCount = 2;

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
        _globalNode = GetNode<GlobalNode>("/root/GlobalNode");
        _baseTerrain = GetNode<TileMapLayer>("BaseTerrain");
        _feature = GetNode<TileMapLayer>("Feature");
        _territory = GetNode<TileMapLayer>("Territory");
        _tickTimer = GetNode<Timer>("TickTimer");
        _marchingLines = GetNode<Node2D>("MarchingLines");
        _tileGuis = GetNode<Control>("TileGUIs");

        // 清除用于编辑器中预览的单元格
        // _feature.Clear();
        _territory.Clear();

        _globalNode.InitIdleStrategyGame();
        _globalNode.EntryContainer.TileConquered
            .Subscribe(e =>
                // Player 的 Id 从 1 开始，所以要减一
                _territory.SetCell(BackEndUtil.FromI(e.coord),
                    TerritorySrcId, TerritoryAtlasCoords[e.conquerorId.Item - 1]));
        _globalNode.EntryContainer.TileConquered
            .Where(e => e.loserId == null)
            .Subscribe(e =>
                NewTileGui(e.id.Item, e.coord, e.population));
        _globalNode.EntryContainer.TilePopulationChanged
            .Subscribe(e =>
                TileGui.ChangePopulation(e.id.Item, e.afterPopulation));

        EventBus.Instance.MarchingArmyArrivedDestination += OnMarchingArmyArrivedDestination;

        var usedCells = _baseTerrain.GetUsedCells();

        // 初始化地图
        var tileDict = _globalNode.EntryContainer.InitTiles(usedCells.Select(BackEndUtil.To))
            .ToDictionary(t => BackEndUtil.FromI(t.coord), t => t.id.Item);

        var tileIds = string.Join(", ", tileDict);
        GD.Print($"初始化地块 {tileIds}");

        foreach (var cell in usedCells)
        {
            // 完成连接图
            var surroundings = _baseTerrain.GetSurroundingCells(cell);
            foreach (var surrounding in surroundings)
            {
                if (tileDict.TryGetValue(surrounding, out var surroundingTileId)
                    && tileDict.TryGetValue(cell, out var cellTileId))
                {
                    NavigationService.Instance.ConnectPoints(cellTileId, surroundingTileId);
                }
            }
        }

        // 生成玩家
        usedCells.Shuffle();
        _globalNode.EntryContainer.InitPlayerAndSpawnOnTile(usedCells.Take(_playerCount).Select(BackEndUtil.To));

        var playerIds = string.Join(", ",
            _globalNode.EntryContainer.QueryAllPlayers()
                .Select(dto => dto.id.Item));
        GD.Print($"玩家初始化完成：{playerIds}");

        _tickTimer.Timeout += OnTickTimerTimeout;
        _tickTimer.Start();

        // 临时测试各个玩家的第一次发起出兵
        GetTree().CreateTimer(3).Timeout += () =>
        {
            GD.Print("各 AI 玩家第一次出征");
            foreach (var player in _globalNode.EntryContainer.QueryAllPlayers())
            {
                RandomSendMarchingArmy(player);
            }
        };
    }

    private void RandomSendMarchingArmy(DomainT.Player player)
    {
        var marchingArmy =
            _globalNode.EntryContainer.RandomSendMarchingArmy(player.id.Item);
        GD.Print($"AI 玩家 {player.id.Item} 发出部队 {marchingArmy.id.Item} 人数为 {marchingArmy.population}");
        // 初始化一次行军部队
        var marchingLine = _marchingLineScene.Instantiate<MarchingLine>();
        _marchingLines.AddChild(marchingLine);
        var fromCoord = _globalNode.EntryContainer.QueryTileById(marchingArmy.fromTileId.Item).coord;
        var toCoord = _globalNode.EntryContainer.QueryTileById(marchingArmy.toTileId.Item).coord;
        marchingLine.Init(marchingArmy,
            GetTileCoordGlobalPosition(BackEndUtil.FromI(fromCoord)),
            GetTileCoordGlobalPosition(BackEndUtil.FromI(toCoord)),
            Constants.PlayerColors[player.id.Item - 1]); // Player 的 Id 从 1 开始，所以要减一
    }

    private Vector2 GetTileCoordGlobalPosition(Vector2I coord)
    {
        return _territory.ToGlobal(_territory.MapToLocal(coord));
    }

    private void OnMarchingArmyArrivedDestination(int marchingArmyId)
    {
        // 结束之前的行军
        var playerId = _globalNode.EntryContainer.MarchingArmyArriveDestination(marchingArmyId);
        // 让玩家再次发起出兵
        RandomSendMarchingArmy(_globalNode.EntryContainer.QueryPlayerById(playerId));
    }

    private void NewTileGui(int id, Tuple<int, int> coord, int population)
    {
        var tileGui = _tileGuiScene.Instantiate<TileGui>();
        _tileGuis.AddChild(tileGui); // 先添加子节点，Init 的时候 tileGui 的 _population 才拿得到 Label
        tileGui.Init(id, coord, population, GetTileCoordGlobalPosition(BackEndUtil.FromI(coord)));
    }

    private void OnTickTimerTimeout()
    {
        _globalNode.EntryContainer.AddPopulationToPlayerTiles(1);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}