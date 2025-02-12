using Godot;
using ProjectFS.HexPlanet;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

[Tool]
public partial class HexPlanetManager : HexPlanetManagerFS
{
    [Export(PropertyHint.Range, "5, 1000")]
    public override float Radius { get; set; } = 10f;

    [Export(PropertyHint.Range, "1, 15")] public override int Subdivision { get; set; } = 4;

    [Export(PropertyHint.Range, "0.1f, 1f")]
    public override float HexSize { get; set; } = 1f;

    // 不能省略 partial 和 _Ready 这些，否则分部类生成代码不生效。请忽视 IDE 的省略提示
    public override void _Ready() => base._Ready();
    public override void _Process(double delta) => base._Process(delta);
}