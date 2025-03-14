using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

namespace ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;

public class EventBus
{
    public static EventBus Instance { get; } = new();

    public delegate void CameraMovedEvent(Vector3 pos, float delta);

    public event CameraMovedEvent CameraMoved;

    public delegate void NewCameraDestinationEvent(Vector3 posDirection);

    public event NewCameraDestinationEvent NewCameraDestination;

    // 返回在显示特征事件时回传特征 MultiMesh ID
    public delegate int ShowFeatureEvent(Transform3D transform, FeatureType type, bool preview);

    public event ShowFeatureEvent ShowFeature;

    public delegate void HideFeatureEvent(int id, FeatureType type, bool preview);

    public event HideFeatureEvent HideFeature;

    public static void EmitCameraMoved(Vector3 pos, float delta) => Instance.CameraMoved?.Invoke(pos, delta);
    public static void EmitNewCameraDestination(Vector3 posDir) => Instance.NewCameraDestination?.Invoke(posDir);

    public static int EmitShowFeature(Transform3D transform, FeatureType type, bool preview) =>
        Instance.ShowFeature?.Invoke(transform, type, preview) ?? -1;

    public static void EmitHideFeature(int id, FeatureType type, bool preview) =>
        Instance.HideFeature?.Invoke(id, type, preview);
}