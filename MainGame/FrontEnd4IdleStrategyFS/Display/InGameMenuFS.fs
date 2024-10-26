namespace FrontEnd4IdleStrategyFS.Display

module InGameMenuFS =
    open Godot

    let onTabBarTabClicked (tabBar: TabBar) (playerInfosGrid: GridContainer) (tab: int) =
        if tab = 0 then
            if tabBar.CurrentTab = tab then
                playerInfosGrid.Show()
            else
                playerInfosGrid.Hide()

    let onRefreshTimerTimeout() =
        // TODO: 还在构思这里的实现，感觉得和 Reactive 一起改造
        failwith "not implemented"


