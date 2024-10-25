namespace FrontEnd4IdleStrategyFS.Display

open FSharp.Control.Reactive
open Godot

module MainMenuFS =
    let setLocale (languageOptionButton: OptionButton) index =
        let locale =
            match languageOptionButton.GetItemText index with
            | "中文" -> "zh"
            | _ -> "en"

        TranslationServer.SetLocale locale

    let onStartButtonPressed (mainMenu: Node) =
        mainMenu.GetTree().ChangeSceneToFile "res://game/inGame/menu/InGameMenu.tscn"

    let onQuitButtonPressed (mainMenu: Node) = mainMenu.GetTree().Quit()

    let onBilibiliButtonPressed () =
        OS.ShellOpen "https://space.bilibili.com/27867310"

    let onGitHubButtonPressed () =
        OS.ShellOpen "https://github.com/ZeromaXHe/ZeromaX-s-Playground"

    let setDefaultLocale (languageOptionButton: OptionButton) =
        let locale = OS.GetLocale()
        GD.Print(locale)

        languageOptionButton.Selected <-
            match locale with
            | l when l <> null && l.StartsWith "zh_" -> 0
            | _ -> 1

module InGameMenuFS =
    let onTabBarTabClicked (tabBar: TabBar) (playerInfosGrid: GridContainer) (tab: int) =
        if tab = 0 then
            if tabBar.CurrentTab = tab then
                playerInfosGrid.Show()
            else
                playerInfosGrid.Hide()

    let onRefreshTimerTimeout() =
        // TODO: 还在构思这里的实现，感觉得和 Reactive 一起改造
        failwith "not implemented"

