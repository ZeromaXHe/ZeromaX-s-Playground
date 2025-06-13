using System;
using Godot;
using Godot.Abstractions.Extensions.Cameras;
using Godot.Abstractions.Extensions.Planets;

namespace TerraObserver.Scenes.Cameras.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// 由于需要作为上下文的 Export 变量，必须添加 [Tool] 注解
[Tool]
public partial class OrbitCameraRig : Node3D, IOrbitCameraRig
{
    #region 依赖

    public IPlanet Planet
    {
        get => _planet;
        set
        {
            _planet = value;
            Reset();
            _planet.ParamsChanged += OnPlanetParamsChanged;
            _planet.ParamsChanged += OnZoomChanged;
        }
    }

    private IPlanet _planet = null!;

    public void PreDelete()
    {
        if (Planet == null!)
            return;
        Planet.ParamsChanged -= OnPlanetParamsChanged;
        Planet.ParamsChanged -= OnZoomChanged;
    }

    #endregion

    #region 事件与 Export 属性

    public event IOrbitCameraRig.MovedEvent? Moved;
    public event IOrbitCameraRig.TransformedEvent? Transformed;
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

    #region 内部变量、属性

    private Node3D? _focusBase;
    private CsgBox3D? _focusBox;
    private Node3D? _focusBackStick;
    private CsgBox3D? _backBox;
    private SpotLight3D? _light;
    private Node3D? _swivel;
    private Node3D? _stick;
    private RemoteTransform3D? _camRig; // 对应相机应该在的位置

    private float Zoom
    {
        get => _zoom;
        set
        {
            _zoom = value;
            if (!_ready) return;
            OnZoomChanged();
        }
    }

    private float _zoom = 1f;

    private Vector3 _fromDirection = Vector3.Zero;
    private Vector3 _destinationDirection = Vector3.Zero;
    private float _autoPilotProgress;
    private float _antiStuckSpeedMultiplier = 1f; // 用于防止速度过低的时候相机卡死

    #endregion

    #region 生命周期

    private bool _ready;

    public override void _Ready()
    {
        _focusBase = GetNode<Node3D>("%FocusBase");
        _focusBox = GetNode<CsgBox3D>("%FocusBox");
        _focusBackStick = GetNode<Node3D>("%FocusBackStick");
        _backBox = GetNode<CsgBox3D>("%BackBox");
        _light = GetNode<SpotLight3D>("%Light");
        _swivel = GetNode<Node3D>("%Swivel");
        _stick = GetNode<Node3D>("%Stick");
        _camRig = GetNode<RemoteTransform3D>("%CamRig");
        if (Camera != null)
            _camRig.RemotePath = _camRig.GetPathTo(Camera);
        _ready = true;
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            SetProcess(false);
            return;
        }

        var floatDelta = (float)delta;
        var transformed = false; // 变换是否发生过改变
        // 相机自动跳转
        if (IsAutoPiloting())
        {
            // // 点击左键，打断相机自动跳转（不能这样写，因为点击小地图时左键也是按下的）
            // if (Input.IsMouseButtonPressed(MouseButton.Left))
            //     CancelAutoPilot();
            _autoPilotProgress += AutoPilotSpeed * floatDelta;
            var arrived = false;
            if (_autoPilotProgress >= 1f)
            {
                _autoPilotProgress = 1f;
                arrived = true;
            }

            var lookDir = _fromDirection.Slerp(_destinationDirection, _autoPilotProgress);
            // 有可能出现一帧内移动距离过短无法 LookAt 的情况
            if (!lookDir.IsEqualApprox(GetFocusBasePos().Normalized()))
            {
                LookAt(lookDir, _focusBase!.GlobalBasis.Z);
                Moved?.Invoke(GetFocusBasePos(), floatDelta);
                transformed = true;
            }

            // 抵达目的地，取消自动跳转
            if (arrived)
                CancelAutoPilot();
        }

        // 旋转
        var rotationDelta = floatDelta * Input.GetAxis("cam_rotate_left", "cam_rotate_right");
        transformed |= RotateCamera(rotationDelta);
        // 移动
        var xDelta = Input.GetAxis("cam_move_left", "cam_move_right");
        var zDelta = Input.GetAxis("cam_move_forward", "cam_move_back");
        transformed |= MoveCamera(xDelta, zDelta, floatDelta);
        if (Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            xDelta = -Input.GetLastMouseVelocity().X * MouseMoveSensitivity;
            zDelta = -Input.GetLastMouseVelocity().Y * MouseMoveSensitivity;
            transformed |= MoveCamera(xDelta, zDelta, (float)delta);
        }

        if (transformed)
            Transformed?.Invoke(_camRig!.GlobalTransform, floatDelta);

