using FrontEndToolFS.SebastianPlanet;
using Godot;

namespace ZeromaXPlayground.game.SebastianPlanet.Settings;

[Tool]
[GlobalClass]
public partial class NoiseLayer : Resource, INoiseLayer
{
    [Export] public bool Enabled { get; set; } = true;
    [Export] public bool UseFirstLayerAsMask { get; set; }
    [Export] public NoiseSettings NoiseSettings { get; set; }
    public INoiseSettings noiseSettings => NoiseSettings;
}