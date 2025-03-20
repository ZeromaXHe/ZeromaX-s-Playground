using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public interface IChunkService
{
    delegate void RefreshChunkEvent(int id);

    event RefreshChunkEvent RefreshChunk;

    delegate void RefreshTileLabelEvent(int chunkId, int tileId, string text);

    event RefreshTileLabelEvent RefreshChunkTileLabel;
    void Refresh(Chunk chunk);
    void RefreshTileLabel(Chunk chunk, int tileId, string text);

    #region 透传存储库方法

    Chunk GetById(int id);
    int GetCount();
    IEnumerable<Chunk> GetAll();
    void Truncate();

    #endregion

    // 最近邻搜索
    Chunk SearchNearest(Vector3 pos);
    IEnumerable<Chunk> GetNeighbors(Chunk chunk);
    Chunk GetNeighborByIdx(Chunk chunk, int idx);
    void InitChunks();
}