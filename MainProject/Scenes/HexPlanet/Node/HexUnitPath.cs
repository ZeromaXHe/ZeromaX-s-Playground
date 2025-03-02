using System;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class HexUnitPath : Path3D
{
    public bool Working { get; set; }

    private CsgPolygon3D _view;
    
    public override void _Ready()
    {
        _view = GetNode<CsgPolygon3D>("%View");
    }

    // Godot 的这个 PathFollow3D 真难用…… 放弃用它实现
    // 1. 莫名奇妙的报错：貌似它在莫名其妙的 look_at 并且报错
    // （测试出来好像是因为 Curve3D in out 参数配的不对，导致曲线不连续导致的？改小 in out 就不报错了）
    // E 0:00:08:0190   looking_at: The target vector can't be zero.
    // <C++ 错误>       Condition "p_target.is_zero_approx()" is true. Returning: Basis()
    // <C++ 源文件>      core/math/basis.cpp:1044 @ looking_at()
    // 2. 移动过程中拿不到当前位置，返回是无限的。而且 Basis 也是 NaN
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
}