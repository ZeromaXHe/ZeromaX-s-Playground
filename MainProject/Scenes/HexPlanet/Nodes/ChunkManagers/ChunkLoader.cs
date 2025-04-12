using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apps.Events;
using Apps.Services.Shaders;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Repos.PlanetGenerates;
using Domains.Services.PlanetGenerates;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-26 20:33:11
[Tool]
public partial class ChunkLoader : Node3D
{
    public ChunkLoader() => InitServices();

    [Export] private PackedScene _gridChunkScene;

    #region 服务和存储

    private IChunkRepo _chunkRepo;
    private ITileRepo _tileRepo;
    private ITileShaderService _tileShaderService;
    private IPlanetSettingService _planetSettingService;

    private void InitServices()
    {
        _chunkRepo = Context.GetBeanFromHolder<IChunkRepo>();
        _chunkRepo.RefreshChunkTileLabel += OnChunkServiceRefreshChunkTileLabel;
        _tileRepo = Context.GetBeanFromHolder<ITileRepo>();
        _tileRepo.RefreshChunk += OnChunkServiceRefreshChunk;
        _tileShaderService = Context.GetBeanFromHolder<ITileShaderService>();
        _planetSettingService = Context.GetBeanFromHolder<IPlanetSettingService>();
        _tileShaderService.TileExplored += ExploreFeatures;
        if (!Engine.IsEditorHint())
        {
            OrbitCameraEvent.Instance.Transformed += UpdateInsightChunks;
        }
    }

    private void ExploreFeatures(int tileId)
    {
        var chunkId = _tileRepo.GetById(tileId)!.ChunkId;
        // BUG: 动态加载的分块会显示未探索的特征
        _usingChunks[chunkId]?.ExploreFeatures(tileId);
    }

    private void OnChunkServiceRefreshChunk(int id)
    {
        // 现在地图生成器也会调用，这时候分块还没创建。
        // _ready 不可或缺，否则启动失败
        if (_ready && _usingChunks.TryGetValue(id, out var chunk))
            chunk?.Refresh();
    }

    private void OnChunkServiceRefreshChunkTileLabel(int chunkId, int tileId, string text) =>
        _usingChunks[chunkId]?.RefreshTileLabel(tileId, text);

    private void CleanEventListeners()
    {
        // 不小心忽视了事件的解绑，会在编辑器下"重载已保存场景"时出问题报错！
        // （比如地图生成器逻辑会发出分块刷新信号，这时候老的场景代码貌似还在内存里，
        // 它接到事件后处理时，字典里的 Chunk 场景都已经释放，不存在了所以报错）
        // （对于新的场景，新分块字典里没数据，没有问题）
        // ERROR: /root/godot/modules/mono/glue/GodotSharp/GodotSharp/Core/NativeInterop/ExceptionUtils.cs:113 - System.ObjectDisposedException: Cannot access a disposed object.
        // ERROR: Object name: 'ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.HexGridChunk'.
        // 【切记】所以这里需要在退出场景树时清理事件监听！！！
        _tileShaderService.TileExplored -= ExploreFeatures;
        _tileRepo.RefreshChunk -= OnChunkServiceRefreshChunk;
        _chunkRepo.RefreshChunkTileLabel -= OnChunkServiceRefreshChunkTileLabel;
        if (!Engine.IsEditorHint())
        {
            OrbitCameraEvent.Instance.Transformed -= UpdateInsightChunks;
        }
    }

    #endregion

    // 值可能为 null，为 null 时说明分块需要初始化
    private readonly Dictionary<int, HexGridChunk> _usingChunks = new();
    private readonly Queue<HexGridChunk> _unusedChunks = [];

    // 表示当前可视分块 Set 的 _insightChunkIds 索引
    private int _insightSetIdx;
    private readonly HashSet<int>[] _insightChunkIds = [[], []];
    private HashSet<int> InsightChunkIdsNow => _insightChunkIds[_insightSetIdx];
    private HashSet<int> InsightChunkIdsNext => _insightChunkIds[_insightSetIdx ^ 1];
    private readonly Queue<int> _chunkQueryQueue = [];
    private readonly HashSet<int> _visitedChunkIds = [];

