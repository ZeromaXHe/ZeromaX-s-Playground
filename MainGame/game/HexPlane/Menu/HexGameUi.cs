using FrontEndToolFS.Tool;
using Godot;

namespace ZeromaXPlayground.game.HexPlane.Menu;

public partial class HexGameUi : HexGameUiFS
{
    [Export]
    public HexGridFS Grid
    {
        get => grid;
        set => grid = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _UnhandledInput(InputEvent @event) => base._UnhandledInput(@event);
    public override void _Process(double delta) => base._Process(delta);
}