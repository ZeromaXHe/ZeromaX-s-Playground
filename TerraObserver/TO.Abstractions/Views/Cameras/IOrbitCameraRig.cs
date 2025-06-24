using Godot;
using Godot.Abstractions.Bases;

namespace TO.Abstractions.Views.Cameras;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-03 17:38:03
public interface IOrbitCameraRig : INode3D
{
    #region 事件

    event Action<float>? Processed;
    event Action? ZoomChanged;

    delegate void MovedEvent(Vector3 pos, float delta);

    event MovedEvent? Moved;
    void EmitMoved(Vector3 pos, float delta);

    delegate void TransformedEvent(Transform3D transform, float delta);

    event TransformedEvent? Transformed;
    void EmitTransformed(Transform3D trans, float delta);

    #endregion

    #region Export

    float StickMinZoom { get; }
    float StickMaxZoom { get; }
    float SwivelMinZoom { get; }
    float SwivelMaxZoom { get; }
    float MoveSpeedMinZoom { get; }
    float MoveSpeedMaxZoom { get; }
    float RotationSpeed { get; }
    Node3D? Sun { get; }
    float AutoPilotSpeed { get; }

    #endregion

    #region on-ready

    Node3D FocusBase { get; }
    CsgBox3D FocusBox { get; }
    Node3D FocusBackStick { get; }
    CsgBox3D BackBox { get; }
    SpotLight3D Light { get; }
    Node3D Swivel { get; }
    Node3D Stick { get; }
    RemoteTransform3D CamRig { get; }

    #endregion

    #region 普通属性

    float Zoom { get; set; }
    float AntiStuckSpeedMultiplier { get; set; }
    float AutoPilotProgress { get; set; }
    Vector3 FromDirection { get; set; }
    Vector3 DestinationDirection { get; set; }

    #endregion
}