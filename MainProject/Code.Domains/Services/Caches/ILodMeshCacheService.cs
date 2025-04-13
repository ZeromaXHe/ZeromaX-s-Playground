using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Services.Caches;

public enum MeshType
{
    Terrain,
    Water,
    WaterShore,
    Estuary
}

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-15 20:53
public interface ILodMeshCacheService
{
    Mesh[]? GetLodMeshes(ChunkLod lod, int id);
    void AddLodMeshes(ChunkLod lod, int id, Mesh[] mesh);
    void RemoveAllLodMeshes();
    void RemoveLodMeshes(int id);
}