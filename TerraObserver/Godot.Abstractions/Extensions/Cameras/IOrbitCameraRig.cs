using Godot.Abstractions.Bases;

namespace Godot.Abstractions.Extensions.Cameras;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-03 17:38:03
public interface IOrbitCameraRig : INode3D
{
    delegate void MovedEvent(Vector3 pos, float delta);

    event MovedEvent? Moved;

    delegate void TransformedEvent(Transform3D transform, float delta);

    event TransformedEvent? Transformed;
    event Action<float>? RadiusChanged;
    event Action<float>? ZoomChanged;

    void SetRadius(float value, float maxHeightRatio, float standardScale);
    void SetZoom(float value, float standardScale);

    void SetAutoPilot(Vector3 destinationDirection);
    Vector3 GetFocusBasePos();
    void Reset(float radius);
}