        // 根据相对于全局太阳光的位置，控制灯光亮度
        if (Sun == null)
            return;
        var lightSunAngle = _light!.GlobalPosition.AngleTo(Sun.GlobalPosition);
        // 从 60 度开始到 120 度之间，灯光亮度逐渐从 0 增加到 1
        _light.LightEnergy = Mathf.Clamp((lightSunAngle - Mathf.Pi / 3) / (Mathf.Pi / 3), 0f, 0.1f);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Engine.IsEditorHint()) return;
        // 缩放
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.WheelDown or MouseButton.WheelUp } e)
        {
            var zoomDelta = 0.025f * e.Factor * (e.ButtonIndex == MouseButton.WheelUp ? 1f : -1f);
            Zoom = Mathf.Clamp(Zoom + zoomDelta, 0f, 1f);
            Transformed?.Invoke(_camRig!.GlobalTransform, (float)GetProcessDeltaTime());
        }
    }

    #endregion

    public Vector3 GetFocusBasePos() => _focusBase!.GlobalPosition;

    public void SetAutoPilot(Vector3 destinationDirection)
    {
        var fromDirection = GetFocusBasePos().Normalized();
        if (fromDirection.IsEqualApprox(destinationDirection))
        {
            GD.Print("设置的自动跳转位置就是当前位置");
            return;
        }

        _fromDirection = fromDirection;
        _destinationDirection = destinationDirection;
        _autoPilotProgress = 0f;
    }

    private bool IsAutoPiloting() => _destinationDirection != Vector3.Zero;

    // 取消自动跳转目的地
    private void CancelAutoPilot()
    {
        _fromDirection = Vector3.Zero;
        _destinationDirection = Vector3.Zero;
        _autoPilotProgress = 0f;
    }

    private const float MouseMoveSensitivity = 0.01f;

    private bool RotateCamera(float rotationDelta)
    {
        // 旋转
        if (rotationDelta == 0f)
            return false;
        _focusBackStick!.RotateY(-Mathf.DegToRad(rotationDelta * RotationSpeed));
        Zoom = _zoom; // 更新 FocusBackStick 方向
        return true;
    }

    private bool MoveCamera(float xDelta, float zDelta, float delta)
    {
        if (xDelta == 0f && zDelta == 0f)
            return false;
        var direction = (Vector3.Right * xDelta + Vector3.Back * zDelta).Normalized();
        var damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        var distance = Mathf.Lerp(MoveSpeedMinZoom, MoveSpeedMaxZoom, Zoom)
                       * Planet.Radius * _antiStuckSpeedMultiplier * damping * delta;
        var target = GetFocusBasePos() - GlobalPosition +
                     _focusBackStick!.GlobalBasis * (direction * distance);
        // 现在在速度很慢，半径很大的时候，容易在南北极卡住（游戏开始后，只按 WS 即可走到南北极）
        // 所以检查一下按下移动键后，是否没能真正移动。如果没移动，则每帧放大速度 1.5 倍
        var prePos = GetFocusBasePos();
        LookAt(target, _focusBase!.GlobalBasis.Z);
        _antiStuckSpeedMultiplier = prePos.IsEqualApprox(GetFocusBasePos())
            ? _antiStuckSpeedMultiplier * 1.5f
            : 1f;
        Moved?.Invoke(GetFocusBasePos(), delta);
        // 打断自动跳转
        CancelAutoPilot();
        return true;
    }

    private float _boxSizeMultiplier = 0.01f;
    private float _focusBackZoom = 0.2f;
    private float _lightRangeMultiplier = 1f;

    private void OnZoomChanged()
    {
        _focusBackStick!.Position =
            _focusBackStick.Basis * Vector3.Back * Mathf.Lerp(0f,
                _focusBackZoom * Planet.Radius * Planet.StandardScale, Zoom);
        var distance = Mathf.Lerp(StickMinZoom,
            StickMaxZoom * Planet.StandardScale * 2f,
            Zoom) * Planet.Radius;
        _stick!.Position = Vector3.Back * distance;
        var angle = Mathf.Lerp(SwivelMinZoom, SwivelMaxZoom, Zoom);
        _swivel!.RotationDegrees = Vector3.Right * angle;
    }

    private void OnPlanetParamsChanged()
    {
        _focusBase!.Position = Vector3.Forward * (Planet.Radius + Planet.MaxHeight);
        _focusBox!.Size = Vector3.One * Planet.Radius * _boxSizeMultiplier * Planet.StandardScale;
        _backBox!.Size = Vector3.One * Planet.Radius * _boxSizeMultiplier * Planet.StandardScale;
        _light!.SpotRange = Planet.Radius * _lightRangeMultiplier;
        _light.Position = Vector3.Up * Planet.Radius * _lightRangeMultiplier * 0.5f;
    }

    public void Reset()
    {
        OnPlanetParamsChanged();
        Zoom = 1f;
    }
}