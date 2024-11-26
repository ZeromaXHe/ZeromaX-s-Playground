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

    [Export]
    public Color DefaultColor
    {
        get => _defaultColor;
        set => _defaultColor = value;
    }

    [Export]
    public Color TouchedColor
    {
        get => _touchedColor;
        set => _touchedColor = value;
    }

    [Export]
    public Texture2D NoiseSource
    {
        get => _noiseSource;
        set => _noiseSource = value;
    }

    public override void _Ready()
    {
        base._Ready();
    }

    // public override void _Input(InputEvent e)
    // {
    //     base._Input(e);
    // }
}