using Godot;
using System;

public partial class MainMenu : Control
{
    private GlobalNode _globalNode;
    
    private OptionButton _languageOptionButton;
    private Button _startButton;
    private Button _quitButton;

    private Button _bilibiliButton;
    private Button _gitHubButton;

    public override void _Ready()
    {
        // on-ready
        _globalNode = GetNode<GlobalNode>("/root/GlobalNode");
        _languageOptionButton = GetNode<OptionButton>("%LanguageOptionButton");
        _startButton = GetNode<Button>("CenterContainer/VBox/ButtonVBox/StartButton");
        _quitButton = GetNode<Button>("CenterContainer/VBox/ButtonVBox/QuitButton");
        _bilibiliButton = GetNode<Button>("LbMarginContainer/VBox/BilibiliButton");
        _gitHubButton = GetNode<Button>("LbMarginContainer/VBox/GitHubButton");

        _languageOptionButton.ItemSelected += OnLanguageOptionButtonItemSelected;

        _startButton.Pressed += _globalNode.ChangeToIdleStrategyScene;
        _quitButton.Pressed += OnQuitButtonPressed;
        _bilibiliButton.Pressed += OnBilibiliButtonPressed;
        _gitHubButton.Pressed += OnGitHubButtonPressed;

        SetDefaultLocale();
    }

    private void OnLanguageOptionButtonItemSelected(long index)
    {
        var itemText = _languageOptionButton.GetItemText((int)index);
        var locale = "中文".Equals(itemText) ? "zh" : "en";
        TranslationServer.SetLocale(locale);
    }
    
    private void OnQuitButtonPressed() =>
        GetTree().Quit();
    
    private static void OnBilibiliButtonPressed() =>
        OS.ShellOpen("https://space.bilibili.com/27867310");
    
    private static void OnGitHubButtonPressed() =>
        OS.ShellOpen("https://github.com/ZeromaXHe/ZeromaX-s-Playground");

    private void SetDefaultLocale()
    {
        var locale = OS.GetLocale();
        GD.Print(locale); // zh_CN
        if (locale != null && locale.StartsWith("zh_"))
            _languageOptionButton.Selected = 0;
        else
            _languageOptionButton.Selected = 1;
    }
}