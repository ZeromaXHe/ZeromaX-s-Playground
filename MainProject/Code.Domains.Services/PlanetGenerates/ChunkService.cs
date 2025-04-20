using Commons.Utils;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Services.Abstractions.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;

namespace Domains.Services.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public class ChunkService(
    IPointService pointService,
    IPointRepo pointRepo,
    IFaceRepo faceRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    IChunkRepo chunkRepo)
    : IChunkService
{
    // 单位球上的点 VP 树
    private readonly VpTree<Vector3> _chunkPointVpTree = new();

    public Chunk? SearchNearest(Vector3 pos)
    {
        _chunkPointVpTree.Search(pos.Normalized(), 1, out var results, out _);
        var centerId = pointRepo.GetIdByPosition(true, results[0]);
        return centerId == null ? null : chunkRepo.GetByCenterId((int)centerId);
    }

    public void InitChunks()
    {
        var time = Time.GetTicksMsec();
        pointService.InitPointsAndFaces(true, hexPlanetManagerRepo.ChunkDivisions);
        foreach (var point in pointRepo.GetAllByChunky(true))
        {
            var hexFaces = faceRepo.GetOrderedFaces(point);
            var neighborCenters = pointRepo.GetNeighborCenterIds(hexFaces, point)
                .Select(c => c.Id)
                .ToList();
            chunkRepo.Add(point.Id, point.Position * (hexPlanetManagerRepo.Radius + hexPlanetManagerRepo.MaxHeight),
                hexFaces.Select(f => f.Id).ToList(), neighborCenters);
        }

        _chunkPointVpTree.Create(pointRepo.GetAllByChunky(true).Select(c => c.Position).ToArray(),
            (p0, p1) => p0.DistanceTo(p1));
        GD.Print($"InitChunks chunkDivisions {
            hexPlanetManagerRepo.ChunkDivisions}, cost: {Time.GetTicksMsec() - time} ms");
    }
}