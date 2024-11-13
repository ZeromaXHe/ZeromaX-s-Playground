namespace FrontEnd4IdleStrategyFS.Display

open BackEnd4IdleStrategyFS.Game.RepositoryT
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

    let onTabBarTabClicked (tab: int64) =
        if tab = 0 then
            if _tabBar.Value.CurrentTab = int tab then
                _playerInfosGrid.Value.Show()
            else
                _playerInfosGrid.Value.Hide()

    let mutable playerStatLabelsList: Label list list = []

    let newPlayerStatLabels playerCount =
        let labelInit _ = new Label()
        let labelListInit _ = List.init 4 labelInit
        playerStatLabelsList <- List.init playerCount labelListInit

        playerStatLabelsList
        |> List.iter (fun l -> l |> List.iter _playerInfosGrid.Value.AddChild)

    let onPlayerStatUpdated (playerMap: Map<PlayerId, PlayerStat>) =
        let list =
            playerMap
            |> Map.toList
            |> List.sortByDescending (fun (_, playerStat) -> playerStat.TilePopulation)

        if playerStatLabelsList.IsEmpty then
            newPlayerStatLabels list.Length

        list
        |> List.iteri (fun i (playerId, playerStat) ->
            playerStatLabelsList[i]
            |> List.iteri (fun j label ->
                match j with
                | 0 ->
                    let (PlayerId playerIdInt) = playerId
                    label.AddThemeColorOverride("font_color", Constants.playerColors[playerIdInt - 1])
                    label.SetText <| this.Tr Constants.playerNames[playerIdInt - 1]
                | 1 -> label.SetText <| string playerStat.Territory
                | 2 -> label.SetText <| string playerStat.TilePopulation
                | _ -> label.SetText <| string playerStat.ArmyPopulation))

    /// 清空其他标签
    let clearPlayStat () =
        _playerInfosGrid.Value.GetChildren() |> Seq.skip 4 |> Seq.iter _.QueueFree()

    override this._Ready() =
        _tabBar.Value.add_TabClicked onTabBarTabClicked

        // 通过设置 Size 使得 PanelContainer 刷新大小为本地化后字符长度，否则会因为默认待替换的本地化字符串长度太长导致它宽度太大
        _topLeftPanel.Value.Size <- Vector2(10f, 10f)

        clearPlayStat ()

        _globalNode.Value.IdleStrategyEntry.Value.PlayerStatUpdated
        |> ObservableSyncContextUtil.subscribePost onPlayerStatUpdated
