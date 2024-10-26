namespace FrontEnd4IdleStrategyFS.Display

module MainMenuFS =
    open Godot

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
