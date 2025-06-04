namespace Godot.Abstractions.Bases;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 16:51:16
public interface INode3D : INode
{
    bool Visible { get; set; }
    void Show();
    void Hide();
    Transform3D Transform { get; set; }
    Transform3D GlobalTransform { get; set; }
    Vector3 Position { get; set; }
    Vector3 Rotation { get; set; }
    Vector3 RotationDegrees { get; set; }
    Quaternion Quaternion { get; set; }
    Basis Basis { get; set; }
    Vector3 Scale { get; set; }
    Vector3 GlobalPosition { get; set; }
    Basis GlobalBasis { get; set; }

    World3D GetWorld3D();
    Vector3 ToLocal(Vector3 globalPoint);
    Vector3 ToGlobal(Vector3 localPoint);

    void Rotate(Vector3 axis, float angle);
    void GlobalRotate(Vector3 axis, float angle);
    void RotateX(float angle);
}