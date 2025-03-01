using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
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

    public void ValidateLocation()
    {
        var tile = _tileService.GetById(_tileId);
        var position = tile.GetCentroid(HexMetrics.Radius + _tileService.GetHeight(tile));
        Node3dUtil.PlaceOnSphere(this, position, 0.5f * Size.Y, Vector3.Up);
        _beginRotation = Rotation;
    }

    public void Die()
    {
        _unitService.Delete(Id);
        _tileService.SetUnitId(_tileService.GetById(_tileId), 0);
        QueueFree();
    }
}