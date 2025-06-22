using System;
using Godot;
using TO.Abstractions.Models.Planets;
using TO.Presenters.Models.Planets;

namespace TerraObserver.Scenes.Planets.Models;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
[Tool]
[GlobalClass]
#pragma warning disable GD0401
public partial class Planet : PlanetFS, IPlanet
#pragma warning restore GD0401
{
    #region 事件

    public event Action? ParamsChanged;
    public override void EmitParamsChanged() => ParamsChanged?.Invoke();

    #endregion

    #region Export 属性

    [ExportGroup("戈德堡多面体配置")]
    [Export(PropertyHint.Range, "5, 1000")]
    public override float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            OnParamsChanged();
        }
    }

    private float _radius = 100f;

    [Export(PropertyHint.Range, "1, 200")]
    public override int Divisions
    {
        get => _divisions;
        set
        {
            _divisions = value;
            _chunkDivisions = Mathf.Min(Mathf.Max(1, _divisions / 10), _chunkDivisions);
            OnParamsChanged();
        }
    }

    private int _divisions = 20;

    [Export(PropertyHint.Range, "1, 20")]
    public override int ChunkDivisions
    {
        get => _chunkDivisions;
        set
        {
            _chunkDivisions = value;
            _divisions = Mathf.Max(Mathf.Min(200, _chunkDivisions * 10), _divisions);
            OnParamsChanged();
        }
    }

    private int _chunkDivisions = 2;

    #endregion
}