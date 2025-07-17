using System;
using System.Collections.Generic;
using Godot;
using TO.Domains.Types.Chunks;
using TO.Domains.Types.HexSpheres.Components.Chunks;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:31:36
[Tool]
public partial class HexGridChunk : Node3D, IHexGridChunk
{
    #region 事件

    public event Action<HexGridChunk>? Processed;

    #endregion

    #region Export 属性

    [Export] public HexMesh? Terrain { get; set; }
    public IHexMesh? GetTerrain() => Terrain;
    [Export] public HexMesh? Rivers { get; set; }
    public IHexMesh? GetRivers() => Rivers;
    [Export] public HexMesh? Roads { get; set; }
    public IHexMesh? GetRoads() => Roads;
    [Export] public HexMesh? Water { get; set; }
    public IHexMesh? GetWater() => Water;
    [Export] public HexMesh? WaterShore { get; set; }
    public IHexMesh? GetWaterShore() => WaterShore;
    [Export] public HexMesh? Estuary { get; set; }
    public IHexMesh? GetEstuary() => Estuary;
    [Export] public HexMesh? Walls { get; set; }
    public IHexMesh? GetWalls() => Walls;

    #endregion

    #region 普通属性

    public int Id { get; set; }
    public ChunkLodEnum Lod { get; set; } = ChunkLodEnum.JustHex;
    public HashSet<int>? EditingTileIds => null;

    #endregion

    #region 生命周期

    public override void _Process(double delta) => Processed?.Invoke(this);

    #endregion
}