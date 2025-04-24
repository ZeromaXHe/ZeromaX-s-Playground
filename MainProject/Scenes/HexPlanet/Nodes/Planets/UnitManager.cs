using System;
using System.Collections.Generic;
using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;
using Nodes.Abstractions.Planets;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-27 10:39:20
public partial class UnitManager : Node3D, IUnitManager
{
    public UnitManager()
    {
        Context.RegisterToHolder<IUnitManager>(this);
    }
    public event Action? PathFromTileIdSetZero;
    public NodeEvent? NodeEvent => null;

    public override void _Ready() => InitOnReadyNodes();

    [Export] private PackedScene? _unitScene;

    public Dictionary<int, IHexUnit> Units { get; } = new();

    #region on-ready 节点

    private HexUnitPathPool? HexUnitPathPool { get; set; }
    public IHexUnitPathPool? GetHexUnitPathPool() => HexUnitPathPool;

    private void InitOnReadyNodes()
    {
        HexUnitPathPool = GetNode<HexUnitPathPool>("%HexUnitPathPool");
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
                PathFromTileIdSetZero?.Invoke();
        }
    }

    public void AddUnit(int tileId, float orientation)
    {
        var unit = _unitScene!.Instantiate<HexUnit>();
        AddChild(unit);
        Units[unit.Id] = unit;
        unit.TileId = tileId;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(int unitId)
    {
        Units[unitId].Die();
        Units.Remove(unitId);
    }
}