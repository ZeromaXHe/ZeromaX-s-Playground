using Godot;
using ProjectFS.HexPlanet;

namespace ZeromaXsPlaygroundProject.Demo.HexPlanet;

[Tool]
public partial class IcosahedronVisual : IcosahedronVisualFS
{
    // 不能省略 partial 和 _Ready 这些，否则分部类生成代码不生效。请忽视 IDE 的省略提示
    public override void _Ready() => base._Ready();
}