using System;
using Godot;
using ZeromaXPlayground.game.inGame.map.scenes;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.domain;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;
using ZeromaXPlayground.game.inGame.map.scripts.service;

namespace ZeromaXPlayground.game.inGame.map;

public partial class MapBoard : Node2D
{
    #region on-ready nodes

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

    private static readonly Color[] PlayerColors =
    {
        Colors.Red, Colors.Yellow, Colors.Green, Colors.Aqua,
        Colors.Blue, Colors.Purple, Colors.DeepPink, Colors.Orange
    };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // on-ready params
        _baseTerrain = GetNode<TileMapLayer>("BaseTerrain");
        _feature = GetNode<TileMapLayer>("Feature");
        _territory = GetNode<TileMapLayer>("Territory");
        _tickTimer = GetNode<Timer>("TickTimer");
        _marchingLines = GetNode<Node2D>("MarchingLines");
        _tileGuis = GetNode<Control>("TileGUIs");

        // 清除用于编辑器中预览的单元格
        // _feature.Clear();
        _territory.Clear();

        EventBus.Instance.TileConquered += OnTileConquered;
        EventBus.Instance.TilePopulationChanged += OnTilePopulationChanged;
        EventBus.Instance.MarchingArmyArrivedDestination += OnMarchingArmyArrivedDestination;

        TileInfo.InitMapTile(_baseTerrain);
        Player.InitAndSpawnOnTile(_baseTerrain, _playerCount);

        _tickTimer.Timeout += OnTickTimerTimeout;
        _tickTimer.Start();

        // 临时测试各个玩家的第一次发起出兵
        GetTree().CreateTimer(3).Timeout += () =>
        {
            GD.Print("各 AI 玩家第一次出征");
            Player.GetAll().ForEach(RandomSendMarchingArmy);
        };
    }

    private void RandomSendMarchingArmy(Player player)
    {
        // 随机出兵地块、目标地块、出动兵力
        var tileInfos = TileInfo.GetByPlayerId(player.Id);
        if (tileInfos.Count == 0)
        {
            // 玩家已经没有领土了，出不了兵
            GD.Print($"玩家 {player.Id} 已经灭亡！");
            return;
        }

        var random = new Random();
        var fromTile = tileInfos[random.Next(tileInfos.Count)];
        var candidateToTileIds = NavigationService.Instance.GetPointConnections(fromTile.Id);
        var toTileId = candidateToTileIds[random.Next(candidateToTileIds.Length)];
        var toTile = TileInfo.GetById((int)toTileId);
        var population = random.Next(1, fromTile.Population + 1);
        // 初始化一次行军部队
        var marchingArmy = new MarchingArmy(population, player.Id, fromTile.Id, toTile.Id);
        var marchingLine = _marchingLineScene.Instantiate<MarchingLine>();
        _marchingLines.AddChild(marchingLine);
        marchingLine.Init(marchingArmy, GetTileCoordGlobalPosition(fromTile.Coord),
            GetTileCoordGlobalPosition(toTile.Coord), PlayerColors[player.Id - 1]); // Player 的 Id 从 1 开始，所以要减一
    }

    private Vector2 GetTileCoordGlobalPosition(Vector2I coord)
    {
        return _territory.ToGlobal(_territory.MapToLocal(coord));
    }

    private void OnMarchingArmyArrivedDestination(int marchingArmyId)
    {
        // 结束之前的行军
        var marchingArmy = MarchingArmy.GetById(marchingArmyId);
        marchingArmy.ArriveDestination();
        // 让玩家再次发起出兵
        RandomSendMarchingArmy(Player.GetById(marchingArmy.PlayerId));
    }

    private void OnTileConquered(int tileId, int conquerorId, int loserId)
    {
        var tileInfo = TileInfo.GetById(tileId);
        if (conquerorId == Constants.NullId)
        {
            _territory.EraseCell(tileInfo.Coord);
        }
        else
        {
            // Player 的 Id 从 1 开始，所以要减一
            _territory.SetCell(tileInfo.Coord, TerritorySrcId, TerritoryAtlasCoords[conquerorId - 1]);
        }

        // 无主地块第一次被占领，需要初始化地块 GUI
        if (loserId == Constants.NullId)
        {
            var tileGui = _tileGuiScene.Instantiate<TileGui>();
            _tileGuis.AddChild(tileGui); // 先添加子节点，Init 的时候 tileGui 的 _population 才拿得到 Label
            tileGui.Init(tileInfo, GetTileCoordGlobalPosition(tileInfo.Coord));
        }
    }

    private void OnTilePopulationChanged(int tileId)
    {
        var tileGui = TileGui.GetById(tileId);
        var tileInfo = TileInfo.GetById(tileId);
        tileGui.ChangePopulation(tileInfo.Population);
    }

    private void OnTickTimerTimeout()
    {
        TileInfo.AllPlayerTilesAddPopulation(1);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}