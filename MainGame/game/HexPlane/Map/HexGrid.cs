using FrontEndToolFS.Tool;
using Godot;

namespace ZeromaXPlayground.game.HexPlane.Map;

[Tool]
public partial class HexGrid : HexGridFS
{
    [Export]
    public PackedScene CellPrefab
    {
        get => cellPrefab;
        set => cellPrefab = value;
    }

    [Export]
    public PackedScene CellLabelPrefab
    {
        get => cellLabelPrefab;
        set => cellLabelPrefab = value;
    }

    [Export]
    public PackedScene ChunkPrefab
    {
        get => chunkPrefab;
        set => chunkPrefab = value;
    }

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
    [Export]
    public PackedScene UnitPrefab
    {
        get => unitPrefab;
        set => unitPrefab = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}