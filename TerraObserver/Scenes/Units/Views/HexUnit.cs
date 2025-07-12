using System;
using Godot;
using TO.Domains.Types.Units;

namespace TerraObserver.Scenes.Units.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 09:29:10
public partial class HexUnit : CsgBox3D, IHexUnit
{
    #region 事件

    public event Action<HexUnit, float>? Processed;

    #endregion

    #region 普通属性

    public int Id { get; set; }
    public Vector3 BeginRotation { get; set; }

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

    private float _orientation;
    public IHexUnitPath? Path { get; set; }
    public int PathTileIdx { get; set; }
    public bool PathOriented { get; set; }

    #endregion

    #region 生命周期

    public override void _Process(double delta) => Processed?.Invoke(this, (float)delta);

    #endregion
}