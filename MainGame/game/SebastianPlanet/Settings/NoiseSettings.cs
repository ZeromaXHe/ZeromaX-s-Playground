using FrontEndToolFS.SebastianPlanet;
using Godot;

namespace ZeromaXPlayground.game.SebastianPlanet.Settings;

[Tool]
[GlobalClass]
public partial class NoiseSettings : Resource, INoiseSettings
{
    [Export] public FilterType FilterType { get; set; } = FilterType.Simple;
    [Export] public float Strength { get; set; } = 1f;
    [Export(PropertyHint.Range, "1, 8")] public int NumLayers { get; set; } = 1;
    [Export] public float BaseRoughness { get; set; } = 1f;

    [Export(PropertyHint.Range, "1.0, 10.0")]
    public float Roughness { get; set; } = 2f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Persistence { get; set; } = 0.5f;

    [Export] public Vector3 Center { get; set; } = Vector3.Zero;
    [Export] public float MinValue { get; set; }
    [Export] public float WeightMultiplier { get; set; } = 0.8f;
}