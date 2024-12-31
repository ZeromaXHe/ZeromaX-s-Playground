using FrontEnd4IdleStrategyFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS;

public partial class DebugPanel : DebugPanelFS
{
    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
    public override void _Process(double delta) => base._Process(delta);
    public override void _Input(InputEvent @event) => base._Input(@event);
}