    private readonly HashSet<int> _rimChunkIds = [];

    private void UpdateInSightSetNextIdx() => _insightSetIdx ^= 1;

    private bool _ready;

    public override void _Ready()
    {
        _ready = true;
    }

    public override void _ExitTree() => CleanEventListeners();

    private readonly Stopwatch _stopwatch = new();

    public override void _Process(double delta)
    {
        if (!_ready) return;
        _stopwatch.Restart();
        var allClear = true;
        var limitCount = Mathf.Min(20, _loadSet.Count);
#if MY_DEBUG
        var loadCount = 0;
#endif
        // 限制加载耗时（但加载优先级最高）
        while (limitCount > 0 && _stopwatch.ElapsedMilliseconds <= 14)
        {
            var chunkId = _loadSet.First();
            _loadSet.Remove(chunkId);
            ShowChunk(_chunkRepo.GetById(chunkId));
            limitCount--;
#if MY_DEBUG
            loadCount++;
#endif
        }

        if (_loadSet.Count > 0)
            allClear = false;
        var loadTime = _stopwatch.ElapsedMilliseconds;
        var totalTime = loadTime;
        _stopwatch.Restart();

        limitCount = Math.Min(20, _refreshSet.Count);
#if MY_DEBUG
        var refreshCount = 0;
#endif
        // 限制刷新耗时（刷新优先级其次）
        while (limitCount > 0 && totalTime + _stopwatch.ElapsedMilliseconds <= 14)
        {
            var chunkId = _refreshSet.First();
            _refreshSet.Remove(chunkId);
            ShowChunk(_chunkRepo.GetById(chunkId));
            limitCount--;
#if MY_DEBUG
            refreshCount++;
#endif
        }

        if (_refreshSet.Count > 0)
            allClear = false;
        var refreshTime = _stopwatch.ElapsedMilliseconds;
        totalTime += refreshTime;
        _stopwatch.Restart();

        limitCount = Math.Min(100, _unloadSet.Count);
#if MY_DEBUG
        var unloadCount = 0;
#endif
        // 限制卸载耗时（卸载优先级最低）
        while (limitCount > 0 && totalTime + _stopwatch.ElapsedMilliseconds <= 14)
        {
            var chunkId = _unloadSet.First();
            _unloadSet.Remove(chunkId);
            HideChunk(chunkId);
            limitCount--;
#if MY_DEBUG
            unloadCount++;
#endif
        }

        if (_unloadSet.Count > 0)
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
        if (allClear) SetProcess(false);
    }

    // 待卸载的分块 Id 集合（上轮显示本轮不显示的分块，包括边缘分块中不再显示的）
    private readonly HashSet<int> _unloadSet = [];

    // 待刷新的分块 Id 集合（包括上轮显示本轮继续显示的分块，包括边缘分块中继续显示的）
    private readonly HashSet<int> _refreshSet = [];

