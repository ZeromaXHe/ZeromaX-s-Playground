using Domains.Models.Entities.PlanetGenerates;
using Godot;

namespace Domains.Services.Abstractions.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public interface IChunkService
{
    // 最近邻搜索
    Chunk? SearchNearest(Vector3 pos);
    void InitChunks();
}