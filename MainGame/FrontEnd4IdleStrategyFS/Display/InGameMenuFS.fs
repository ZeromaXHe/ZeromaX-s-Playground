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

    let _gameSpeedHBox =
        lazy this.GetNode<HBoxContainer> "TopLeftPanel/TopLeftVBox/SpeedMultiplierHBox"

    let _pauseButton =
        lazy this.GetNode<Button> "TopLeftPanel/TopLeftVBox/SpeedMultiplierHBox/Pause"

    let _speed025xButton =
        lazy this.GetNode<Button> "TopLeftPanel/TopLeftVBox/SpeedMultiplierHBox/Speed0_25x"

    let _speed05xButton =
        lazy this.GetNode<Button> "TopLeftPanel/TopLeftVBox/SpeedMultiplierHBox/Speed0_5x"

    let _speed1xButton =
        lazy this.GetNode<Button> "TopLeftPanel/TopLeftVBox/SpeedMultiplierHBox/Speed1x"

    let _speed2xButton =
        lazy this.GetNode<Button> "TopLeftPanel/TopLeftVBox/SpeedMultiplierHBox/Speed2x"

    let _speed3xButton =
        lazy this.GetNode<Button> "TopLeftPanel/TopLeftVBox/SpeedMultiplierHBox/Speed3x"

    let _speed4xButton =
        lazy this.GetNode<Button> "TopLeftPanel/TopLeftVBox/SpeedMultiplierHBox/Speed4x"

    let onTabBarTabClicked (tab: int64) =
        _playerInfosGrid.Value.Hide()
        _gameSpeedHBox.Value.Hide()

        if _tabBar.Value.CurrentTab = int tab then
            match int tab with
            | 0 -> _playerInfosGrid.Value.Show()
            | 1 -> _gameSpeedHBox.Value.Show()
            | _ -> ()

        // 通过设置 Size 使得 PanelContainer 大小重新刷新
        _topLeftPanel.Value.Size <- Vector2(10f, 10f)

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

    /// 清空玩家统计中第一行以外的其他标签（暂时用不到了）
    let clearPlayStat () =
        _playerInfosGrid.Value.GetChildren() |> Seq.skip 4 |> Seq.iter _.QueueFree()

    let onGameSpeedButtonPressed speed () =
        _globalNode.Value.IdleStrategyEntry.Value.ChangeGameSpeed speed

    override this._Ready() =
        _tabBar.Value.add_TabClicked onTabBarTabClicked

        _pauseButton.Value.add_Pressed <| onGameSpeedButtonPressed 0
        _speed025xButton.Value.add_Pressed <| onGameSpeedButtonPressed 0.25
        _speed05xButton.Value.add_Pressed <| onGameSpeedButtonPressed 0.5
        _speed1xButton.Value.add_Pressed <| onGameSpeedButtonPressed 1.0
        _speed2xButton.Value.add_Pressed <| onGameSpeedButtonPressed 2.0
        _speed3xButton.Value.add_Pressed <| onGameSpeedButtonPressed 3.0
        _speed4xButton.Value.add_Pressed <| onGameSpeedButtonPressed 4.0

        // 默认显示玩家统计界面
        _playerInfosGrid.Value.Show()
        _gameSpeedHBox.Value.Hide()
        // 默认速度 1x
        _speed1xButton.Value.ButtonPressed <- true
        // 通过设置 Size 使得 PanelContainer 刷新大小为本地化后字符长度，否则会因为默认待替换的本地化字符串长度太长导致它宽度太大
        _topLeftPanel.Value.Size <- Vector2(10f, 10f)

        _globalNode.Value.IdleStrategyEntry.Value.PlayerStatUpdated
        |> ObservableSyncContextUtil.subscribePost onPlayerStatUpdated