    // 待加载的分块 Id 集合（新显示分块）
    private readonly HashSet<int> _loadSet = [];


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
    private void UpdateInsightChunks(Transform3D transform, float delta)
    {
        // var time = Time.GetTicksMsec();
        // 未能卸载的分块，说明本轮依然是在显示的分块
        foreach (var unloadId in _unloadSet.Where(id => _usingChunks.ContainsKey(id)))
            InsightChunkIdsNow.Add(unloadId);
        _unloadSet.Clear();
        _refreshSet.Clear(); // 刷新分块一定在 _rimChunkIds 或 InsightChunkIdsNow 中，直接丢弃
        _loadSet.Clear(); // 未加载分块直接丢弃

        var camera = GetViewport().GetCamera3D();
        // 隐藏边缘分块
        foreach (var chunkId in _rimChunkIds.Where(id => _usingChunks.ContainsKey(id)))
        {
            _unloadSet.Add(chunkId);
            UpdateChunkInsightAndLod(_chunkRepo.GetById(chunkId), camera, false);
        }

        _rimChunkIds.Clear();
        foreach (var preInsightChunkId in InsightChunkIdsNow)
        {
            var preInsightChunk = _chunkRepo.GetById(preInsightChunkId);
            _visitedChunkIds.Add(preInsightChunkId);
            if (!IsChunkInsight(preInsightChunk, camera))
            {
                // 分块不在视野范围内，隐藏它
                _unloadSet.Add(preInsightChunkId);
                UpdateChunkInsightAndLod(preInsightChunk, camera, false);
                continue;
            }

            InsightChunkIdsNext.Add(preInsightChunkId);
            UpdateChunkInsightAndLod(preInsightChunk, camera, true);
            // 刷新 Lod
            if (_usingChunks.ContainsKey(preInsightChunkId))
                _refreshSet.Add(preInsightChunkId);
            else
                _loadSet.Add(preInsightChunkId);
            // 分块在视野内，他的邻居才比较可能是在视野内
            // 将之前不在但现在可能在视野范围内的 id 加入带查询队列
            SearchNeighbor(preInsightChunk, InsightChunkIdsNow);
        }

        // 有种极端情况，就是新的视野范围内一个旧视野范围分块都没有！
        // 这时放开限制进行 BFS，直到找到第一个可见的分块
        // （因为我们认为新位置还是会具有空间上的相近性，BFS 应该会比随便找可见分块更好）
        if (InsightChunkIdsNext.Count == 0)
        {
            foreach (var chunk in InsightChunkIdsNow.Select(_chunkRepo.GetById))
                SearchNeighbor(chunk, _visitedChunkIds); // 搜索所有外缘邻居

            while (_chunkQueryQueue.Count > 0)
            {
                var chunkId = _chunkQueryQueue.Dequeue();
                var chunk = _chunkRepo.GetById(chunkId);
                if (IsChunkInsight(chunk, camera))
                {
                    // 找到第一个可见分块，重新入队，后面进行真正的处理
                    _chunkQueryQueue.Enqueue(chunkId);
                    break;
                }

                SearchNeighbor(chunk);
            }
        }

        // BFS 查询那些原来不在视野范围内的分块
        while (_chunkQueryQueue.Count > 0)
        {
            var chunkId = _chunkQueryQueue.Dequeue();
            var chunk = _chunkRepo.GetById(chunkId);
            if (!IsChunkInsight(chunk, camera)) continue;
            if (!InsightChunkIdsNext.Add(chunkId)) continue;
            _loadSet.Add(chunkId);
            UpdateChunkInsightAndLod(chunk, camera, true);
            SearchNeighbor(chunk);
        }

        // 清理好各个数据结构，等下一次调用直接使用
        _chunkQueryQueue.Clear();
        _visitedChunkIds.Clear();
        InsightChunkIdsNow.Clear();
        UpdateInSightSetNextIdx();
        // 显示外缘分块
        InitOutRimChunks(camera);
        SetProcess(true);
        // GD.Print($"ChunkLoader UpdateInsightChunks cost {Time.GetTicksMsec() - time} ms");
    }

    private void SearchNeighbor(Chunk chunk, HashSet<int> filterSet = null)
    {
        foreach (var neighbor in _chunkRepo.GetNeighbors(chunk))
        {
            if (filterSet?.Contains(neighbor.Id) ?? false) continue;
            if (_visitedChunkIds.Add(neighbor.Id))
                _chunkQueryQueue.Enqueue(neighbor.Id);
        }
    }

    private void UpdateChunkInsightAndLod(Chunk chunk, Camera3D camera, bool insight) =>
        _chunkRepo.UpdateChunkInsightAndLod(chunk.Id, insight,
            insight ? CalcLod(chunk.Pos.DistanceTo(ToLocal(camera.GlobalPosition))) : ChunkLod.JustHex);

