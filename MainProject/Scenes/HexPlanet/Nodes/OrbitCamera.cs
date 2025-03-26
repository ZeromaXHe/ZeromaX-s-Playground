using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-17 22:45
[Tool]
public partial class OrbitCamera : Node3D
{
    public OrbitCamera() => InitService();
    [Export] private Camera3D _camera; // 设置摄像机节点
    private float _radius = 10f;

    [Export(PropertyHint.Range, "0.01, 1000, or_greater")]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            if (!_ready) return;
            _focusBase.Position = Vector3.Forward * value * (1 + _planetSettingService.MaxHeightRatio);
            _focusBox.Size = Vector3.One * value * _boxSizeMultiplier * _planetSettingService.StandardScale;
            _backBox.Size = Vector3.One * value * _boxSizeMultiplier * _planetSettingService.StandardScale;
            _light.SpotRange = value * _lightRangeMultiplier;
            _light.Position = Vector3.Up * value * _lightRangeMultiplier * 0.5f;
        }
    }

    private float _boxSizeMultiplier = 0.01f;
    private float _focusBackZoom = 0.2f;
    private float _lightRangeMultiplier = 1f;

    [Export(PropertyHint.Range, "0.01, 10")]
    private float _stickMinZoom = 1f;

    [Export(PropertyHint.Range, "0.01, 10")]
    private float _stickMaxZoom = 0.2f;

    [Export(PropertyHint.Range, "-180, 180")]
    private float _swivelMinZoom = -90f;

    [Export(PropertyHint.Range, "-180, 180")]
    private float _swivelMaxZoom = -45f;

    [Export(PropertyHint.Range, "0.01, 10")]
    private float _moveSpeedMinZoom = 0.8f;

    [Export(PropertyHint.Range, "0.01, 10")]
    private float _moveSpeedMaxZoom = 0.2f;

    private float _antiStuckSpeedMultiplier = 1f; // 用于防止速度过低的时候相机卡死

    [Export(PropertyHint.Range, "0.01, 3600, degrees")]
    private float _rotationSpeed = 180f;

    [Export] private Node3D _sun;

    [ExportGroup("自动导航设置")]
    // 自动导航速度。该值的倒数，对应自动移动时间：比如 2f 对应 0.5s 抵达
    [Export(PropertyHint.Range, "0.01, 10")]
    public float AutoPilotSpeed { get; set; } = 1f;

    #region 服务

    private IPlanetSettingService _planetSettingService;

    private void InitService()
    {
        _planetSettingService = Context.GetBean<IPlanetSettingService>();
    }

    #endregion

    #region on-ready 节点

    private Node3D _focusBase;
    private CsgBox3D _focusBox;
    private Node3D _focusBackStick;
    private CsgBox3D _backBox;
    private SpotLight3D _light;
    private Node3D _swivel;
    private Node3D _stick;
    private RemoteTransform3D _camRig; // 对应相机应该在的位置

    private void InitOnReadyNodes()
    {
        _focusBase = GetNode<Node3D>("%FocusBase");
        _focusBox = GetNode<CsgBox3D>("%FocusBox");
        _focusBackStick = GetNode<Node3D>("%FocusBackStick");
        _backBox = GetNode<CsgBox3D>("%BackBox");
        _light = GetNode<SpotLight3D>("%Light");
        _swivel = GetNode<Node3D>("%Swivel");
        _stick = GetNode<Node3D>("%Stick");
        _camRig = GetNode<RemoteTransform3D>("%CamRig");
        if (_camera != null)
            _camRig.RemotePath = _camRig.GetPathTo(_camera);
        _ready = true;
    }

    #endregion

    private float _zoom = 1f;

    private float Zoom
    {
        get => _zoom;
        set
        {
            _zoom = value;
            if (!_ready) return;
            _focusBackStick.Position =
                _focusBackStick.Basis * Vector3.Back * Mathf.Lerp(0f,
                    _focusBackZoom * Radius * _planetSettingService.StandardScale, value);
            var distance = Mathf.Lerp(_stickMinZoom,
                _stickMaxZoom * _planetSettingService.StandardScale * 2f,
                value) * Radius;
            _stick.Position = Vector3.Back * distance;
            var angle = Mathf.Lerp(_swivelMinZoom, _swivelMaxZoom, value);
            _swivel.RotationDegrees = Vector3.Right * angle;
        }
    }

    private Vector3 _fromDirection = Vector3.Zero;
    private Vector3 _destinationDirection = Vector3.Zero;
    private float _autoPilotProgress;

    private bool _ready;

    public override void _Ready()
    {
        InitOnReadyNodes();
        if (!Engine.IsEditorHint())
        {
            EventBus.Instance.NewCameraDestination += SetAutoPilot;
            // 必须在 _ready = true 后面，触发各数据 setter 的初始化
            Reset();
        }

        GD.Print("OrbitCamera _Ready");
    }

    public override void _ExitTree()
    {
        if (!Engine.IsEditorHint())
            EventBus.Instance.NewCameraDestination -= SetAutoPilot;
    }

    public Vector3 GetFocusBasePos() => _focusBase.GlobalPosition;

    private void SetAutoPilot(Vector3 destinationDirection)
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
                LookAt(lookDir, _focusBase.GlobalBasis.Z);
                EventBus.EmitCameraMoved(GetFocusBasePos(), floatDelta);
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
            EventBus.EmitCameraTransformed(_camRig.GlobalTransform, floatDelta);

        // 根据相对于全局太阳光的位置，控制灯光亮度
        if (_sun == null)
            return;
        var lightSunAngle = _light.GlobalPosition.AngleTo(_sun.GlobalPosition);
        // 从 60 度开始到 120 度之间，灯光亮度逐渐从 0 增加到 1
        _light.LightEnergy = Mathf.Clamp((lightSunAngle - Mathf.Pi / 3) / (Mathf.Pi / 3), 0f, 0.1f);
    }

    private bool RotateCamera(float rotationDelta)
    {
        // 旋转
        if (rotationDelta == 0f)
            return false;
        _focusBackStick.RotateY(-Mathf.DegToRad(rotationDelta * _rotationSpeed));
        Zoom = _zoom; // 更新 FocusBackStick 方向
        return true;
    }

    private bool MoveCamera(float xDelta, float zDelta, float delta)
    {
        if (xDelta == 0f && zDelta == 0f)
            return false;
        var direction = (Vector3.Right * xDelta + Vector3.Back * zDelta).Normalized();
        var damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        var distance = Mathf.Lerp(_moveSpeedMinZoom, _moveSpeedMaxZoom, Zoom)
                       * Radius * _antiStuckSpeedMultiplier * damping * delta;
        var target = GetFocusBasePos() - GlobalPosition +
                     _focusBackStick.GlobalBasis * (direction * distance);
        // 现在在速度很慢，半径很大的时候，容易在南北极卡住（游戏开始后，只按 WS 即可走到南北极）
        // 所以检查一下按下移动键后，是否没能真正移动。如果没移动，则每帧放大速度 1.5 倍
        var prePos = GetFocusBasePos();
        LookAt(target, _focusBase.GlobalBasis.Z);
        _antiStuckSpeedMultiplier = prePos.IsEqualApprox(GetFocusBasePos())
            ? _antiStuckSpeedMultiplier * 1.5f
            : 1f;
        EventBus.EmitCameraMoved(GetFocusBasePos(), delta);
        // 打断自动跳转
        CancelAutoPilot();
        return true;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Engine.IsEditorHint()) return;
        // 缩放
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.WheelDown or MouseButton.WheelUp } e)
        {
            var zoomDelta = 0.025f * e.Factor * (e.ButtonIndex == MouseButton.WheelUp ? 1f : -1f);
            Zoom = Mathf.Clamp(Zoom + zoomDelta, 0f, 1f);
            EventBus.EmitCameraTransformed(_camRig.GlobalTransform, (float)GetProcessDeltaTime());
        }
    }

    public void Reset()
    {
        Radius = _planetSettingService.Radius;
        Zoom = 1f;
    }
}