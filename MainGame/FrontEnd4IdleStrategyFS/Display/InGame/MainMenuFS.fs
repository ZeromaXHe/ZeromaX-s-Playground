namespace FrontEnd4IdleStrategyFS.Display.InGame

open Godot
open FrontEnd4IdleStrategyFS.Global

type MainMenuFS() as this =
    inherit Control()

    let _globalNode = lazy this.GetNode<GlobalNodeFS> "/root/GlobalNode"

    let _languageOptionButton = lazy this.GetNode<OptionButton> "%LanguageOptionButton"

    let _startButton =
        lazy this.GetNode<Button> "CenterContainer/VBox/ButtonVBox/StartButton"

    let _quitButton =
        lazy this.GetNode<Button> "CenterContainer/VBox/ButtonVBox/QuitButton"

    let _hexGlobalButton =
        lazy this.GetNode<Button> "CenterContainer/VBox/ButtonVBox/HexGlobalButton"

    let _bilibiliButton =
        lazy this.GetNode<Button> "LbMarginContainer/VBox/BilibiliButton"

    let _gitHubButton = lazy this.GetNode<Button> "LbMarginContainer/VBox/GitHubButton"

    let onLanguageOptionButtonItemSelected (index: int64) =
        let itemText = int index |> _languageOptionButton.Value.GetItemText

        if itemText = "中文" then "zh" else "en"
        |> TranslationServer.SetLocale

    let onQuitButtonPressed () = this.GetTree().Quit()
    
    let onBilibiliButtonPressed () =
        OS.ShellOpen "https://space.bilibili.com/27867310" |> ignore

    let onGitHubButtonPressed () =
        OS.ShellOpen "https://github.com/ZeromaXHe/ZeromaX-s-Playground" |> ignore

    let setDefaultLocale () =
        let locale = OS.GetLocale()
        GD.Print(locale) // zh_CN
        _languageOptionButton.Value.Selected <- if locale <> null && locale.StartsWith "zh_" then 0 else 1

    override this._Ready() =
        _languageOptionButton.Value.add_ItemSelected onLanguageOptionButtonItemSelected
        _startButton.Value.add_Pressed (fun _ -> _globalNode.Value.ChangeToIdleStrategyScene())
        _quitButton.Value.add_Pressed onQuitButtonPressed
        _hexGlobalButton.Value.add_Pressed (fun _ -> _globalNode.Value.ChangeToHexGlobalScene())
        _bilibiliButton.Value.add_Pressed onBilibiliButtonPressed
        _gitHubButton.Value.add_Pressed onGitHubButtonPressed
        setDefaultLocale ()
