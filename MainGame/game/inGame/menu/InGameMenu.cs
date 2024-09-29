using System.Linq;
using Godot;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.domain;

namespace ZeromaXPlayground.game.inGame.menu;

public partial class InGameMenu : Control
{
    private PanelContainer _topLeftPanel;
    private TabBar _tabBar;
    private GridContainer _playerInfosGrid;
    private Timer _refreshTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _topLeftPanel = GetNode<PanelContainer>("TopLeftPanel");
        _tabBar = GetNode<TabBar>("TopLeftPanel/TopLeftVBox/TabBar");
        _playerInfosGrid = GetNode<GridContainer>("TopLeftPanel/TopLeftVBox/PlayerInfosGrid");
        _refreshTimer = GetNode<Timer>("RefreshTimer");

        _tabBar.TabClicked += tab =>
        {
            if (tab == 0)
            {
                if (_tabBar.CurrentTab == tab)
                {
                    _playerInfosGrid.Show();
                }
                else
                {
                    _playerInfosGrid.Hide();
                }
            }
        };

        _refreshTimer.Timeout += OnRefreshTimerTimeout;
        // 通过设置 Size 使得 PanelContainer 刷新大小为本地化后字符长度，否则会因为默认待替换的本地化字符串长度太长导致它宽度太大
        _topLeftPanel.Size = new Vector2(10, 10);
    }

    private void OnRefreshTimerTimeout()
    {
        // 通过这个功能的实现，就发现现在架构还是很别扭
        // 暂时先每秒查一次，后续重构
        var allPlayerData =
            from player in Player.GetAll()
            select player.Id
            into playerId
            from tile in TileInfo.GetByPlayerId(playerId)
            group tile by playerId
            into playerGroup
            select new
            {
                PlayerId = playerGroup.Key,
                Territory = playerGroup.Count(),
                Population = playerGroup.Sum(tile => tile.Population)
            }
            into playerData
            orderby playerData.Population descending
            select playerData;

        // 清空其他标签
        _playerInfosGrid.GetChildren()
            .Skip(3)
            .ToList()
            .ForEach(child => child.QueueFree());

        foreach (var playerData in allPlayerData)
        {
            var playerLabel = new Label();
            playerLabel.SetText(Tr(Constants.PlayerNames[playerData.PlayerId - 1]));
            playerLabel.AddThemeColorOverride("font_color", Constants.PlayerColors[playerData.PlayerId - 1]);
            _playerInfosGrid.AddChild(playerLabel);

            var territoryLabel = new Label();
            territoryLabel.SetText(playerData.Territory.ToString());
            _playerInfosGrid.AddChild(territoryLabel);

            var populationLabel = new Label();
            populationLabel.SetText(playerData.Population.ToString());
            _playerInfosGrid.AddChild(populationLabel);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}