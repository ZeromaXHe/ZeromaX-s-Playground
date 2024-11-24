using FrontEndToolFS.Tool;
using Godot;

public partial class HexMapEditor : HexMapEditorFS
{
    [Export]
    public Color[] Colors
    {
        get => _colors;
        set => _colors = value;
    }

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Input(InputEvent e)
    {
        base._Input(e);
    }
}