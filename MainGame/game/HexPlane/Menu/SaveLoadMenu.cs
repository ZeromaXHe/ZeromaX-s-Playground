using FrontEndToolFS.Tool;
using Godot;

public partial class SaveLoadMenu : SaveLoadMenuFS
{
    [Export]
    public HexGridFS HexGrid
    {
        get => hexGrid;
        set => hexGrid = value;
    }

    [Export]
    public PackedScene ItemPrefab
    {
        get => itemPrefab;
        set => itemPrefab = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}