using Godot;
using Godot.Abstractions.Bases;
using TO.Domains.Enums.HexSpheres.Chunks;

namespace TO.Abstractions.Chunks;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 15:17:08
public interface IChunk : INode3D
{
    IHexMesh? GetTerrain();
    IHexMesh? GetRivers();
    IHexMesh? GetRoads();
    IHexMesh? GetWater();
    IHexMesh? GetWaterShore();
    IHexMesh? GetEstuary();
    ChunkLodEnum Lod { get; set; }
    void ShowMesh(Mesh[] meshes);
    Mesh[] GetMeshes();
}