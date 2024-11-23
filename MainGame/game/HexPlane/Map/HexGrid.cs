using Godot;
using FrontEndToolFS.Tool;

[Tool]
public partial class HexGrid : HexGridFS
{
    [Export]
    public int Width
    {
        get => _width;
        set => _width = value;
    }

    [Export]
    public int Height
    {
        get => _height;
        set => _height = value;
    }

    public override void _Ready()
    {
        base._Ready();
    }
}