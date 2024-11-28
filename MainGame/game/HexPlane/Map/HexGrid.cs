using Godot;
using FrontEndToolFS.Tool;

[Tool]
public partial class HexGrid : HexGridFS
{
    [Export]
    public int ChunkCountX
    {
        get => _chunkCountX;
        set => _chunkCountX = value;
    }

    [Export]
    public int ChunkCountZ
    {
        get => _chunkCountZ;
        set => _chunkCountZ = value;
    }

    [Export]
    public Color DefaultColor
    {
        get => _defaultColor;
        set => _defaultColor = value;
    }

    [Export]
    public Texture2D NoiseSource
    {
        get => _noiseSource;
        set => _noiseSource = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}