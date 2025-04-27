using System;
using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-01 12:45
public partial class HexUnit : CsgBox3D, IHexUnit
{
    public HexUnit() => Context.RegisterToHolder<IHexUnit>(this);
    public event IHexUnit.TileIdChangedEvent? TileIdChanged;
    public event Action? Died;

    public NodeEvent NodeEvent { get; } = new(process: true);

    public override void _Process(double delta) => NodeEvent.EmitProcessed(delta);

    public int Id { get; set; }
    public Vector3 BeginRotation { get; set; }
    private int _tileId;

    public int TileId
    {
        get => _tileId;
        set
        {
            var pre = _tileId;
            _tileId = value;
            TileIdChanged?.Invoke(pre, value);
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
            Rotation = BeginRotation;
            Rotate(Position.Normalized(), _orientation);
        }
    }

    public IHexUnitPath? Path { get; set; }
    public int PathTileIdx { get; set; }
    public bool PathOriented { get; set; }
    public void SetTravelTileId(int id) => _tileId = id;

    public void FinishPath()
    {
        Path = null;
    }

    public void Die() => Died?.Invoke();
}