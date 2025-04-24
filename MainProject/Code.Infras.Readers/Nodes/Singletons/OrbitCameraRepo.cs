using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:23:35
public class OrbitCameraRepo : SingletonNodeRepo<IOrbitCamera>, IOrbitCameraRepo
{
    public event IOrbitCameraRepo.MovedEvent? Moved;
    private void OnMoved(Vector3 posDir, float delta) => Moved?.Invoke(posDir, delta);
    public event IOrbitCameraRepo.TransformedEvent? Transformed;
    private void OnTransformed(Transform3D transform, float delta) => Transformed?.Invoke(transform, delta);
    public event Action<float>? RadiusChanged;
    private void OnRadiusChanged(float radius) => RadiusChanged?.Invoke(radius);
    public event Action<float>? ZoomChanged;
    private void OnZoomChanged(float zoom) => ZoomChanged?.Invoke(zoom);

    protected override void ConnectNodeEvents()
    {
        Singleton!.Moved += OnMoved;
        Singleton.Transformed += OnTransformed;
        Singleton.RadiusChanged += OnRadiusChanged;
        Singleton.ZoomChanged += OnZoomChanged;
    }

    protected override void DisconnectNodeEvents()
    {
        Singleton!.Moved -= OnMoved;
        Singleton.Transformed -= OnTransformed;
        Singleton.RadiusChanged -= OnRadiusChanged;
        Singleton.ZoomChanged -= OnZoomChanged;
    }
}