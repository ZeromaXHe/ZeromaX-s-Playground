using Godot;
using ZeromaXPlayground.game.inGame.map.scenes;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.domain;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;

namespace ZeromaXPlayground.game.inGame.map;

public partial class MapBoard : Node2D
{
    #region on-ready nodes

    private TileMapLayer _baseTerrain;
    private TileMapLayer _feature;
    private TileMapLayer _territory;
    private Timer _tickTimer;
    private Control _tileGuis;

    #endregion

    private PackedScene _tileGuiScene = GD.Load("res://game/inGame/map/scenes/TileGUI.tscn") as PackedScene;
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
        _baseTerrain = GetNode<TileMapLayer>("BaseTerrain");
        _feature = GetNode<TileMapLayer>("Feature");
        _territory = GetNode<TileMapLayer>("Territory");
        _tickTimer = GetNode<Timer>("TickTimer");
        _tileGuis = GetNode<Control>("TileGUIs");

        // 清除用于编辑器中预览的单元格
        // _feature.Clear();
        _territory.Clear();

        EventBus.Instance.TileConquered += OnTileConquered;

        TileInfo.InitMapTile(_baseTerrain);
        Player.InitAndSpawnOnTile(_baseTerrain, _playerCount);

        _tickTimer.Timeout += OnTickTimerTimeout;
        _tickTimer.Start();
    }

    private void OnTileConquered(int conquerorId, int loserId, Vector2I vec)
    {
        if (conquerorId == Constants.NullId)
        {
            _territory.EraseCell(vec);
        }
        else
        {
            // Player 的 Id 从 1 开始，所以要减一
            _territory.SetCell(vec, TerritorySrcId, TerritoryAtlasCoords[conquerorId - 1]);
        }

        // 无主地块第一次被占领，需要初始化地块 GUI
        if (loserId == Constants.NullId)
        {
            var tileGui = _tileGuiScene.Instantiate<TileGui>();
            _tileGuis.AddChild(tileGui); // 先添加子节点，Init 的时候 tileGui 的 _population 才拿得到 Label
            tileGui.Init(TileInfo.GetByCoord(vec).Id);
            tileGui.Position = _territory.ToGlobal(_territory.MapToLocal(vec));
        }
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