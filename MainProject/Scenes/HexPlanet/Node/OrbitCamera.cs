using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class OrbitCamera : Node3D
{
    private float _radius = 10f;

    [Export]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            if (!_ready) return;
            _focusBase.Position = Vector3.Forward * value * (1 + HexMetrics.MaxHeightRatio);
            _focusBox.Size = Vector3.One * value * _boxSizeMultiplier * HexMetrics.StandardScale;
            _backBox.Size = Vector3.One * value * _boxSizeMultiplier * HexMetrics.StandardScale;
            _light.SpotRange = value * _lightRangeMultiplier;
            _light.Position = Vector3.Up * value * _lightRangeMultiplier * 0.5f;
        }
    }

    private float _boxSizeMultiplier = 0.01f;
    private float _focusBackZoom = 0.2f;
    private float _lightRangeMultiplier = 1f;

    [Export] private float _stickMinZoom = 1f;
    [Export] private float _stickMaxZoom = 0.2f;
    [Export] private float _swivelMinZoom = -90f;
    [Export] private float _swivelMaxZoom = -45f;
    [Export] private float _moveSpeedMinZoom = 0.8f;
    [Export] private float _moveSpeedMaxZoom = 0.2f;
    private float _antiStuckSpeedMultiplier = 1f; // 用于防止速度过低的时候相机卡死
    [Export] private float _rotationSpeed = 180f;
    [Export] private Node3D _sun;

    #region on-ready 节点

    private Node3D _focusBase;
    private CsgBox3D _focusBox;
    private Node3D _focusBackStick;
    private CsgBox3D _backBox;
    private SpotLight3D _light;
    private Node3D _swivel;
    private Node3D _stick;

    private void InitOnReadyNodes()
    {
        _focusBase = GetNode<Node3D>("%FocusBase");
        _focusBox = GetNode<CsgBox3D>("%FocusBox");
        _focusBackStick = GetNode<Node3D>("%FocusBackStick");
        _backBox = GetNode<CsgBox3D>("%BackBox");
        _light = GetNode<SpotLight3D>("%Light");
        _swivel = GetNode<Node3D>("%Swivel");
        _stick = GetNode<Node3D>("%Stick");
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
                _focusBackStick.Basis * Vector3.Back * Mathf.Lerp(0f, _focusBackZoom * Radius, value);
            var distance = Mathf.Lerp(_stickMinZoom, _stickMaxZoom, value) * Radius;
            _stick.Position = Vector3.Back * distance;
            var angle = Mathf.Lerp(_swivelMinZoom, _swivelMaxZoom, value);
            _swivel.RotationDegrees = Vector3.Right * angle;
        }
    }

    private bool _ready;

    public override void _Ready()
    {
        InitOnReadyNodes();
        if (!Engine.IsEditorHint())
            // 必须在 _ready = true 后面，触发各数据 setter 的初始化
            Reset();
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
        // 旋转
        var rotationDelta = floatDelta * Input.GetAxis("cam_rotate_left", "cam_rotate_right");
        RotateCamera(rotationDelta);
        // 移动
        var xDelta = Input.GetAxis("cam_move_left", "cam_move_right");
        var zDelta = Input.GetAxis("cam_move_forward", "cam_move_back");
        MoveCamera(xDelta, zDelta, floatDelta);
        if (Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            xDelta = -Input.GetLastMouseVelocity().X * MouseMoveSensitivity;
            zDelta = -Input.GetLastMouseVelocity().Y * MouseMoveSensitivity;
            MoveCamera(xDelta, zDelta, (float)delta);
        }

        // 根据相对于全局太阳光的位置，控制灯光亮度
        if (_sun == null)
            return;
        var lightSunAngle = _light.GlobalPosition.AngleTo(_sun.GlobalPosition);
        // 从 60 度开始到 120 度之间，灯光亮度逐渐从 0 增加到 1
        _light.LightEnergy = Mathf.Clamp((lightSunAngle - Mathf.Pi / 3) / (Mathf.Pi / 3), 0f, 1f);
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
        var target = _focusBase.GlobalPosition - GlobalPosition +
                     _focusBackStick.GlobalBasis * (direction * distance);
        // 现在在速度很慢，半径很大的时候，容易在南北极卡住（游戏开始后，只按 WS 即可走到南北极）
        // 所以检查一下按下移动键后，是否没能真正移动。如果没移动，则每帧放大速度 1.5 倍
        var prePos = _focusBase.GlobalPosition;
        LookAt(target, _focusBase.GlobalBasis.Z);
        _antiStuckSpeedMultiplier = prePos.IsEqualApprox(_focusBase.GlobalPosition)
            ? _antiStuckSpeedMultiplier * 1.5f
            : 1f;
        SignalBus.EmitCameraMoved(_focusBase.GlobalPosition, delta);
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
        }
    }

    public void Reset()
    {
        Radius = HexMetrics.Radius;
        Zoom = 1f;
    }
}