    // 处理视野外的一圈边缘分块。如果是之前的边缘地块，需要放入刷新队列，如果是新的，则放入加载队列
    // 显示的分块向外多生成一圈，防止缺失进入视野的边缘瓦片
    private void InitOutRimChunks(Camera3D camera)
    {
        foreach (var rim in from chunkId in InsightChunkIdsNow
                 select _chunkRepo.GetById(chunkId)
                 into chunk
                 from neighbor in _chunkRepo.GetNeighbors(chunk)
                 where !InsightChunkIdsNow.Contains(neighbor.Id)
                 select neighbor)
        {
            if (!_rimChunkIds.Add(rim.Id)) continue;
            UpdateChunkInsightAndLod(rim, camera, true);
            if (_unloadSet.Contains(rim.Id))
            {
                _unloadSet.Remove(rim.Id);
                _refreshSet.Add(rim.Id);
            }
            else
                _loadSet.Add(rim.Id);
        }
    }

    private void ShowChunk(Chunk chunk)
    {
        if (_usingChunks.TryGetValue(chunk.Id, out var usingChunk))
            usingChunk.UpdateLod(chunk.Lod, false);
        else
        {
            HexGridChunk hexGridChunk;
            if (_unusedChunks.Count == 0)
            {
                // 没有空闲分块的话，初始化新的
                hexGridChunk = _gridChunkScene.Instantiate<HexGridChunk>();
                hexGridChunk.Name = $"HexGridChunk{GetChildCount()}";
                AddChild(hexGridChunk); // 必须先加入场景树，让 _Ready() 先于 Init() 执行
            }
            else
                hexGridChunk = _unusedChunks.Dequeue();

            hexGridChunk.UsedBy(chunk);
            _usingChunks.Add(chunk.Id, hexGridChunk);
        }
    }

    // 将之前显示的分块隐藏掉（归还到分块池中）
    private void HideChunk(int chunkId)
    {
        var usingChunk = _usingChunks[chunkId];
        usingChunk.HideOutOfSight();
        _usingChunks.Remove(chunkId);
        _unusedChunks.Enqueue(usingChunk);
    }

    public void ClearOldData()
    {
        _usingChunks.Clear();
        _unusedChunks.Clear();
        // 清空分块
        foreach (var child in GetChildren())
            child.QueueFree();
        // 清空动态加载分块相关数据结构
        _chunkQueryQueue.Clear();
        _visitedChunkIds.Clear();
        _rimChunkIds.Clear();
        InsightChunkIdsNow.Clear();
        _insightSetIdx = 0;
    }

    public void InitChunkNodes()
    {
        var camera = GetViewport().GetCamera3D();
        foreach (var chunk in _chunkRepo.GetAll())
        {
            var id = chunk.Id;
            // 此时拿不到真正 focusBase 的位置，暂且用相机自己的代替
            if (!IsChunkInsight(chunk, camera))
                continue;
            _loadSet.Add(id);
            UpdateChunkInsightAndLod(chunk, camera, true);
            InsightChunkIdsNow.Add(id);
        }

        InitOutRimChunks(camera);
        SetProcess(true);
    }

    private ChunkLod CalcLod(float distance)
    {
        var tileLen = _planetSettingService.Radius / _planetSettingService.Divisions;
        return distance > tileLen * 160 ? ChunkLod.JustHex :
            distance > tileLen * 80 ? ChunkLod.PlaneHex :
            distance > tileLen * 40 ? ChunkLod.SimpleHex :
            distance > tileLen * 20 ? ChunkLod.TerracesHex : ChunkLod.Full;
    }

    // 注意，判断是否在摄像机内，不是用 GetViewport().GetVisibleRect().HasPoint(camera.UnprojectPosition(chunk.Pos))
    // 因为后面要根据相机位置动态更新可见区域，上面方法这个仅仅是对应初始时的可见区域
    private bool IsChunkInsight(Chunk chunk, Camera3D camera) =>
        Mathf.Cos(chunk.Pos.Normalized().AngleTo(ToLocal(camera.GlobalPosition).Normalized()))
        > _planetSettingService.Radius / camera.GlobalPosition.Length()
        && camera.IsPositionInFrustum(ToGlobal(chunk.Pos));
}