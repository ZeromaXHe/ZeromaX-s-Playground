using System;
using Godot;
using TO.Abstractions.Views.Chunks;
using TO.Presenters.Views.Chunks;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:31:36
[Tool]
public partial class HexGridChunk : HexGridChunkFS, IHexGridChunk
{
    #region 生命周期

    public override void _Process(double delta) => Processed?.Invoke(this);

    #endregion

    #region 事件

    public event Action<IHexGridChunk>? Processed;

    #endregion

    #region Export 属性

    [Export] public HexMesh? Terrain { get; set; }
    public override IHexMesh? GetTerrain() => Terrain;
    [Export] public HexMesh? Rivers { get; set; }
    public override IHexMesh? GetRivers() => Rivers;
    [Export] public HexMesh? Roads { get; set; }
    public override IHexMesh? GetRoads() => Roads;
    [Export] public HexMesh? Water { get; set; }
    public override IHexMesh? GetWater() => Water;
    [Export] public HexMesh? WaterShore { get; set; }
    public override IHexMesh? GetWaterShore() => WaterShore;
    [Export] public HexMesh? Estuary { get; set; }
    public override IHexMesh? GetEstuary() => Estuary;

    #endregion
}