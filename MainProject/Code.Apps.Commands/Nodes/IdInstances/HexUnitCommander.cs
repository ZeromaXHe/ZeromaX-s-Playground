using Commons.Utils;
using Domains.Models.Entities.Civs;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.IdInstances;
using Domains.Services.Abstractions.Searches;
using Domains.Services.Abstractions.Shaders;
using Godot;
using Infras.Readers.Abstractions.Nodes.IdInstances;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.Civs;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Apps.Commands.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-27 16:02:07
public class HexUnitCommander
{
    private readonly IHexUnitService _hexUnitService;
    private readonly IHexUnitRepo _hexUnitRepo;

    private readonly ITileSearchService _tileSearchService;
    private readonly ITileShaderService _tileShaderService;
    private readonly IHexPlanetManagerRepo _hexPlanetManagerRepo;
    private readonly ITileRepo _tileRepo;
    private readonly IUnitRepo _unitRepo;

    public HexUnitCommander(IHexUnitService hexUnitService, IHexUnitRepo hexUnitRepo,
        ITileSearchService tileSearchService, ITileShaderService tileShaderService,
        IHexPlanetManagerRepo hexPlanetManagerRepo, ITileRepo tileRepo, IUnitRepo unitRepo)
    {
        _hexUnitService = hexUnitService;
        _hexUnitRepo = hexUnitRepo;
        _hexUnitRepo.Ready += OnReady;
        _hexUnitRepo.Processed += OnProcessed;
        _hexUnitRepo.TileIdChanged += OnTileIdChanged;
        _hexUnitRepo.Died += OnDied;

        _tileSearchService = tileSearchService;
        _tileShaderService = tileShaderService;
        _tileShaderService.RangeVisibilityIncreased += IncreaseVisibility;
        _hexPlanetManagerRepo = hexPlanetManagerRepo;
        _tileRepo = tileRepo;
        _unitRepo = unitRepo;
    }

    public void ReleaseEvents()
    {
        _hexUnitRepo.Ready -= OnReady;
        _hexUnitRepo.Processed -= OnProcessed;
        _hexUnitRepo.TileIdChanged -= OnTileIdChanged;
        _hexUnitRepo.Died -= OnDied;
        
        _tileShaderService.RangeVisibilityIncreased -= IncreaseVisibility;
    }

    private void OnReady(IHexUnit hexUnit)
    {
        var unit = _unitRepo.Add();
        hexUnit.Id = unit.Id;
    }

    private const float PathRotationSpeed = Mathf.Pi;
    private const float PathMoveSpeed = 30f; // 每秒走 30f 标准距离

    private void OnProcessed(IHexUnit unit, double delta)
    {
        if (unit.Path == null) return;
        var deltaProgress = (float)delta * _hexPlanetManagerRepo!.StandardScale * PathMoveSpeed;
        if (unit.PathOriented)
        {
            var prePathTileIdx = unit.PathTileIdx;
            var progress = unit.Path.GetProgress();
            while (unit.PathTileIdx < unit.Path.Progresses!.Count && unit.Path.Progresses[unit.PathTileIdx] < progress)
                unit.PathTileIdx++;
            if (prePathTileIdx != unit.PathTileIdx)
            {
                DecreaseVisibility(unit.Path.Tiles![prePathTileIdx], Unit.VisionRange);
                IncreaseVisibility(unit.Path.Tiles[unit.PathTileIdx], Unit.VisionRange);
            }

            var before = unit.Path.Curve.SampleBaked(progress - deltaProgress, true);
            Node3dUtil.AlignYAxisToDirection(unit, unit.Position, alignForward: before.DirectionTo(unit.Position));
        }
        else
        {
            var forward = unit.Position.DirectionTo(unit.Path.Curve.SampleBaked(deltaProgress, true));
            var angle = Math3dUtil.GetPlanarAngle(-unit.Basis.Z, forward, unit.Position, true);
            var deltaAngle = float.Sign(angle) * PathRotationSpeed * (float)delta;
            if (Mathf.Abs(deltaAngle) >= Mathf.Abs(angle))
            {
                unit.Rotate(unit.Position.Normalized(), angle);
                unit.PathOriented = true;
                unit.Path.StartMove(unit);
            }
            else
                unit.Rotate(unit.Position.Normalized(), deltaAngle);
        }
    }

    private void IncreaseVisibility(Tile fromTile, int range)
    {
        var tiles = _tileSearchService.GetVisibleTiles(fromTile, range);
        foreach (var tile in tiles)
            _tileShaderService.IncreaseVisibility(tile);
    }

    private void DecreaseVisibility(Tile fromTile, int range)
    {
        var tiles = _tileSearchService.GetVisibleTiles(fromTile, range);
        foreach (var tile in tiles)
            _tileShaderService.DecreaseVisibility(tile);
    }

    private void OnTileIdChanged(IHexUnit unit, int pre, int now)
    {
        if (pre > 0)
        {
            var preTile = _tileRepo.GetById(pre)!;
            DecreaseVisibility(preTile, Unit.VisionRange);
            _tileRepo.SetUnitId(preTile, 0);
        }

        _unitRepo.GetById(unit.Id)!.TileId = now;
        _hexUnitService.ValidateLocation(unit);
        var tile = _tileRepo.GetById(now)!;
        IncreaseVisibility(tile, Unit.VisionRange);
        _tileRepo.SetUnitId(tile, unit.Id);
    }

    private void OnDied(IHexUnit unit)
    {
        _unitRepo.Delete(unit.Id);
        var tile = _tileRepo.GetById(unit.TileId)!;
        DecreaseVisibility(tile, Unit.VisionRange);
        _tileRepo.SetUnitId(tile, 0);
        unit.QueueFree();
    }
}