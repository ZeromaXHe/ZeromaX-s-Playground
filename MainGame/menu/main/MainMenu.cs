using Godot;
using System;

public partial class MainMenu : Control
{
    private OptionButton _languageOptionButton;

    public override void _Ready()
    {
        // on-ready
        _languageOptionButton = GetNode<OptionButton>("%LanguageOptionButton");

        _languageOptionButton.ItemSelected += index => SetLocale((int)index);

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