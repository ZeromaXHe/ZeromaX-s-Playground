using System.Linq;
using BackEnd4IdleStrategy.Game.UserInterface.Dto;
using Godot;
using ZeromaXPlayground.game.inGame.map.scenes;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;
using ZeromaXPlayground.game.inGame.map.scripts.service;
using ZeromaXPlayground.game.inGame.map.scripts.Utils;

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

        _globalNode.GameControllerContainer.TileConquered += OnTileConquered;
        _globalNode.GameControllerContainer.TilePopulationChanged += OnTilePopulationChanged;
        EventBus.Instance.MarchingArmyArrivedDestination += OnMarchingArmyArrivedDestination;

        var usedCells = _baseTerrain.GetUsedCells();

        // 初始化地图
        var tileDict = _globalNode.GameControllerContainer.InitTiles(
                usedCells.Select(BackEndUtil.To),
                NavigationService.Instance)
            .ToDictionary(t => BackEndUtil.From(t.Coord), t => t.Id);
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
        _globalNode.GameControllerContainer.InitPlayerAndSpawnOnTile(
            usedCells.Take(_playerCount)
                .Select(v => BackEndUtil.To(v)));

        _tickTimer.Timeout += OnTickTimerTimeout;
        _tickTimer.Start();

        // 临时测试各个玩家的第一次发起出兵
        GetTree().CreateTimer(3).Timeout += () =>
        {
            GD.Print("各 AI 玩家第一次出征");
            foreach (var player in _globalNode.GameControllerContainer.QueryAllPlayers())
            {
                RandomSendMarchingArmy(player);
            }
        };
    }

    private void RandomSendMarchingArmy(QueryPlayerDto player)
    {
        var marchingArmy =
            _globalNode.GameControllerContainer.RandomSendMarchingArmy(player.Id, NavigationService.Instance);
        // 初始化一次行军部队
        var marchingLine = _marchingLineScene.Instantiate<MarchingLine>();
        _marchingLines.AddChild(marchingLine);
        marchingLine.Init(marchingArmy,
            GetTileCoordGlobalPosition(BackEndUtil.From(marchingArmy.FromTile!.Coord)),
            GetTileCoordGlobalPosition(BackEndUtil.From(marchingArmy.ToTile!.Coord)),
            Constants.PlayerColors[player.Id - 1]); // Player 的 Id 从 1 开始，所以要减一
    }

    private Vector2 GetTileCoordGlobalPosition(Vector2I coord)
    {
        return _territory.ToGlobal(_territory.MapToLocal(coord));
    }

    private void OnMarchingArmyArrivedDestination(int marchingArmyId)
    {
        // 结束之前的行军
        var playerId = _globalNode.GameControllerContainer.MarchingArmyArriveDestination(marchingArmyId);
        // 让玩家再次发起出兵
        RandomSendMarchingArmy(_globalNode.GameControllerContainer.QueryPlayerById(playerId));
    }

    private void OnTileConquered(int tileId, int conquerorId, int loserId)
    {
        var tileInfo = _globalNode.GameControllerContainer.QueryTileById(tileId);
        if (conquerorId == Constants.NullId)
        {
            _territory.EraseCell(BackEndUtil.From(tileInfo.Coord));
        }
        else
        {
            // Player 的 Id 从 1 开始，所以要减一
            _territory.SetCell(BackEndUtil.From(tileInfo.Coord), TerritorySrcId, TerritoryAtlasCoords[conquerorId - 1]);
        }

        // 无主地块第一次被占领，需要初始化地块 GUI
        if (loserId == Constants.NullId)
        {
            var tileGui = _tileGuiScene.Instantiate<TileGui>();
            _tileGuis.AddChild(tileGui); // 先添加子节点，Init 的时候 tileGui 的 _population 才拿得到 Label
            tileGui.Init(tileInfo, GetTileCoordGlobalPosition(BackEndUtil.From(tileInfo.Coord)));
        }
    }

    private void OnTilePopulationChanged(int tileId)
    {
        var tileGui = TileGui.GetById(tileId);
        var tileInfo = _globalNode.GameControllerContainer.QueryTileById(tileId);
        tileGui.ChangePopulation(tileInfo.Population);
    }

    private void OnTickTimerTimeout()
    {
        _globalNode.GameControllerContainer.AddPopulationToPlayerTiles(1);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}