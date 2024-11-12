namespace FrontEnd4IdleStrategyFS.Display

open BackEnd4IdleStrategyFS.Game.DomainT
open FrontEnd4IdleStrategyFS.Global.Common
open Godot
open FrontEnd4IdleStrategyFS.Global

type InGameMenuFS() as this =
    inherit Control()

    let _globalNode = lazy this.GetNode<GlobalNodeFS> "/root/GlobalNode"
    let _topLeftPanel = lazy this.GetNode<PanelContainer> "TopLeftPanel"
    let _tabBar = lazy this.GetNode<TabBar> "TopLeftPanel/TopLeftVBox/TabBar"

    let _playerInfosGrid =
        lazy this.GetNode<GridContainer> "TopLeftPanel/TopLeftVBox/PlayerInfosGrid"

    let _refreshTimer = lazy this.GetNode<Timer> "RefreshTimer"

    let onTabBarTabClicked (tab: int64) =
        if tab = 0 then
            if _tabBar.Value.CurrentTab = int tab then
                _playerInfosGrid.Value.Show()
            else
                _playerInfosGrid.Value.Hide()


    /// 通过这个功能的实现，就发现现在架构还是很别扭
    /// 暂时先每秒查一次，后续重构
    let onRefreshTimerTimeout () =
        // 清空其他标签
        _playerInfosGrid.Value.GetChildren() |> Seq.skip 3 |> Seq.iter _.QueueFree()

        _globalNode.Value.IdleStrategyEntry.Value.QueryAllPlayers()
        |> Seq.map _.Id
        |> Seq.collect (fun playerId ->
            let (PlayerId playerIdInt) = playerId
            _globalNode.Value.IdleStrategyEntry.Value.QueryTilesByPlayerId playerIdInt)
        |> Seq.groupBy (_.PlayerId)
        |> Seq.map (fun kv ->
            let playerId, tiles = kv

            {| PlayerId = playerId
               Territory = tiles |> Seq.length
               Population = tiles |> Seq.sumBy _.Population |})
        |> Seq.sortByDescending _.Population
        |> Seq.iter (fun playerInfo ->
            let playerLabel = new Label()
            let (PlayerId playerIdInt) = playerInfo.PlayerId.Value
            playerLabel.SetText <| this.Tr Constants.playerNames[playerIdInt - 1]
            playerLabel.AddThemeColorOverride("font_color", Constants.playerColors[playerIdInt - 1])
            _playerInfosGrid.Value.AddChild playerLabel

            let territoryLabel = new Label()
            territoryLabel.SetText <| string playerInfo.Territory
            _playerInfosGrid.Value.AddChild territoryLabel

            let populationLabel = new Label()
            populationLabel.SetText <| string playerInfo.Population 
            _playerInfosGrid.Value.AddChild populationLabel)

    override this._Ready() =
        _tabBar.Value.add_TabClicked onTabBarTabClicked
        _refreshTimer.Value.add_Timeout onRefreshTimerTimeout
        // 通过设置 Size 使得 PanelContainer 刷新大小为本地化后字符长度，否则会因为默认待替换的本地化字符串长度太长导致它宽度太大
        _topLeftPanel.Value.Size <- Vector2(10f, 10f)
