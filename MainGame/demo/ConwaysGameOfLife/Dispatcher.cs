using Godot;
using FrontEnd4IdleStrategyFS.ConwaysGameOfLife;

namespace ZeromaXPlayground.demo.ConwaysGameOfLife;

public partial class Dispatcher : DispatcherFS
{
    [ExportGroup("Settings")]
    [Export(PropertyHint.Range, "1, 1000")]
    public int UpdateFrequency
    {
        get => updateFrequency;
        set => updateFrequency = value;
    }

    [Export]
    public bool AutoStart
    {
        get => autoStart;
        set => autoStart = value;
    }

    [Export]
    public Texture2D DataTexture
    {
        get => dataTexture;
        set => dataTexture = value;
    }

    [ExportGroup("Requirements")]
    [Export(PropertyHint.File)]
    public string ComputeShader
    {
        get => computeShader;
        set => computeShader = value;
    }

    [Export]
    public Sprite2D Renderer
    {
        get => renderer;
        set => renderer = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
    public override void _Input(InputEvent @event) => base._Input(@event);
    public override void _Notification(int what) => base._Notification(what);
}