using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

public enum ChunkLod
{
    JustHex, // 每个地块只有六个平均高度点组成的六边形（非平面）
    PlaneHex, // 高度立面，无特征，无河流的六边形
    SimpleHex, // 最简单的 Solid + 斜面六边形 
    TerracesHex, // 增加台阶
    Full, // 增加边细分
}

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
    Mesh[] GetLodMeshes(ChunkLod lod, int id);
    void AddLodMeshes(ChunkLod lod, int id, Mesh[] mesh);
    void RemoveAllLodMeshes(int id);
}