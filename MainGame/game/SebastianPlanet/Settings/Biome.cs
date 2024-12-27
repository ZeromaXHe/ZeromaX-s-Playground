using FrontEndToolFS.SebastianPlanet;
using Godot;

namespace ZeromaXPlayground.game.SebastianPlanet.Settings;

[Tool]
[GlobalClass]
public partial class Biome : Resource, IBiome
{
    [Export] public Gradient Gradient { get; set; }
    [Export] public Color Tint { get; set; } = Colors.White;
    [Export] public float StartHeight { get; set; }
    [Export] public float TintPercent { get; set; }
}