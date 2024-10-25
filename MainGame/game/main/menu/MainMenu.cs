using Godot;
using System;
using FrontEnd4IdleStrategyFS.Display;

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

        _languageOptionButton.ItemSelected += index => MainMenuFS.setLocale(_languageOptionButton, (int)index);

        _startButton.Pressed += () => MainMenuFS.onStartButtonPressed(this);
        _quitButton.Pressed += () => MainMenuFS.onQuitButtonPressed(this);
        _bilibiliButton.Pressed += () => MainMenuFS.onBilibiliButtonPressed();
        _gitHubButton.Pressed += () => MainMenuFS.onGitHubButtonPressed();

        MainMenuFS.setDefaultLocale(_languageOptionButton);
    }
}