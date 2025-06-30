using System;
using Godot;
using TO.Domains.Types.Cameras;

namespace TerraObserver.Scenes.Cameras.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// 由于需要作为上下文的 Export 变量，必须添加 [Tool] 注解
[Tool]
public partial class OrbitCameraRig : Node3D, IOrbitCameraRig
{
    #region 事件

    public event Action<float>? Processed;
    public event Action? ZoomChanged;

    public delegate void MovedEvent(Vector3 pos, float delta);

    public event MovedEvent? Moved;
    public void EmitMoved(Vector3 pos, float delta) => Moved?.Invoke(pos, delta);

    public delegate void TransformedEvent(Transform3D transform, float delta);

    public event TransformedEvent? Transformed;
    public void EmitTransformed(Transform3D trans, float delta) => Transformed?.Invoke(trans, delta);

    #endregion

    #region Export 属性

    [Export] public Camera3D? Camera { get; set; } // 设置摄像机节点

    [Export(PropertyHint.Range, "0.01, 10")]
    public float StickMinZoom { get; set; } = 1f;

    [Export(PropertyHint.Range, "0.01, 10")]
    public float StickMaxZoom { get; set; } = 0.2f;

    [Export(PropertyHint.Range, "-180, 180")]
    public float SwivelMinZoom { get; set; } = -90f;

    [Export(PropertyHint.Range, "-180, 180")]
    public float SwivelMaxZoom { get; set; } = -45f;

    [Export(PropertyHint.Range, "0.01, 10")]
    public float MoveSpeedMinZoom { get; set; } = 0.8f;

    [Export(PropertyHint.Range, "0.01, 10")]
    public float MoveSpeedMaxZoom { get; set; } = 0.2f;

    [Export(PropertyHint.Range, "0.01, 3600, degrees")]
    public float RotationSpeed { get; set; } = 180f;

    [Export] public Node3D? Sun { get; set; }

    [ExportGroup("自动导航设置")]
    // 自动导航速度。该值的倒数，对应自动移动时间：比如 2f 对应 0.5s 抵达
    [Export(PropertyHint.Range, "0.01, 10")]
    public float AutoPilotSpeed { get; set; } = 1f;

    #endregion

    #region 普通属性

    public float Zoom
    {
        get => _zoom;
        set
        {
            _zoom = value;
            if (!_ready) return;
            ZoomChanged?.Invoke();
        }
    }

    private float _zoom = 1f;
    public float AntiStuckSpeedMultiplier { get; set; } = 1f; // 用于防止速度过低的时候相机卡死
    public float AutoPilotProgress { get; set; }
    public Vector3 FromDirection { get; set; } = Vector3.Zero;
    public Vector3 DestinationDirection { get; set; } = Vector3.Zero;

    #endregion

    #region on-ready

    public Node3D FocusBase { get; private set; } = null!;
    public CsgBox3D FocusBox { get; private set; } = null!;
    public Node3D FocusBackStick { get; private set; } = null!;
    public CsgBox3D BackBox { get; private set; } = null!;
    public SpotLight3D Light { get; private set; } = null!;
    public Node3D Swivel { get; private set; } = null!;
    public Node3D Stick { get; private set; } = null!;
    public RemoteTransform3D CamRig { get; private set; } = null!; // 对应相机应该在的位置

    #endregion

    #region 生命周期

    private bool _ready;

    public override void _Ready()
    {
        FocusBase = GetNode<Node3D>("%FocusBase");
        FocusBox = GetNode<CsgBox3D>("%FocusBox");
        FocusBackStick = GetNode<Node3D>("%FocusBackStick");
        BackBox = GetNode<CsgBox3D>("%BackBox");
        Light = GetNode<SpotLight3D>("%Light");
        Swivel = GetNode<Node3D>("%Swivel");
        Stick = GetNode<Node3D>("%Stick");
        CamRig = GetNode<RemoteTransform3D>("%CamRig");
        if (Camera != null)
            CamRig.RemotePath = CamRig.GetPathTo(Camera);
        _ready = true;
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            SetProcess(false);
            return;
        }

        Processed?.Invoke((float)delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Engine.IsEditorHint()) return;
        // 缩放
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.WheelDown or MouseButton.WheelUp } e)
        {
            var zoomDelta = 0.025f * e.Factor * (e.ButtonIndex == MouseButton.WheelUp ? 1f : -1f);
            Zoom = Mathf.Clamp(Zoom + zoomDelta, 0f, 1f);
            Transformed?.Invoke(CamRig.GlobalTransform, (float)GetProcessDeltaTime());
        }
    }

    #endregion
}