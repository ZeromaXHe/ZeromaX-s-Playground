using System;
using System.Reactive.Linq;
using System.Threading;
using BackEnd4IdleStrategyFS.Game;
using Godot;
using ZeromaXPlayground.game.Global.Common;
using ZeromaXPlayground.game.inGame.map.scenes;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using Timer = Godot.Timer;

namespace ZeromaXPlayground.game.inGame.map;

public partial class MapBoard : Node2D
{
    #region on-ready nodes

    private GlobalNode _globalNode;

    private TileMapLayer _baseTerrain;
    private TileMapLayer _feature;
    private TileMapLayer _territory;
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
        _marchingLines = GetNode<Node2D>("MarchingLines");
        _tileGuis = GetNode<Control>("TileGUIs");

        // 清除用于编辑器中预览的单元格
        // _feature.Clear();
        _territory.Clear();

        _globalNode.InitIdleStrategyGame(_baseTerrain, _playerCount);

        var godotSyncContext = SynchronizationContext.Current!;

        _globalNode.EntryContainer.GameTicked
            .SubscribeOn(godotSyncContext)
            .Subscribe(e => GD.Print($"tick {e}"));

        _globalNode.EntryContainer.GameFirstArmyGenerated
            .SubscribeOn(godotSyncContext)
            .Subscribe(e => GD.Print($"第一次出兵！！！"));

        _globalNode.EntryContainer.TileAdded
            .SubscribeOn(godotSyncContext)
            .Subscribe(e => GD.Print($"Tile {e.TileId} added at {e.Coord}"));

        _globalNode.EntryContainer.TileConquered
            .SubscribeOn(godotSyncContext)
            .Subscribe(e =>
                godotSyncContext.Post(_ =>
                {
                    // 涉及到 Godot 节点展示层绘制的内容必须在同步上下文中执行
                    // Player 的 Id 从 1 开始，所以要减一
                    _territory.SetCell(BackEndUtil.FromI(e.Coord),
                        TerritorySrcId, TerritoryAtlasCoords[e.ConquerorId.Item - 1]);
                }, null));

        _globalNode.EntryContainer.TileConquered
            // 占领无主空地时
            .Where(e => e.LoserId == null)
            .SubscribeOn(godotSyncContext)
            .Subscribe(e =>
            {
                godotSyncContext.Post(_ =>
                {
                    GD.Print($"Player {e.ConquerorId} conquer tile at {e.Coord}");
                    NewTileGui(e.TileId.Item, e.Coord, e.Population);
                }, null);
            });

        _globalNode.EntryContainer.TilePopulationChanged
            .SubscribeOn(godotSyncContext)
            .Subscribe(e =>
                godotSyncContext.Post(_ =>
                    TileGui.ChangePopulation(e.TileId.Item, e.AfterPopulation), null));

        _globalNode.EntryContainer.MarchingArmyAdded
            .SubscribeOn(godotSyncContext)
            .Subscribe(e =>
            {
                // 涉及到 Godot 节点展示层绘制的内容必须在同步上下文中执行
                // 如果使用 CallDeferred / 信号等，还是会一样报错（信号是因为 EmitSignal 也必须在主线程里）
                // 目前貌似只有同步上下文这种方式可以解决
                godotSyncContext.Post(_ => ShowMarchingArmy(e), null);
            });

        // 必须在同步上下文中执行，否则 Init 内容不会被响应式编程 Subscribe 监听到（会比上面监听逻辑更早执行）
        godotSyncContext.Post(_ => _globalNode.EntryContainer.Init(), null);
        GD.Print("MapBoard 初始化完成!");
    }

    private void ShowMarchingArmy(EventT.MarchingArmyAddedEvent e)
    {
        var armyId = e.MarchingArmyId.Item;
        var fromTileId = e.FromTileId.Item;
        var toTileId = e.ToTileId.Item;
        var playerId = e.PlayerId.Item;
        GD.Print($"AI 玩家 {playerId} 发出部队 {armyId} 人数为 {e.Population}");
        // 初始化一次行军部队
        var marchingLine = _marchingLineScene.Instantiate<MarchingLine>();
        _marchingLines.AddChild(marchingLine);
        var fromCoord = _globalNode.EntryContainer.QueryTileById(fromTileId).Coord;
        var toCoord = _globalNode.EntryContainer.QueryTileById(toTileId).Coord;
        marchingLine.Init(e.MarchingArmyId.Item, e.Population,
            GetTileCoordGlobalPosition(BackEndUtil.FromI(fromCoord)),
            GetTileCoordGlobalPosition(BackEndUtil.FromI(toCoord)),
            Constants.PlayerColors[playerId - 1]); // Player 的 Id 从 1 开始，所以要减一
    }

    private Vector2 GetTileCoordGlobalPosition(Vector2I coord)
    {
        return _territory.ToGlobal(_territory.MapToLocal(coord));
    }

    private void NewTileGui(int id, Tuple<int, int> coord, int population)
    {
        var tileGui = _tileGuiScene.Instantiate<TileGui>();
        _tileGuis.AddChild(tileGui); // 先添加子节点，Init 的时候 tileGui 的 _population 才拿得到 Label
        tileGui.Init(id, coord, population, GetTileCoordGlobalPosition(BackEndUtil.FromI(coord)));
    }
}