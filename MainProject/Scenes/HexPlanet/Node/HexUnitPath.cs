using System;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class HexUnitPath : Path3D
{
    public bool Working { get; set; }

    private PathFollow3D _pathFollow;
    private RemoteTransform3D _remoteTransform;
    private CsgPolygon3D _view;

    public override void _Ready()
    {
        _pathFollow = GetNode<PathFollow3D>("%PathFollow3D");
        _remoteTransform = GetNode<RemoteTransform3D>("%RemoteTransform3D");
        _view = GetNode<CsgPolygon3D>("%View");
    }

    // 有以下问题的话，请检查 Curve3D 的生成是否不连续：
    //
    // 1. 如果出现莫名奇妙的报错：莫名其妙的 look_at 并且报错
    // 很可能是因为 Curve3D in out 参数配的不对，导致曲线不连续导致的？改小 in out 就不报错了
    // E 0:00:08:0190   looking_at: The target vector can't be zero.
    // <C++ 错误>       Condition "p_target.is_zero_approx()" is true. Returning: Basis()
    // <C++ 源文件>      core/math/basis.cpp:1044 @ looking_at()
    //
    // 2. 移动过程中拿不到当前 Position，返回是无限的 NaN。而且 Basis 也是 NaN。原因应该就是上面导致的。
    // 奇怪的是赋值给单位后却可以使得他真的按那个位置移动，效果和使用 RemoteTransform3D 一样。
    // 但问题也是一样的：RemoteTransform3D 也是一样会使得 _unit.Position.IsFinite() 返回 true。
    public void TaskStart(Curve3D curve)
    {
        Curve = curve;
        _view.Visible = true;
    }

    public void TaskFinished()
    {
        Working = false;
        _view.Visible = false;
    }

    private const float MoveSpeedByTile = 3; // 每 1s 走的地块格数

    public void HandleMove(HexUnit unit)
    {
        _pathFollow.ProgressRatio = 0;
        _remoteTransform.SetRemoteNode(unit.GetPath());
        var duration = Curve.PointCount / 2.0 / MoveSpeedByTile;
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_pathFollow, PathFollow3D.PropertyName.ProgressRatio.ToString(), 1, duration);
        // _tween.Parallel().TweenMethod(Callable.From((Vector3 pos) => unit.AdjustMovingRotation(pos)), 0f, 1f, duration);
        tween.TweenCallback(Callable.From(() =>
        {
            Working = false;
            unit.FinishPath();
            _remoteTransform.SetRemoteNode(null);
        }));
    }

    public float GetProgress() => _pathFollow.Progress;
}