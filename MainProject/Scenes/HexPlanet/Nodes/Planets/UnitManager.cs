using Domains.Models.Entities.PlanetGenerates;
using Domains.Repos.Civs;
using Domains.Repos.PlanetGenerates;
using Domains.Services.Navigations;
using Domains.Services.Uis;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-27 10:39:20
public partial class UnitManager : Node3D
{
    public UnitManager() => InitServices();

    [Export] private PackedScene? _unitScene;

    private readonly System.Collections.Generic.Dictionary<int, HexUnit> _units = new();

    #region 服务和存储

    private IUnitRepo? _unitRepo;
    private ITileRepo? _tileRepo;
    private ITileSearchService? _tileSearchService;
    private ISelectViewService? _selectViewService;

    private void InitServices()
    {
        _unitRepo = Context.GetBeanFromHolder<IUnitRepo>();
        _tileRepo = Context.GetBeanFromHolder<ITileRepo>();
        _tileRepo.UnitValidateLocation += OnTileServiceUnitValidateLocation;
        _tileSearchService = Context.GetBeanFromHolder<ITileSearchService>();
        _selectViewService = Context.GetBeanFromHolder<ISelectViewService>();
    }

    private void OnTileServiceUnitValidateLocation(int unitId) => _units[unitId].ValidateLocation();

    private void CleanEventListeners()
    {
        // 不小心忽视了事件的解绑，会在编辑器下"重载已保存场景"时出问题报错！
        // 【切记】所以这里需要在退出场景树时清理事件监听！！！
        _tileRepo!.UnitValidateLocation -= OnTileServiceUnitValidateLocation;
    }

    #endregion

    #region on-ready 节点

    private HexUnitPathPool? _hexUnitPathPool;

    private void InitOnReadyNodes()
    {
        _hexUnitPathPool = GetNode<HexUnitPathPool>("%HexUnitPathPool");
    }

    #endregion

    private int _pathFromTileId;

    public int PathFromTileId
    {
        get => _pathFromTileId;
        set
        {
            _pathFromTileId = value;
            if (_pathFromTileId == 0)
                _selectViewService!.ClearPath();
        }
    }

    public override void _Ready() => InitOnReadyNodes();
    public override void _ExitTree() => CleanEventListeners();

    public void AddUnit(int tileId, float orientation)
    {
        var unit = _unitScene!.Instantiate<HexUnit>();
        AddChild(unit);
        _units[unit.Id] = unit;
        unit.TileId = tileId;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(int unitId)
    {
        _units[unitId].Die();
        _units.Remove(unitId);
    }

    public void ClearAllUnits()
    {
        foreach (var unit in _units.Values)
            unit.Die();
        _units.Clear();
        _unitRepo!.Truncate();
    }

    public void FindPath(Tile? tile)
    {
        if (PathFromTileId != 0)
        {
            if (tile == null || tile.Id == PathFromTileId)
            {
                // 重复点选同一地块，则取消选择
                PathFromTileId = 0;
            }
            else MoveUnit(tile);
        }
        else
            // 当前没有选择地块（即没有选中单位）的话，则在有单位时选择该地块
            PathFromTileId = tile == null || tile.UnitId == 0 ? 0 : tile.Id;
    }

    private void MoveUnit(Tile toTile)
    {
        var fromTile = _tileRepo!.GetById(PathFromTileId)!;
        var path = _tileSearchService!.FindPath(fromTile, toTile, true);
        if (path is { Count: > 1 })
        {
            // 确实有找到从出发点到 tile 的路径
            var unit = _units[fromTile.UnitId];
            _hexUnitPathPool!.NewTask(unit, path);
        }

        PathFromTileId = 0;
    }
}