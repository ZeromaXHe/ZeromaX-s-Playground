using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class HexUnitPath : Path3D
{
    public HexUnitPath() => InitServices();
    public bool Working { get; set; }

    #region 服务

    private static ITileService _tileService;
    private static IPlanetSettingService _planetSettingService;

    private static void InitServices()
    {
        _tileService ??= Context.GetBean<ITileService>();
        _planetSettingService ??= Context.GetBean<IPlanetSettingService>();
    }

    #endregion

    #region on-ready 节点

    private PathFollow3D _pathFollow;
    private RemoteTransform3D _remoteTransform;
    private CsgPolygon3D _view;

    private void InitOnReadyNodes()
    {
        _pathFollow = GetNode<PathFollow3D>("%PathFollow3D");
        _remoteTransform = GetNode<RemoteTransform3D>("%RemoteTransform3D");
        _view = GetNode<CsgPolygon3D>("%View");
    }

    #endregion

    public List<Tile> Tiles { get; private set; }
    public List<float> Progresses { get; private set; }

    public override void _Ready()
    {
        InitOnReadyNodes();
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
    public void TaskStart(List<Tile> path)
    {
        SetPathAndCurve(path);
        _view.Visible = true;
    }

    private void SetPathAndCurve(List<Tile> path)
    {
        Tiles = path;
        var keyPoints = new List<Vector3>();
        // 转换为曲线
        var curve = new Curve3D();
        var fromTile = path[0];
        var fromHeight = _tileService.GetHeight(fromTile);
        var fromCentroid = fromTile.GetCentroid(_planetSettingService.Radius + fromHeight);
        var toTile = path[1];
        var toHeight = _tileService.GetHeight(toTile);
        var toCentroid = toTile.GetCentroid(_planetSettingService.Radius + toHeight);

        var fromIdx = fromTile.GetNeighborIdx(toTile);
        var toIdx = toTile.GetNeighborIdx(fromTile);
        var fromEdgeMid = _tileService.GetSolidEdgeMiddle(fromTile, fromIdx, _planetSettingService.Radius + fromHeight);
        var toEdgeMid = _tileService.GetSolidEdgeMiddle(toTile, toIdx, _planetSettingService.Radius + toHeight);
        // 需要注意下面 in out 入参 / 2f 的操作，用于避免 in out 入参太长（前后相交于 centroid），导致 Curve3D 不连续
        curve.AddPoint(fromCentroid, @out: (fromEdgeMid - fromCentroid) / 2f);
        curve.AddPoint(fromEdgeMid, (fromCentroid - fromEdgeMid) / 2f, (toEdgeMid - fromEdgeMid) / 2f);
        curve.AddPoint(toEdgeMid, (fromEdgeMid - toEdgeMid) / 2f, (toCentroid - toEdgeMid) / 2f);
        var keyPoint = fromEdgeMid.Lerp(toEdgeMid, 0.5f);
        keyPoints.Add(keyPoint);
        for (var i = 1; i < path.Count - 1; i++)
        {
            fromTile = toTile;
            fromHeight = toHeight;
            fromCentroid = toCentroid;

            toTile = path[i + 1];
            toHeight = _tileService.GetHeight(toTile);
            toCentroid = toTile.GetCentroid(_planetSettingService.Radius + toHeight);

            fromIdx = fromTile.GetNeighborIdx(toTile);
            toIdx = toTile.GetNeighborIdx(fromTile);
            fromEdgeMid = _tileService.GetSolidEdgeMiddle(fromTile, fromIdx, _planetSettingService.Radius + fromHeight);
            toEdgeMid = _tileService.GetSolidEdgeMiddle(toTile, toIdx, _planetSettingService.Radius + toHeight);
            curve.AddPoint(fromEdgeMid, (fromCentroid - fromEdgeMid) / 2f, (toEdgeMid - fromEdgeMid) / 2f);
            curve.AddPoint(toEdgeMid, (fromEdgeMid - toEdgeMid) / 2f, (toCentroid - toEdgeMid) / 2f);
            keyPoint = fromEdgeMid.Lerp(toEdgeMid, 0.5f);
            keyPoints.Add(keyPoint);
        }

        curve.AddPoint(toCentroid, (toEdgeMid - toCentroid) / 2f);
        Curve = curve;
        // 处理路径地块间的关键分割点
        Progresses = keyPoints.Select(Curve.GetClosestOffset).ToList();
    }

    private const float MoveSpeedByTile = 3; // 每 1s 走的地块格数

    public void StartMove(HexUnit unit)
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
            _view.Visible = false;
            Tiles = null;
            _remoteTransform.SetRemoteNode(null);
            unit.FinishPath();
        }));
    }

    public float GetProgress() => _pathFollow.Progress;

    public Tile GetProgressTile()
    {
        var idx = Progresses.BinarySearch(_pathFollow.Progress);
        if (idx < 0) idx = ~idx;
        return Tiles[idx];
    }
}