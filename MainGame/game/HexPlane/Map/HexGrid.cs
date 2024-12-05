using Godot;
using FrontEndToolFS.Tool;

[Tool]
public partial class HexGrid : HexGridFS
{
    [Export]
    public int CellCountX
    {
        get => cellCountX;
        set => cellCountX = value;
    }

    [Export]
    public int CellCountZ
    {
        get => cellCountZ;
        set => cellCountZ = value;
    }

    [Export]
    public Texture2D NoiseSource
    {
        get => _noiseSource;
        set => _noiseSource = value;
    }

    [Export(PropertyHint.Range, "0, 2147483647")]
    public int Seed
    {
        get => seed;
        set => seed = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}