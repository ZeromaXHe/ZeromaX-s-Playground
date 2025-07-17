using System.Collections.Generic;
using Godot;
using TO.Domains.Types.Units;

namespace TerraObserver.Scenes.Units.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 09:33:59
public partial class HexUnitPath : Path3D, IHexUnitPath
{
    #region 普通属性

    public bool Working { get; set; }
    public int[]? TileIds { get; set; }
    public float[]? Progresses { get; set; }

    #endregion

    #region on-ready

    public PathFollow3D? PathFollow { get; private set; }
    public RemoteTransform3D? RemoteTransform { get; private set; }
    public CsgPolygon3D? View { get; private set; }

    #endregion

    #region 生命周期

    public override void _Ready()
    {
        PathFollow = GetNode<PathFollow3D>("%PathFollow3D");
        RemoteTransform = GetNode<RemoteTransform3D>("%RemoteTransform3D");
        View = GetNode<CsgPolygon3D>("%View");
    }

    #endregion
}