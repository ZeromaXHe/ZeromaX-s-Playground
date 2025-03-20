using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public class ChunkService(IPointService pointService, IPlanetSettingService planetSettingService, IChunkRepo chunkRepo)
    : IChunkService
{
    public event IChunkService.RefreshChunkEvent RefreshChunk;
    public event IChunkService.RefreshTileLabelEvent RefreshChunkTileLabel;
    public void Refresh(Chunk chunk) => RefreshChunk?.Invoke(chunk.Id);

    public void RefreshTileLabel(Chunk chunk, int tileId, string text) =>
        RefreshChunkTileLabel?.Invoke(chunk.Id, tileId, text);

    #region 透传存储库方法

    public Chunk GetById(int id) => chunkRepo.GetById(id);
    public int GetCount() => chunkRepo.GetCount();
    public IEnumerable<Chunk> GetAll() => chunkRepo.GetAll();
    public void Truncate() => chunkRepo.Truncate();

    #endregion

    // 单位球上的点 VP 树
    private readonly VpTree<Vector3> _chunkPointVpTree = new();

    public Chunk SearchNearest(Vector3 pos)
    {
        _chunkPointVpTree.Search(pos.Normalized(), 1, out var results, out _);
        var centerId = pointService.GetIdByPosition(true, results[0]);
        return centerId == null ? null : chunkRepo.GetByCenterId((int)centerId);
    }

    public IEnumerable<Chunk> GetNeighbors(Chunk chunk) =>
        chunk.NeighborCenterIds.Select(chunkRepo.GetByCenterId);

    public Chunk GetNeighborByIdx(Chunk chunk, int idx) =>
        idx >= 0 && idx < chunk.NeighborCenterIds.Count
            ? chunkRepo.GetByCenterId(chunk.NeighborCenterIds[idx])
            : null;

    public void InitChunks()
    {
        var time = Time.GetTicksMsec();
        pointService.InitPointsAndFaces(true, planetSettingService.ChunkDivisions);
        foreach (var point in pointService.GetAllByChunky(true))
        {
            var hexFaces = pointService.GetOrderedFaces(point);
            var neighborCenters = pointService.GetNeighborCenterIds(hexFaces, point)
                .Select(c => c.Id)
                .ToList();
            chunkRepo.Add(point.Id, point.Position * (planetSettingService.Radius + planetSettingService.MaxHeight),
                hexFaces.Select(f => f.Id).ToList(), neighborCenters);
        }

        _chunkPointVpTree.Create(pointService.GetAllByChunky(true).Select(c => c.Position).ToArray(),
            (p0, p1) => p0.DistanceTo(p1));
        GD.Print($"InitChunks chunkDivisions {
                planetSettingService.ChunkDivisions}, cost: {Time.GetTicksMsec() - time} ms");
    }
}