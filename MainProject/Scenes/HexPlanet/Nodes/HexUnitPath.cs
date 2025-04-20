using System.Collections.Generic;
using System.Linq;
using Contexts;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions.Addition;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-02 12:48
public partial class HexUnitPath : Path3D, IHexUnitPath
{
    public HexUnitPath() => InitServices();
    public bool Working { get; set; }

    #region 服务

    private static ITileService? _tileService;
    private static ITileRepo? _tileRepo;
    private static IHexPlanetManagerRepo? _hexPlanetManagerRepo;

    private static void InitServices()
    {
        _tileService ??= Context.GetBeanFromHolder<ITileService>();
        _tileRepo ??= Context.GetBeanFromHolder<ITileRepo>();
        _hexPlanetManagerRepo ??= Context.GetBeanFromHolder<IHexPlanetManagerRepo>();
    }

    #endregion

    #region on-ready 节点

    private PathFollow3D? _pathFollow;
    private RemoteTransform3D? _remoteTransform;
    private CsgPolygon3D? _view;

    private void InitOnReadyNodes()
    {
        _pathFollow = GetNode<PathFollow3D>("%PathFollow3D");
        _remoteTransform = GetNode<RemoteTransform3D>("%RemoteTransform3D");
        _view = GetNode<CsgPolygon3D>("%View");
    }

    #endregion

    public List<Tile>? Tiles { get; private set; }
    public List<float>? Progresses { get; private set; }

    public override void _Ready()
    {
        InitOnReadyNodes();
    }
    public NodeEvent? NodeEvent => null;

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
        _view!.Visible = true;
    }

    private void SetPathAndCurve(List<Tile> path)
    {
        Tiles = path;
        var keyPoints = new List<Vector3>();
        // 转换为曲线
        var curve = new Curve3D();
        var fromTile = path[0];
        var fromHeight = _hexPlanetManagerRepo!.GetHeight(fromTile);
        var fromCentroid = fromTile.GetCentroid(_hexPlanetManagerRepo.Radius + fromHeight);
        var toTile = path[1];
        var toHeight = _hexPlanetManagerRepo.GetHeight(toTile);
        var toCentroid = toTile.GetCentroid(_hexPlanetManagerRepo.Radius + toHeight);

        var fromIdx = fromTile.GetNeighborIdx(toTile);
        var toIdx = toTile.GetNeighborIdx(fromTile);
        var fromEdgeMid = fromTile.GetSolidEdgeMiddle(fromIdx, _hexPlanetManagerRepo.Radius + fromHeight);
        var toEdgeMid = toTile.GetSolidEdgeMiddle(toIdx, _hexPlanetManagerRepo.Radius + toHeight);
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
            toHeight = _hexPlanetManagerRepo.GetHeight(toTile);
            toCentroid = toTile.GetCentroid(_hexPlanetManagerRepo.Radius + toHeight);

            fromIdx = fromTile.GetNeighborIdx(toTile);
            toIdx = toTile.GetNeighborIdx(fromTile);
            fromEdgeMid = fromTile.GetSolidEdgeMiddle(fromIdx, _hexPlanetManagerRepo.Radius + fromHeight);
            toEdgeMid = toTile.GetSolidEdgeMiddle(toIdx, _hexPlanetManagerRepo.Radius + toHeight);
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
        _pathFollow!.ProgressRatio = 0;
        _remoteTransform!.SetRemoteNode(unit.GetPath());
        var duration = Curve.PointCount / 2.0 / MoveSpeedByTile;
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_pathFollow, PathFollow3D.PropertyName.ProgressRatio.ToString(), 1, duration);
        // _tween.Parallel().TweenMethod(Callable.From((Vector3 pos) => unit.AdjustMovingRotation(pos)), 0f, 1f, duration);
        tween.TweenCallback(Callable.From(() =>
        {
            Working = false;
            _view!.Visible = false;
            Tiles = null;
            _remoteTransform.SetRemoteNode(null);
            unit.FinishPath();
        }));
    }

    public float GetProgress() => _pathFollow!.Progress;

    public Tile GetProgressTile()
    {
        var idx = Progresses!.BinarySearch(_pathFollow!.Progress);
        if (idx < 0) idx = ~idx;
        return Tiles![idx];
    }
}