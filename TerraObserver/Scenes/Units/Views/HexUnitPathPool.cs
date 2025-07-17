using System.Collections.Generic;
using Godot;
using TO.Domains.Types.Units;

namespace TerraObserver.Scenes.Units.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 09:34:10
public partial class HexUnitPathPool : Node3D, IHexUnitPathPool
{
    #region Export

    [Export] public PackedScene? PathScene { get; set; }

    public IHexUnitPath InstantiatePath()
    {
        var path = PathScene!.Instantiate<HexUnitPath>();
        AddChild(path);
        return path;
    }

    #endregion

    #region 普通属性

    public List<IHexUnitPath> Paths { get; } = [];

    #endregion
}