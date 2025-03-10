using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }

    [Signal]
    public delegate void NewPlanetGeneratedEventHandler();

    [Signal]
    public delegate void CameraMovedEventHandler(Vector3 pos, float delta);

    public override void _Ready()
    {
        Instance = this;
    }

    // 4.4 竟然多了自动生成的方法？好像原来没有？不过因为是 protected 的，所以要封装一下
    public static void EmitNewPlanetGenerated() => Instance.EmitSignalNewPlanetGenerated();
    public static void EmitCameraMoved(Vector3 pos, float delta) => Instance.EmitSignalCameraMoved(pos, delta);
}