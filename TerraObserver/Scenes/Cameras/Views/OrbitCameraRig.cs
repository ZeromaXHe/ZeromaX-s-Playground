using System;
using Godot;
using TO.Abstractions.Cameras;
using TO.Presenters.Views.Cameras;

namespace TerraObserver.Scenes.Cameras.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// 由于需要作为上下文的 Export 变量，必须添加 [Tool] 注解
[Tool]
public partial class OrbitCameraRig : OrbitCameraRigFS, IOrbitCameraRig
{
    #region 生命周期

    // 需要忽略 IDE 省略 partial、_Ready 等的提示，必须保留它们
    public override void _Ready() => base._Ready();
    public override void _Process(double delta) => base._Process(delta);
    public override void _UnhandledInput(InputEvent @event) => base._UnhandledInput(@event);

    #endregion

    #region 事件与 Export 属性

    public event Action<float>? Processed;
    public override void EmitProcessed(float delta) => Processed?.Invoke(delta);
    public event Action? ZoomChanged;
    public override void EmitZoomChanged() => ZoomChanged?.Invoke();
    public event IOrbitCameraRig.MovedEvent? Moved;
    public void EmitMoved(Vector3 pos, float delta) => Moved?.Invoke(pos, delta);
    public event IOrbitCameraRig.TransformedEvent? Transformed;
    public override void EmitTransformed(Transform3D trans, float delta) => Transformed?.Invoke(trans, delta);
    [Export] public override Camera3D? Camera { get; set; } // 设置摄像机节点

    [Export(PropertyHint.Range, "0.01, 10")]
    public override float StickMinZoom { get; set; } = 1f;

    [Export(PropertyHint.Range, "0.01, 10")]
    public override float StickMaxZoom { get; set; } = 0.2f;

    [Export(PropertyHint.Range, "-180, 180")]
    public override float SwivelMinZoom { get; set; } = -90f;

    [Export(PropertyHint.Range, "-180, 180")]
    public override float SwivelMaxZoom { get; set; } = -45f;

    [Export(PropertyHint.Range, "0.01, 10")]
    public override float MoveSpeedMinZoom { get; set; } = 0.8f;

    [Export(PropertyHint.Range, "0.01, 10")]
    public override float MoveSpeedMaxZoom { get; set; } = 0.2f;

    [Export(PropertyHint.Range, "0.01, 3600, degrees")]
    public override float RotationSpeed { get; set; } = 180f;

    [Export] public override Node3D? Sun { get; set; }

    [ExportGroup("自动导航设置")]
    // 自动导航速度。该值的倒数，对应自动移动时间：比如 2f 对应 0.5s 抵达
    [Export(PropertyHint.Range, "0.01, 10")]
    public override float AutoPilotSpeed { get; set; } = 1f;

    #endregion
}