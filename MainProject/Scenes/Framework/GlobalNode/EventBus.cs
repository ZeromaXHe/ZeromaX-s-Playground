using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;

public class EventBus
{
    public static EventBus Instance { get; } = new();

    public delegate void CameraMovedEvent(Vector3 pos, float delta);
    public event CameraMovedEvent CameraMoved;
    public delegate void NewCameraDestinationEvent(Vector3 posDirection);
    public event NewCameraDestinationEvent NewCameraDestination;

    public static void EmitCameraMoved(Vector3 pos, float delta) => Instance.CameraMoved?.Invoke(pos, delta);
    public static void EmitNewCameraDestination(Vector3 posDir) => Instance.NewCameraDestination?.Invoke(posDir);
}