using System;
using Godot;
using TO.Abstractions.Chunks;
using TO.Domains.Enums.HexSpheres.Chunks;
using TO.Domains.Enums.Meshes;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:31:36
[Tool]
public partial class HexGridChunk : Node3D, IHexGridChunk
{
    #region 事件和 Export 属性
    public event Action<IHexGridChunk>? Processed;
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

    #endregion

    #region 外部属性、变量

    public int Id { get; set; }
    public ChunkLodEnum Lod { get; set; } = ChunkLodEnum.JustHex;

    #endregion

    #region 生命周期

    public override void _Process(double delta) => Processed?.Invoke(this);

    #endregion

    public void HideOutOfSight()
    {
        Hide();
        Terrain!.Clear();
        Rivers!.Clear();
        Roads!.Clear();
        Water!.Clear();
        WaterShore!.Clear();
        Estuary!.Clear();
        Id = 0; // 重置 id，归还给池子
    }

    public void ShowMesh(Mesh[] meshes)
    {
        Terrain!.ShowMesh(meshes[(int)MeshType.Terrain]);
        Water!.ShowMesh(meshes[(int)MeshType.Water]);
        WaterShore!.ShowMesh(meshes[(int)MeshType.WaterShore]);
        Estuary!.ShowMesh(meshes[(int)MeshType.Estuary]);
    }

    public Mesh[] GetMeshes() => [Terrain!.Mesh, Water!.Mesh, WaterShore!.Mesh, Estuary!.Mesh];

    public void ApplyNewData()
    {
        Terrain!.Apply();
        Rivers!.Apply(); // 河流暂时不支持 Lod
        Roads!.Apply(); // 道路暂时不支持 Lod
        Water!.Apply();
        WaterShore!.Apply();
        Estuary!.Apply();
    }

    public void ClearOldData()
    {
        Terrain!.Clear();
        Rivers!.Clear();
        Roads!.Clear();
        Water!.Clear();
        WaterShore!.Clear();
        Estuary!.Clear();
    }
}