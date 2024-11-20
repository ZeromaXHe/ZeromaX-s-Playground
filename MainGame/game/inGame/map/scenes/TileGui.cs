using FrontEnd4IdleStrategyFS.Display.InGame;

namespace ZeromaXPlayground.game.inGame.map.scenes;

public partial class TileGui : TileGuiFS
{
    // IDE 提示可以省略，但其实不行。还有 partial，得这样保证 Godot 分部类正常编译。
    public override void _Ready()
    {
        base._Ready();
    }
}