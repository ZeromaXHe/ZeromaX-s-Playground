using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class ChunkService(IPointService pointService, IChunkRepo chunkRepo) : IChunkService
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

    private readonly VpTree<Vector3> _chunkPointVpTree = new();

    public Chunk SearchNearest(Vector3 pos)
    {
        _chunkPointVpTree.Search(pos, 1, out var results, out _);
        return chunkRepo.GetByPos(results[0]);
    }

    public void InitChunks(int chunkDivisions)
    {
        var time = Time.GetTicksMsec();
        pointService.SubdivideIcosahedron(chunkDivisions, (v, _) => chunkRepo.Add(v));
        _chunkPointVpTree.Create(chunkRepo.GetAll().Select(c => c.Pos).ToArray(),
            (p0, p1) => p0.DistanceTo(p1));
        GD.Print($"InitChunks chunkDivisions {chunkDivisions}, cost: {Time.GetTicksMsec() - time}");
    }
}