using FrontEndToolFS.SebastianPlanet;
using Godot;

namespace ZeromaXPlayground.game.SebastianPlanet.Settings;

[Tool]
[GlobalClass]
public partial class ColorSettings : Resource, IColorSettings
{
    [Export] public ShaderMaterial PlanetMaterial { get; set; }
    [Export] public BiomeColorSettings BiomeColorSettings { get; set; }
    public IBiomeColorSettings biomeColorSettings => BiomeColorSettings;
    [Export] public Gradient OceanColor { get; set; }
}