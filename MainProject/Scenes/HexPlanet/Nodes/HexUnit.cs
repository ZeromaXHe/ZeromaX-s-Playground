using Apps.Queries.Abstractions.Tiles;
using Commons.Utils;
using Contexts;
using Domains.Models.Entities.Civs;
using Godot;
using GodotNodes.Abstractions.Addition;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.Civs;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-01 12:45
public partial class HexUnit : CsgBox3D, IHexUnit
{
    public HexUnit()
    {
        InitServices();
        var unit = _unitRepo!.Add();
        Id = unit.Id;
    }

    public NodeEvent? NodeEvent => null;

    #region 服务与存储

    private static ITileRepo? _tileRepo;
    private static ITileShaderApplication? _tileShaderApplication;
    private static IUnitRepo? _unitRepo;
    private static IHexPlanetManagerRepo? _hexPlanetManagerRepo;

    private static void InitServices()
    {
        _tileRepo ??= Context.GetBeanFromHolder<ITileRepo>();
        _tileShaderApplication ??= Context.GetBeanFromHolder<ITileShaderApplication>();
        _unitRepo ??= Context.GetBeanFromHolder<IUnitRepo>();
        _hexPlanetManagerRepo ??= Context.GetBeanFromHolder<IHexPlanetManagerRepo>();
    }

    #endregion

    public int Id { get; private set; }
    private Vector3 _beginRotation;
    private int _tileId;

    public int TileId
    {
        get => _tileId;
        set
        {
            if (_tileId > 0)
            {
                var preTile = _tileRepo!.GetById(_tileId)!;
                _tileShaderApplication!.DecreaseVisibility(preTile, Unit.VisionRange);
                _tileRepo.SetUnitId(preTile, 0);
            }

            _tileId = value;
            _unitRepo!.GetById(Id)!.TileId = _tileId;
            ValidateLocation();
            var tile = _tileRepo!.GetById(_tileId)!;
            _tileShaderApplication!.IncreaseVisibility(tile, Unit.VisionRange);
            _tileRepo.SetUnitId(tile, Id);
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

    private HexUnitPath? _path;
    private int _pathTileIdx;
    private bool _pathOriented;
    private const float PathRotationSpeed = Mathf.Pi;
    private const float PathMoveSpeed = 30f; // 每秒走 30f 标准距离

    public override void _Process(double delta)
    {
        if (_path == null) return;
        var deltaProgress = (float)delta * _hexPlanetManagerRepo!.StandardScale * PathMoveSpeed;
        if (_pathOriented)
        {
            var prePathTileIdx = _pathTileIdx;
            var progress = _path.GetProgress();
            while (_pathTileIdx < _path.Progresses.Count && _path.Progresses[_pathTileIdx] < progress)
                _pathTileIdx++;
            if (prePathTileIdx != _pathTileIdx)
            {
                _tileShaderApplication!.DecreaseVisibility(_path.Tiles![prePathTileIdx], Unit.VisionRange);
                _tileShaderApplication.IncreaseVisibility(_path.Tiles[_pathTileIdx], Unit.VisionRange);
            }

            var before = _path.Curve.SampleBaked(progress - deltaProgress, true);
            Node3dUtil.AlignYAxisToDirection(this, Position, alignForward: before.DirectionTo(Position));
        }
        else
        {
            var forward = Position.DirectionTo(_path.Curve.SampleBaked(deltaProgress, true));
            var angle = Math3dUtil.GetPlanarAngle(-Basis.Z, forward, Position, true);
            var deltaAngle = float.Sign(angle) * PathRotationSpeed * (float)delta;
            if (Mathf.Abs(deltaAngle) >= Mathf.Abs(angle))
            {
                Rotate(Position.Normalized(), angle);
                _pathOriented = true;
                _path.StartMove(this);
            }
            else
                Rotate(Position.Normalized(), deltaAngle);
        }
    }

    public void Travel(HexUnitPath path)
    {
        _path = path;
        _pathOriented = false;
        _pathTileIdx = 0;
        // 提前把实际单位数据设置到目标 Tile 中
        var fromTile = _tileRepo!.GetById(_tileId)!;
        _tileRepo.SetUnitId(fromTile, 0);
        var toTile = _path.Tiles![^1];
        _tileRepo.SetUnitId(toTile, Id);
        _unitRepo!.GetById(Id)!.TileId = toTile.Id;
        _tileId = toTile.Id;
    }

    public void FinishPath()
    {
        _path = null;
    }

    public void ValidateLocation()
    {
        var tile = _tileRepo!.GetById(_tileId)!;
        var position = tile.GetCentroid(_hexPlanetManagerRepo!.Radius + _hexPlanetManagerRepo.GetHeight(tile));
        Node3dUtil.PlaceOnSphere(this, position, _hexPlanetManagerRepo.StandardScale,
            alignForward: Vector3.Up); // 单位不需要抬高，场景里已经设置好了
        _beginRotation = Rotation;
    }

    public void Die()
    {
        _unitRepo!.Delete(Id);
        var tile = _tileRepo!.GetById(_tileId)!;
        _tileShaderApplication!.DecreaseVisibility(tile, Unit.VisionRange);
        _tileRepo.SetUnitId(tile, 0);
        QueueFree();
    }
}