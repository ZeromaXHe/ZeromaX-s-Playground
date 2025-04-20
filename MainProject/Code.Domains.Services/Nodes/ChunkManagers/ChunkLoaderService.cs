using System.Diagnostics;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.ChunkManagers;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions.ChunkManagers;

namespace Domains.Services.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:12:45
public class ChunkLoaderService(
    IChunkLoaderRepo chunkLoaderRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    IChunkRepo chunkRepo,
    ITileRepo tileRepo)
    : IChunkLoaderService
{
    private IChunkLoader Self => chunkLoaderRepo.Singleton!;
    private readonly Stopwatch _stopwatch = new();

    public void OnProcessed(double delta)
    {
        _stopwatch.Restart();
        var allClear = true;
        var limitCount = Mathf.Min(20, Self.LoadSet.Count);
#if MY_DEBUG
        var loadCount = 0;
#endif
        // 限制加载耗时（但加载优先级最高）
        while (limitCount > 0 && _stopwatch.ElapsedMilliseconds <= 14)
        {
            var chunkId = Self.LoadSet.First();
            Self.LoadSet.Remove(chunkId);
            Self.ShowChunk(chunkRepo.GetById(chunkId)!);
            limitCount--;
#if MY_DEBUG
            loadCount++;
#endif
        }

        if (Self.LoadSet.Count > 0)
            allClear = false;
        var loadTime = _stopwatch.ElapsedMilliseconds;
        var totalTime = loadTime;
        _stopwatch.Restart();

        limitCount = Math.Min(20, Self.RefreshSet.Count);
#if MY_DEBUG
        var refreshCount = 0;
#endif
        // 限制刷新耗时（刷新优先级其次）
        while (limitCount > 0 && totalTime + _stopwatch.ElapsedMilliseconds <= 14)
        {
            var chunkId = Self.RefreshSet.First();
            Self.RefreshSet.Remove(chunkId);
            Self.ShowChunk(chunkRepo.GetById(chunkId)!);
            limitCount--;
#if MY_DEBUG
            refreshCount++;
#endif
        }

        if (Self.RefreshSet.Count > 0)
            allClear = false;
        var refreshTime = _stopwatch.ElapsedMilliseconds;
        totalTime += refreshTime;
        _stopwatch.Restart();

        limitCount = Math.Min(100, Self.UnloadSet.Count);
#if MY_DEBUG
        var unloadCount = 0;
#endif
        // 限制卸载耗时（卸载优先级最低）
        while (limitCount > 0 && totalTime + _stopwatch.ElapsedMilliseconds <= 14)
        {
            var chunkId = Self.UnloadSet.First();
            Self.UnloadSet.Remove(chunkId);
            Self.HideChunk(chunkId);
            limitCount--;
#if MY_DEBUG
            unloadCount++;
#endif
        }

        if (Self.UnloadSet.Count > 0)
            allClear = false;

#if MY_DEBUG // 好像 C# 默认 define 了 DEBUG，所以这里写 MY_DEBUG。（可以通过字体是否为灰色，判断）
        var unloadTime = _stopwatch.ElapsedMilliseconds;
        totalTime += unloadTime;
        var log = $"ChunkLoader _Process {totalTime} ms | load {loadCount}: {loadTime} ms, unload {
            unloadCount}: {unloadTime} ms, refresh {refreshCount}: {refreshTime} ms";
        if (totalTime <= 16)
            GD.Print(log);
        else
            GD.PrintErr(log);
#endif

        _stopwatch.Stop();
        if (allClear) Self.SetProcess(false);
    }

#if !FEATURE_NEW
    public void ExploreFeatures(Tile tile)
    {
        var tileId = tile.Id;
        var chunkId = tileRepo.GetById(tileId)!.ChunkId;
        Self.ExploreChunkFeatures(chunkId, tileId);
    }
