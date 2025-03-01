using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constant;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class ChunkService(IChunkRepo chunkRepo) : IChunkService
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
        var points = IcosahedronConstants.Vertices;
        var indices = IcosahedronConstants.Indices;
        var framePoints = new List<Vector3>(points);
        foreach (var v in points)
            chunkRepo.Add(v);
        for (var idx = 0; idx < indices.Count; idx += 3)
        {
            var p0 = points[indices[idx]];
            var p1 = points[indices[idx + 1]];
            var p2 = points[indices[idx + 2]];
            var leftSide = Subdivide(p0, p1, chunkDivisions, true);
            var rightSide = Subdivide(p0, p2, chunkDivisions, true);
            for (var i = 1; i <= chunkDivisions; i++)
                Subdivide(leftSide[i], rightSide[i], i, i == chunkDivisions);
        }

        _chunkPointVpTree.Create(chunkRepo.GetAll().Select(c => c.Pos).ToArray(),
            (p0, p1) => p0.DistanceTo(p1));
        GD.Print($"InitChunks chunkDivisions {chunkDivisions}, cost: {Time.GetTicksMsec() - time}");
        return;

        List<Vector3> Subdivide(Vector3 from, Vector3 target, int count, bool checkFrameExist)
        {
            var segments = new List<Vector3> { from };

            for (var i = 1; i < count; i++)
            {
                // 注意这里用 Slerp 而不是 Lerp，让所有的点都在单位球面而不是单位正二十面体上，方便我们后面 VP 树找最近点
                var v = from.Slerp(target, (float)i / count);
                Vector3 newPoint = default;
                if (checkFrameExist)
                {
                    var existingPoint = framePoints.FirstOrDefault(candidatePoint => candidatePoint.IsEqualApprox(v));
                    if (existingPoint != default)
                        newPoint = existingPoint;
                }

                if (newPoint == default)
                {
                    newPoint = v;
                    chunkRepo.Add(v);
                    if (checkFrameExist)
                        framePoints.Add(newPoint);
                }

                segments.Add(newPoint);
            }

            segments.Add(target);
            return segments;
        }
    }
}