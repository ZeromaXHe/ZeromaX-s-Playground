using Godot;
using System;

public partial class MainMenu : Control
{
    private OptionButton _languageOptionButton;
    private Button _startButton;
    private Button _quitButton;

    private Button _bilibiliButton;
    private Button _gitHubButton;

    public override void _Ready()
    {
        // on-ready
        _languageOptionButton = GetNode<OptionButton>("%LanguageOptionButton");
        _startButton = GetNode<Button>("CenterContainer/VBox/ButtonVBox/StartButton");
        _quitButton = GetNode<Button>("CenterContainer/VBox/ButtonVBox/QuitButton");
        _bilibiliButton = GetNode<Button>("LbMarginContainer/VBox/BilibiliButton");
        _gitHubButton = GetNode<Button>("LbMarginContainer/VBox/GitHubButton");

        _languageOptionButton.ItemSelected += index => SetLocale((int)index);

        _startButton.Pressed += OnStartButtonPressed;
        _quitButton.Pressed += OnQuitButtonPressed;
        _bilibiliButton.Pressed += OnBilibiliButtonPressed;
        _gitHubButton.Pressed += OnGitHubButtonPressed;

        string locale = OS.GetLocale();
        GD.Print(locale); // zh_CN
        if (locale != null && locale.StartsWith("zh_"))
        {
            _languageOptionButton.Selected = 0;
        }
        else
        {
            _languageOptionButton.Selected = 1;
        }
    }

    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://game/inGame/menu/InGameMenu.tscn");
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
    
    private void OnBilibiliButtonPressed()
    {
        OS.ShellOpen("https://space.bilibili.com/27867310");
    }
    
    private void OnGitHubButtonPressed()
    {
        OS.ShellOpen("https://github.com/ZeromaXHe/ZeromaX-s-Playground");
    }

    private void SetLocale(int itemIndex)
    {
        string itemText = _languageOptionButton.GetItemText(itemIndex);
        if ("中文".Equals(itemText))
        {
            TranslationServer.SetLocale("zh");
        }
        else
        {
            TranslationServer.SetLocale("en");
        }
    }
}