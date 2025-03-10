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
        // 编辑器里的全局自动加载节点好像 _Ready 比常规节点慢？
        // 会导致节点 _Ready 的时候发出的信号会失败（Instance 为空），现在暂时这些时候判空，为空会被截断（发不出去）
        Instance = this;
        GD.Print("Signal Bus _Ready");
    }

    // 4.4 竟然多了自动生成的方法？好像原来没有？不过因为是 protected 的，所以要封装一下
    public static void EmitNewPlanetGenerated() => Instance?.EmitSignalNewPlanetGenerated();
    public static void EmitCameraMoved(Vector3 pos, float delta) => Instance?.EmitSignalCameraMoved(pos, delta);
}