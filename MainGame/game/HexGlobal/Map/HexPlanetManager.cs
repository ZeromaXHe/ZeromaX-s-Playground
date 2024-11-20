using FrontEnd4IdleStrategyFS.Display.HexGlobal;
using Godot;

namespace ZeromaXPlayground.game.HexGlobal.Map;

public partial class HexPlanetManager : HexPlanetManagerFS
{
    [Export]
    private bool Regenerate
    {
        get => _regenerate;
        set => _regenerate = value;
    }

    [Export(PropertyHint.Range, "1,10000")]
    public float PlanetRadius
    {
        get => _planetRadius;
        set => _planetRadius = value;
    }

    [Export(PropertyHint.Range, "0,7")]
    public int Subdivisions
    {
        get => _subdivisions;
        set => _subdivisions = value;
    }

    [Export(PropertyHint.Range, "0,6")]
    public int ChunkSubdivisions
    {
        get => _chunkSubdivisions;
        set => _chunkSubdivisions = value;
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int Octaves
    {
        get => _octaves;
        set => _octaves = value;
    }

    [Export(PropertyHint.Range, "0, 1")]
    public float Persistence
    {
        get => _persistence;
        set => _persistence = value;
    }

    [Export(PropertyHint.Range, "1, 10")]
    public float Lacunarity
    {
        get => _lacunarity;
        set => _lacunarity = value;
    }

    [Export]
    public float MinHeight
    {
        get => _minHeight;
        set => _minHeight = value;
    }

    [Export]
    public float MaxHeight
    {
        get => _maxHeight;
        set => _maxHeight = value;
    }

    [Export]
    public float NoiseScaling
    {
        get => _noiseScaling;
        set => _noiseScaling = value;
    }

    // 必须保留此处和 partial，请忽略 IDE 建议 
    public override void _Ready()
    {
        base._Ready();
    }
}