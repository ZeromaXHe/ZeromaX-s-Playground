using Godot;
using System;

public partial class MainMenu : Control
{
    private OptionButton _languageOptionButton;
    private Button _startButton;
    private Button _quitButton;

    public override void _Ready()
    {
        // on-ready
        _languageOptionButton = GetNode<OptionButton>("%LanguageOptionButton");
        _startButton = GetNode<Button>("CenterContainer/VBox/ButtonVBox/StartButton");
        _quitButton = GetNode<Button>("CenterContainer/VBox/ButtonVBox/QuitButton");

        _languageOptionButton.ItemSelected += index => SetLocale((int)index);

        _startButton.Pressed += OnStartButtonPressed;
        _quitButton.Pressed += OnQuitButtonPressed;

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