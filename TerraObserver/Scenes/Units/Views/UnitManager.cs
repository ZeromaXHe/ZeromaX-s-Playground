using System;
using System.Collections.Generic;
using Godot;
using TO.Domains.Types.Units;

namespace TerraObserver.Scenes.Units.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 18:54:46
public partial class UnitManager : Node3D, IUnitManager
{
    #region 事件

    public event Action<HexUnit>? UnitInstantiated;

    #endregion

    #region Export

    [Export] public PackedScene? UnitScene { get; set; }

    public IHexUnit InstantiateUnit()
    {
        var unit = UnitScene!.Instantiate<HexUnit>();
        AddChild(unit);
        UnitInstantiated?.Invoke(unit);
        return unit;
    }

    #endregion

    #region 普通属性

    public Dictionary<int, IHexUnit> Units { get; } = new();
    public int PathFromTileId { get; set; }

    #endregion
}