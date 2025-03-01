using System;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class HexUnitPath : Path3D
{
    public bool Working { get; set; }

    private RemoteTransform3D _remoteTransform;
    private PathFollow3D _pathFollow;

    private Tween _tween;
    private HexUnit _unit;
    private Action _onFinished;

    public override void _Ready()
    {
        _remoteTransform = GetNode<RemoteTransform3D>("%RemoteTransform3D");
        _pathFollow = GetNode<PathFollow3D>("%PathFollow3D");
    }

    // Godot 的这个 PathFollow3D 真难用……
    // 1. 莫名奇妙的报错：貌似它在莫名其妙的 look_at 并且报错
    // E 0:00:08:0190   looking_at: The target vector can't be zero.
    // <C++ 错误>       Condition "p_target.is_zero_approx()" is true. Returning: Basis()
    // <C++ 源文件>      core/math/basis.cpp:1044 @ looking_at()
    // 2. 移动过程中拿不到当前位置，返回是无限的。而且 Basis 也是 NaN
    // 奇怪的是赋值给单位后却可以使得他真的按那个位置移动，效果和使用 RemoteTransform3D 一样。
    // 但问题也是一样的：RemoteTransform3D 也是一样会使得 _unit.Position.IsFinite() 返回 true。
    // public override void _Process(double delta)
    // {
    //     if (!Working) return;
    //     var duration = Curve.PointCount / 2.0; // 每 1s 走一格
    //     _pathFollow.ProgressRatio += (float)(delta / duration);
    //     if (!_pathFollow.GlobalPosition.IsFinite())
    //         _unit.Position = _pathFollow.GlobalPosition;
    //     if (_pathFollow.ProgressRatio >= 1)
    //     {
    //         Working = false;
    //         _unit.FinishPath();
    //         _unit = null;
    //         _remoteTransform.SetRemoteNode(null);
    //         _onFinished.Invoke();
    //     }
    // }

    public void NewTask(HexUnit unit, Curve3D curve, Action onFinished)
    {
        Position = unit.Position;
        _onFinished = onFinished;
        _pathFollow.ProgressRatio = 0;
        _remoteTransform.SetRemoteNode(unit.GetPath());
        SetCurve(curve);
        var duration = curve.PointCount / 2.0; // 每 1s 走一格
        _unit = unit;
        _unit.StartPath();
        _tween = GetTree().CreateTween();
        _tween.TweenProperty(_pathFollow, PathFollow3D.PropertyName.ProgressRatio.ToString(), 1, duration);
        // _tween.Parallel().TweenMethod(Callable.From((Vector3 pos) => unit.AdjustMovingRotation(pos)), 0f, 1f, duration);
        _tween.TweenCallback(Callable.From(() =>
        {
            Working = false;
            _unit.FinishPath();
            _unit = null;
            _remoteTransform.SetRemoteNode(null);
            onFinished.Invoke();
        }));
    }
}