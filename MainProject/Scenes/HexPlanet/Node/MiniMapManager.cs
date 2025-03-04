using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class MiniMapManager : Node2D
{
    #region on-ready 节点

    private TileMapLayer _terrainLayer;
    private TileMapLayer _colorLayer;
    private Camera2D _camera;

    private void InitOnReadyNodes()
    {
        _terrainLayer = GetNode<TileMapLayer>("%TerrainLayer");
        _colorLayer = GetNode<TileMapLayer>("%ColorLayer");
        _camera = GetNode<Camera2D>("%Camera2D");
    }

    #endregion

    #region 服务

    private ITileService _tileService;

    private void InitServices()
    {
        _tileService = Context.GetBean<ITileService>();
        _tileService.RefreshTerrainShader += RefreshTile;
    }

    #endregion

    public override void _Ready()
    {
        InitOnReadyNodes();
        InitServices();
    }

    // 标准摄像机对应 Divisions = 10
    private static readonly Vector2 StandardCamPos = new(-345, 75);
    private static readonly Vector2 StandardCamZoom = new(0.4f, 0.4f);

    public void UpdateCamera()
    {
        _camera.Position = StandardCamPos / 10 * HexMetrics.Divisions;
        _camera.Zoom = StandardCamZoom * 10 / HexMetrics.Divisions;
    }

    public void Init()
    {
        UpdateCamera();
        _terrainLayer.Clear();
        _colorLayer.Clear();
        foreach (var tile in _tileService.GetAll())
        {
            var sphereAxial = _tileService.GetSphereAxial(tile);
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
        var tile = _tileService.GetById(tileId);
        var sphereAxial = _tileService.GetSphereAxial(tile);
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
        if (tile.IsUnderwater)
            return tile.WaterLevel - tile.Elevation > 1 ? new Vector2I(0, 1) : new Vector2I(1, 1);
        return tile.TerrainTypeIndex switch
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