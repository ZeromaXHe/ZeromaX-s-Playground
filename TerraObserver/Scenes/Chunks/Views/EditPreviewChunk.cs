using System.Collections.Generic;
using Godot;
using TO.Domains.Types.Chunks;
using TO.Domains.Types.HexSpheres;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-07 14:16:49
public partial class EditPreviewChunk : Node3D, IEditPreviewChunk
{
    [Export] public ShaderMaterial[]? TerrainMaterials { get; set; }
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
    public ChunkLodEnum Lod { get; set; } = ChunkLodEnum.Full; // 默认值是给预览用的
    public HashSet<int> EditingTileIds { get; } = [];
}