#endif

    // 后续优化可以考虑：
    // 全过程异步化，即：下次新任务来时停止上次任务，并保证一次任务能分成多帧执行。
    // 按照优先级，先加载附近的高模，再往外扩散。限制每帧的加载数量，保证不影响帧率。
    // 0. 预先计算好后续队列（加载队列、卸载队列、渲染队列、预加载队列）？
    // 1. 从相机当前位置开始，先保证视野范围内以最低 LOD 初始化
    //  1.1 先 BFS 最近的高精度 LOD 分块，按最低 LOD 初始化
    //  1.2 从正前方向视野两侧进行分块加载，同样按最低 LOD 初始化
    // 2. 提高视野范围内的 LOD 精度，同时对视野范围外的内容预加载（包括往外一圈的分块，和在玩家视角背后的）
    //
    // 动态卸载：
    // 建立一个清理队列，每次终止上次任务时，把所有分块加入到这个清理队列中。
    // 每帧从清理队列中出队一部分，校验它们当前的 LOD 状态，如果 LOD 状态不对，则卸载。
    public void UpdateInsightChunks(Transform3D transform, float delta)
    {
        // var time = Time.GetTicksMsec();
        // 未能卸载的分块，说明本轮依然是在显示的分块
        foreach (var unloadId in Self.UnloadSet.Where(id => Self.UsingChunks.ContainsKey(id)))
            Self.InsightChunkIdsNow.Add(unloadId);
        Self.UnloadSet.Clear();
        Self.RefreshSet.Clear(); // 刷新分块一定在 _rimChunkIds 或 InsightChunkIdsNow 中，直接丢弃
        Self.LoadSet.Clear(); // 未加载分块直接丢弃

        var camera = Self.GetViewport().GetCamera3D();
        // 隐藏边缘分块
        foreach (var chunkId in Self.RimChunkIds.Where(id => Self.UsingChunks.ContainsKey(id)))
        {
            Self.UnloadSet.Add(chunkId);
            UpdateChunkInsightAndLod(chunkRepo.GetById(chunkId)!, camera, false);
        }

        Self.RimChunkIds.Clear();
        foreach (var preInsightChunkId in Self.InsightChunkIdsNow)
        {
            var preInsightChunk = chunkRepo.GetById(preInsightChunkId)!;
            Self.VisitedChunkIds.Add(preInsightChunkId);
            if (!IsChunkInsight(preInsightChunk, camera))
            {
                // 分块不在视野范围内，隐藏它
                Self.UnloadSet.Add(preInsightChunkId);
                UpdateChunkInsightAndLod(preInsightChunk, camera, false);
                continue;
            }

            Self.InsightChunkIdsNext.Add(preInsightChunkId);
            UpdateChunkInsightAndLod(preInsightChunk, camera, true);
            // 刷新 Lod
            if (Self.UsingChunks.ContainsKey(preInsightChunkId))
                Self.RefreshSet.Add(preInsightChunkId);
            else
                Self.LoadSet.Add(preInsightChunkId);
            // 分块在视野内，他的邻居才比较可能是在视野内
            // 将之前不在但现在可能在视野范围内的 id 加入带查询队列
            SearchNeighbor(preInsightChunk, Self.InsightChunkIdsNow);
        }

        // 有种极端情况，就是新的视野范围内一个旧视野范围分块都没有！
        // 这时放开限制进行 BFS，直到找到第一个可见的分块
        // （因为我们认为新位置还是会具有空间上的相近性，BFS 应该会比随便找可见分块更好）
        if (Self.InsightChunkIdsNext.Count == 0)
        {
            foreach (var chunk in Self.InsightChunkIdsNow.Select(chunkRepo!.GetById))
                SearchNeighbor(chunk!, Self.VisitedChunkIds); // 搜索所有外缘邻居

            while (Self.ChunkQueryQueue.Count > 0)
            {
                var chunkId = Self.ChunkQueryQueue.Dequeue();
                var chunk = chunkRepo.GetById(chunkId)!;
                if (IsChunkInsight(chunk, camera))
                {
                    // 找到第一个可见分块，重新入队，后面进行真正的处理
                    Self.ChunkQueryQueue.Enqueue(chunkId);
                    break;
                }

                SearchNeighbor(chunk);
            }
        }

        // BFS 查询那些原来不在视野范围内的分块
        while (Self.ChunkQueryQueue.Count > 0)
        {
            var chunkId = Self.ChunkQueryQueue.Dequeue();
            var chunk = chunkRepo.GetById(chunkId)!;
            if (!IsChunkInsight(chunk, camera)) continue;
            if (!Self.InsightChunkIdsNext.Add(chunkId)) continue;
            Self.LoadSet.Add(chunkId);
            UpdateChunkInsightAndLod(chunk, camera, true);
            SearchNeighbor(chunk);
        }

        // 清理好各个数据结构，等下一次调用直接使用
        Self.ChunkQueryQueue.Clear();
        Self.VisitedChunkIds.Clear();
        Self.InsightChunkIdsNow.Clear();
        Self.UpdateInSightSetNextIdx();
        // 显示外缘分块
        InitOutRimChunks(camera);
        Self.SetProcess(true);
        // GD.Print($"ChunkLoader UpdateInsightChunks cost {Time.GetTicksMsec() - time} ms");
    }

    public void InitChunkNodes()
    {
        var camera = Self.GetViewport().GetCamera3D();
        foreach (var chunk in chunkRepo.GetAll())
        {
            var id = chunk.Id;
            // 此时拿不到真正 focusBase 的位置，暂且用相机自己的代替
            if (!IsChunkInsight(chunk, camera))
                continue;
            Self.LoadSet.Add(id);
            UpdateChunkInsightAndLod(chunk, camera, true);
            Self.InsightChunkIdsNow.Add(id);
        }

        InitOutRimChunks(camera);
        Self.SetProcess(true);
    }

    private void SearchNeighbor(Chunk chunk, HashSet<int>? filterSet = null)
    {
        foreach (var neighbor in chunkRepo.GetNeighbors(chunk))
        {
            if (filterSet?.Contains(neighbor.Id) ?? false) continue;
            if (Self.VisitedChunkIds.Add(neighbor.Id))
                Self.ChunkQueryQueue.Enqueue(neighbor.Id);
        }
    }

    // 处理视野外的一圈边缘分块。如果是之前的边缘地块，需要放入刷新队列，如果是新的，则放入加载队列
    // 显示的分块向外多生成一圈，防止缺失进入视野的边缘瓦片
    private void InitOutRimChunks(Camera3D camera)
    {
        foreach (var rim in from chunkId in Self.InsightChunkIdsNow
                 select chunkRepo.GetById(chunkId)
                 into chunk
                 from neighbor in chunkRepo.GetNeighbors(chunk)
                 where !Self.InsightChunkIdsNow.Contains(neighbor.Id)
                 select neighbor)
        {
            if (!Self.RimChunkIds.Add(rim.Id)) continue;
            UpdateChunkInsightAndLod(rim, camera, true);
            if (Self.UnloadSet.Contains(rim.Id))
            {
                Self.UnloadSet.Remove(rim.Id);
                Self.RefreshSet.Add(rim.Id);
            }
            else
                Self.LoadSet.Add(rim.Id);
        }
    }

    private void UpdateChunkInsightAndLod(Chunk chunk, Camera3D camera, bool insight) =>
        chunkRepo.UpdateChunkInsightAndLod(chunk.Id, insight,
            insight ? CalcLod(chunk.Pos.DistanceTo(Self.ToLocal(camera.GlobalPosition))) : ChunkLod.JustHex);

    private ChunkLod CalcLod(float distance)
    {
        var tileLen = hexPlanetManagerRepo.Radius / hexPlanetManagerRepo.Divisions;
        return distance > tileLen * 160 ? ChunkLod.JustHex :
            distance > tileLen * 80 ? ChunkLod.PlaneHex :
            distance > tileLen * 40 ? ChunkLod.SimpleHex :
            distance > tileLen * 20 ? ChunkLod.TerracesHex : ChunkLod.Full;
    }

    // 注意，判断是否在摄像机内，不是用 GetViewport().GetVisibleRect().HasPoint(camera.UnprojectPosition(chunk.Pos))
    // 因为后面要根据相机位置动态更新可见区域，上面方法这个仅仅是对应初始时的可见区域
    private bool IsChunkInsight(Chunk chunk, Camera3D camera) =>
        Mathf.Cos(chunk.Pos.Normalized().AngleTo(Self.ToLocal(camera.GlobalPosition).Normalized()))
        > hexPlanetManagerRepo.Radius / camera.GlobalPosition.Length()
        && camera.IsPositionInFrustum(Self.ToGlobal(chunk.Pos));
}