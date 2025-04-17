using Apps.Contexts;
using Apps.Events;
using Apps.Nodes;
using Commons.Utils.HexSphereGrid;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Planets;
using Domains.Repos.PlanetGenerates;
using Domains.Services.PlanetGenerates;
using Godot;

namespace Apps.Applications.Uis.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 20:45:21
public class MiniMapManagerApp(
    IPlanetConfig planetConfig,
    ITileRepo tileRepo,
    IPointRepo pointRepo,
    ITileService tileService
) : IMiniMapManagerApp
{
    #region 上下文节点

    private IMiniMapManager? _miniMapManager;
    private IHexPlanetManager? _hexPlanetManager;
    private IOrbitCamera? _orbitCamera;

    public bool NodeReady { get; set; }
    public void OnReady()
    {
        NodeReady = true;
        // 初始化上下文节点
        _miniMapManager = NodeContext.Instance.GetSingleton<IMiniMapManager>()!;
        _hexPlanetManager = NodeContext.Instance.GetSingleton<IHexPlanetManager>()!;
        _orbitCamera = NodeContext.Instance.GetSingleton<IOrbitCamera>()!;
        // 绑定事件
        _hexPlanetManager.NewPlanetGenerated += InitMiniMap;
        tileRepo.RefreshTerrainShader += RefreshTile;
        OrbitCameraEvent.Instance.Moved += SyncCameraIconPos;
    }

    private void InitMiniMap() => _miniMapManager!.Init(_orbitCamera!.GetFocusBasePos());

    private void RefreshTile(int tileId)
    {
        var tile = tileRepo.GetById(tileId)!;
        var sphereAxial = pointRepo.GetSphereAxial(tile);
        _miniMapManager!.TerrainLayer!.SetCell(sphereAxial.Coords.ToVector2I(), 0, TerrainAtlas(tile));
    }

    // 同步相机标志的位置
    private void SyncCameraIconPos(Vector3 pos, float delta)
    {
        var tileId = tileService.SearchNearestTileId(pos);
        if (tileId == null)
        {
            GD.PrintErr($"未找到摄像机对应地块：{pos}");
            return;
        }

        var sa = pointRepo.GetSphereAxial(tileRepo.GetById((int)tileId)!);
        // TODO: 缓动，以及更精确的位置转换
        _miniMapManager!.CameraIcon!.GlobalPosition = _miniMapManager.TerrainLayer!.ToGlobal(
            _miniMapManager.TerrainLayer.MapToLocal(sa.Coords.ToVector2I()));
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

    public void OnProcess(double delta)
    {
        throw new NotImplementedException();
    }

    public void OnExitTree()
    {
        NodeReady = false;
        // 解绑事件
        _hexPlanetManager!.NewPlanetGenerated -= InitMiniMap;
        tileRepo.RefreshTerrainShader -= RefreshTile;
        OrbitCameraEvent.Instance.Moved -= SyncCameraIconPos;
        // 上下文节点置空
        _hexPlanetManager = null;
        _miniMapManager = null;
        _orbitCamera = null;
    }

    #endregion

    // 标准摄像机对应 Divisions = 10
    private static readonly Vector2 StandardCamPos = new(-345, 75);
    private static readonly Vector2 StandardCamZoom = new(0.4f, 0.4f);

    public void UpdateCamera()
    {
        _miniMapManager!.Camera!.Position = StandardCamPos / 10 * planetConfig.Divisions;
        _miniMapManager.Camera.Zoom = StandardCamZoom * 10 / planetConfig.Divisions;
    }

    public void Init(Vector3 orbitCamPos)
    {
        SyncCameraIconPos(orbitCamPos, 0f);
        UpdateCamera();
        _miniMapManager!.TerrainLayer!.Clear();
        _miniMapManager.ColorLayer!.Clear();
        foreach (var tile in tileRepo.GetAll())
        {
            var sphereAxial = pointRepo.GetSphereAxial(tile);
            _miniMapManager.TerrainLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, TerrainAtlas(tile));
            switch (sphereAxial.Type)
            {
                case SphereAxial.TypeEnum.PoleVertices or SphereAxial.TypeEnum.MidVertices:
                    _miniMapManager.ColorLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, new Vector2I(2, 3));
                    break;
                case SphereAxial.TypeEnum.EdgesSpecial
                    when sphereAxial.TypeIdx % 6 == 0 || sphereAxial.TypeIdx % 6 == 5:
                    _miniMapManager.ColorLayer.SetCell(sphereAxial.Coords.ToVector2I(), 0, EdgeAtlas(sphereAxial));
                    break;
            }
        }
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
}