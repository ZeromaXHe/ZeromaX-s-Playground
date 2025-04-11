using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-05 12:21
public partial class MiniMapManager : Node2D
{
    #region on-ready 节点

    private TileMapLayer _terrainLayer;
    private TileMapLayer _colorLayer;
    private Camera2D _camera;
    private Sprite2D _cameraIcon;

    private void InitOnReadyNodes()
    {
        _terrainLayer = GetNode<TileMapLayer>("%TerrainLayer");
        _colorLayer = GetNode<TileMapLayer>("%ColorLayer");
        _camera = GetNode<Camera2D>("%Camera2D");
        _cameraIcon = GetNode<Sprite2D>("%CameraIcon");
    }

    #endregion

    #region 服务与存储

    private ITileService _tileService;
    private ITileRepo _tileRepo;
    private IPointRepo _pointRepo;
    private IPlanetSettingService _planetSettingService;

    private void InitServices()
    {
        _tileService = Context.GetBean<ITileService>();
        _tileRepo = Context.GetBean<ITileRepo>();
        _tileRepo.RefreshTerrainShader += RefreshTile;
        _pointRepo = Context.GetBean<IPointRepo>();
        _planetSettingService = Context.GetBean<IPlanetSettingService>();
    }

    private void CleanEventListeners() =>
        _tileRepo.RefreshTerrainShader -= RefreshTile;

    #endregion

    public override void _Ready()
    {
        InitOnReadyNodes();
        InitServices();
        EventBus.Instance.CameraMoved += SyncCameraIconPos;
        GD.Print("MiniMapManager _Ready");
    }

    public override void _ExitTree()
    {
        CleanEventListeners();
        EventBus.Instance.CameraMoved -= SyncCameraIconPos;
    }

    // 同步相机标志的位置
    private void SyncCameraIconPos(Vector3 pos, float delta)
    {
        var tileId = _tileService.SearchNearestTileId(pos);
        if (tileId == null)
        {
            GD.PrintErr($"未找到摄像机对应地块：{pos}");
            return;
        }

        var sa = _pointRepo.GetSphereAxial(_tileRepo.GetById((int)tileId));
        // TODO: 缓动，以及更精确的位置转换
        _cameraIcon.GlobalPosition = _terrainLayer.ToGlobal(
            _terrainLayer.MapToLocal(sa.Coords.ToVector2I()));
    }

    public void ClickOnMiniMap()
    {
        var mousePos = _terrainLayer.GetLocalMousePosition();
        var mapVec = _terrainLayer.LocalToMap(mousePos);
        var sa = new SphereAxial(mapVec.X, mapVec.Y);
        if (!sa.IsValid()) return;
        EventBus.EmitNewCameraDestination(sa.ToLongitudeAndLatitude().ToDirectionVector3());
    }

    // 标准摄像机对应 Divisions = 10
    private static readonly Vector2 StandardCamPos = new(-345, 75);
    private static readonly Vector2 StandardCamZoom = new(0.4f, 0.4f);

    public void UpdateCamera()
    {
        _camera.Position = StandardCamPos / 10 * _planetSettingService.Divisions;
        _camera.Zoom = StandardCamZoom * 10 / _planetSettingService.Divisions;
    }

    public void Init(Vector3 orbitCamPos)
    {
        SyncCameraIconPos(orbitCamPos, 0f);
        UpdateCamera();
        _terrainLayer.Clear();
        _colorLayer.Clear();
        foreach (var tile in _tileRepo.GetAll())
        {
            var sphereAxial = _pointRepo.GetSphereAxial(tile);
            _terrainLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, TerrainAtlas(tile));
            switch (sphereAxial.Type)
            {
                case SphereAxial.TypeEnum.PoleVertices or SphereAxial.TypeEnum.MidVertices:
                    _colorLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, new Vector2I(2, 3));
                    break;
                case SphereAxial.TypeEnum.EdgesSpecial
                    when sphereAxial.TypeIdx % 6 == 0 || sphereAxial.TypeIdx % 6 == 5:
                    _colorLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, EdgeAtlas(sphereAxial));
                    break;
            }
        }
    }

    private void RefreshTile(int tileId)
    {
        var tile = _tileRepo.GetById(tileId);
        var sphereAxial = _pointRepo.GetSphereAxial(tile);
        _terrainLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, TerrainAtlas(tile));
    }

    private static Vector2I? EdgeAtlas(SphereAxial sphereAxial)
    {
        return sphereAxial.Column switch
        {
            0 => new Vector2I(0, 3),
            1 => new Vector2I(0, 2),
            2 => new Vector2I(1, 3),
            3 => new Vector2I(1, 2),
            4 => new Vector2I(2, 2),
            _ => null
        };
    }

    private static Vector2I? TerrainAtlas(Tile tile)
    {
        if (tile.Data.IsUnderwater)
            return tile.Data.WaterLevel - tile.Data.Elevation > 1 ? new Vector2I(0, 1) : new Vector2I(1, 1);
        return tile.Data.TerrainTypeIndex switch
        {
            0 => new Vector2I(3, 0), // 0 沙漠
            1 => new Vector2I(0, 0), // 1 草原
            2 => new Vector2I(2, 0), // 2 泥地
            3 => new Vector2I(3, 1), // 3 岩石
            4 => new Vector2I(2, 1), // 4 雪地
            _ => null
        };
    }
}