using FrontEndToolFS.SebastianPlanet;
using Godot;

namespace ZeromaXPlayground.game.SebastianPlanet.Settings;

[Tool]
[GlobalClass]
public partial class ShapeSettings : Resource, IShapeSettings
{
    [Export] public float PlanetRadius { get; set; } = 1f;
    [Export] public NoiseLayer[] NoiseLayers { get; set; }
    public INoiseLayer[] noiseLayers => NoiseLayers; // 协变数组转换
}