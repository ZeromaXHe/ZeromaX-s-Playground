using Godot;

namespace Apps.Events;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-12 15:06:58
public class OrbitCameraEvent
{
    public static OrbitCameraEvent Instance { get; } = new();

    public delegate void MovedEvent(Vector3 pos, float delta);

    public event MovedEvent? Moved;

    public delegate void TransformedEvent(Transform3D transform, float delta);

    public event TransformedEvent? Transformed;
    
    public delegate void NewDestinationEvent(Vector3 posDirection);

    public event NewDestinationEvent? NewDestination;
    
    public static void EmitMoved(Vector3 pos, float delta) => Instance.Moved?.Invoke(pos, delta);
    public static void EmitTransformed(Transform3D transform, float delta) => Instance.Transformed?.Invoke(transform, delta);
    public static void EmitNewDestination(Vector3 posDir) => Instance.NewDestination?.Invoke(posDir);
}