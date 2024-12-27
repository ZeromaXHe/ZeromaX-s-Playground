using FrontEndToolFS.SebastianPlanet;
using Godot;

namespace ZeromaXPlayground.game.SebastianPlanet.Settings;

[Tool]
[GlobalClass]
public partial class BiomeColorSettings : Resource, IBiomeColorSettings
{
    [Export] public Biome[] Biomes { get; set; }
    public IBiome[] biomes => Biomes; // 数组协变
    [Export] public NoiseSettings Noise { get; set; }
    public INoiseSettings noise => Noise;
    [Export] public float NoiseOffset { get; set; }
    [Export] public float NoiseStrength { get; set; }
    [Export] public float BlendAmount { get; set; }
}