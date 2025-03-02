using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class HexUnit : CsgBox3D
{
    public HexUnit()
    {
        InitServices();
        var unit = _unitService.Add();
        Id = unit.Id;
    }

    #region 服务

    private static ITileService _tileService;
    private static IUnitService _unitService;

    private static void InitServices()
    {
        _tileService ??= Context.GetBean<ITileService>();
        _unitService ??= Context.GetBean<IUnitService>();
    }

    #endregion

    public int Id { get; set; }
    private Vector3 _beginRotation;
    private int _tileId;

    public int TileId
    {
        get => _tileId;
        set
        {
            if (_tileId > 0)
            {
                var preTile = _tileService.GetById(_tileId);
                _tileService.SetUnitId(preTile, 0);
            }

            _tileId = value;
            ValidateLocation();
            var tile = _tileService.GetById(_tileId);
            _tileService.SetUnitId(tile, Id);
        }
    }

    private float _orientation;

    // 朝向（弧度制）
    public float Orientation
    {
        get => _orientation;
        set
        {
            _orientation = value;
            Rotation = _beginRotation;
            Rotate(Position.Normalized(), _orientation);
        }
    }

    private HexUnitPath _path;
    private int _pathToTileId;
    private bool _pathOriented;
    private const float PathRotationSpeed = Mathf.Pi;
    private const float PathMoveSpeed = 30f; // 每秒走 30f 标准距离

    public override void _Process(double delta)
    {
        if (_path == null) return;
        var deltaProgress = (float)delta * HexMetrics.StandardScale * PathMoveSpeed;
        if (_pathOriented)
        {
            var before = _path.Curve.SampleBaked(_path.GetProgress() - deltaProgress, true);
            Node3dUtil.AlignYAxisToDirection(this, Position, alignForward: before.DirectionTo(Position));
        }
        else
        {
            var forward = Position.DirectionTo(_path.Curve.SampleBaked(deltaProgress, true));
            var angle = Math3dUtil.GetPlanarAngle(-Basis.Z, forward, Position, true);
            var deltaAngle = float.Sign(angle) * PathRotationSpeed * (float)delta;
            if (Mathf.Abs(deltaAngle) > Mathf.Abs(angle))
            {
                Rotate(Position.Normalized(), angle);
                _pathOriented = true;
                _path.HandleMove(this);
            }
            else
                Rotate(Position.Normalized(), deltaAngle);
        }
    }

    public void StartPath(HexUnitPath path, int toTileId)
    {
        _path = path;
        _pathToTileId = toTileId;
        _pathOriented = false;
    }

    public void FinishPath()
    {
        GD.Print($"Unit {Id} arrived at Tile {_pathToTileId}");
        var forward = -Basis.Z;
        TileId = _pathToTileId;
        Orientation = Math3dUtil.GetPlanarAngle(-Basis.Z, forward, Position, true);
        _path.TaskFinished();
        _path = null;
        _pathToTileId = 0;
    }

    public void ValidateLocation()
    {
        var tile = _tileService.GetById(_tileId);
        var position = tile.GetCentroid(HexMetrics.Radius + _tileService.GetHeight(tile));
        Node3dUtil.PlaceOnSphere(this, position, alignForward: Vector3.Up); // 单位不需要抬高，场景里已经设置好了
        _beginRotation = Rotation;
    }

    public void Die()
    {
        _unitService.Delete(Id);
        _tileService.SetUnitId(_tileService.GetById(_tileId), 0);
        QueueFree();
    }
}