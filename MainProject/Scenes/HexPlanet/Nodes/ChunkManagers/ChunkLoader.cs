using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-26 20:33:11
[Tool]
public partial class ChunkLoader : Node3D
{
    public ChunkLoader() => InitServices();

    [Export] private PackedScene _gridChunkScene;

    #region services

    private IChunkService _chunkService;
    private ITileService _tileService;
    private ITileShaderService _tileShaderService;
    private IPlanetSettingService _planetSettingService;

    private void InitServices()
    {
        _chunkService = Context.GetBean<IChunkService>();
        _tileService = Context.GetBean<ITileService>();
        _tileShaderService = Context.GetBean<ITileShaderService>();
        _planetSettingService = Context.GetBean<IPlanetSettingService>();
        _tileShaderService.TileExplored += ExploreFeatures;
        _chunkService.RefreshChunk += OnChunkServiceRefreshChunk;
        _chunkService.RefreshChunkTileLabel += OnChunkServiceRefreshChunkTileLabel;
        if (!Engine.IsEditorHint())
        {
            EventBus.Instance.CameraTransformed += UpdateInsightChunks;
        }
    }

    private void ExploreFeatures(int tileId)
    {
        var chunkId = _tileService.GetById(tileId).ChunkId;
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
        _chunkService.RefreshChunk -= OnChunkServiceRefreshChunk;
        _chunkService.RefreshChunkTileLabel -= OnChunkServiceRefreshChunkTileLabel;
        if (!Engine.IsEditorHint())
        {
            EventBus.Instance.CameraTransformed -= UpdateInsightChunks;
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

    // 暂时简单用时间控制视野更新频率
    private float _insightUpdateTime;
    private const float InsightUpdateInterval = 0.2f;

    private void UpdateInSightSetNextIdx() => _insightSetIdx ^= 1;

    private bool _ready;

    public override void _Ready()
    {
        _ready = true;
    }

    public override void _ExitTree() => CleanEventListeners();

    private void NewInsightChunkTask(Transform3D transform, float delta)
    {
        var camera = GetViewport().GetCamera3D();
        // 1. 查找出当前可见分块（视野剔除内的，和往外一圈的分块）
        // 2. 先初始化为最简单 LOD 网格
        // 3. 按照优先级，先加载附近的高模，再往外扩散。限制每帧的加载数量，保证不影响帧率。

        // 清理好各个数据结构，等下一次调用直接使用
        _chunkQueryQueue.Clear();
        _visitedChunkIds.Clear();
        InsightChunkIdsNow.Clear();
        UpdateInSightSetNextIdx();
        // 显示外缘分块
        InitOutRimChunks();
    }

    private void UpdateInsightChunks(Transform3D transform, float delta)
    {
        var camera = GetViewport().GetCamera3D();
        _insightUpdateTime += delta;
        if (_insightUpdateTime < InsightUpdateInterval)
            return;
        // 隐藏边缘分块
        foreach (var chunkId in _rimChunkIds)
            HideChunk(chunkId);
        _rimChunkIds.Clear();
        foreach (var preInsightChunkId in InsightChunkIdsNow)
        {
            var preInsightChunk = _chunkService.GetById(preInsightChunkId);
            _visitedChunkIds.Add(preInsightChunkId);
            if (!IsChunkInsight(preInsightChunk, camera))
            {
                // 分块不在视野范围内，隐藏它
                HideChunk(preInsightChunkId);
                continue;
            }

            InsightChunkIdsNext.Add(preInsightChunkId);
            // 刷新 Lod
            _usingChunks[preInsightChunkId]
                .UpdateLod(CalcLod(preInsightChunk.Pos.DistanceTo(ToLocal(camera.GlobalPosition))), false);
            // 分块在视野内，他的邻居才比较可能是在视野内
            // 将之前不在但现在可能在视野范围内的 id 加入带查询队列
            SearchNeighbor(preInsightChunk, InsightChunkIdsNow);
        }

        // 有种极端情况，就是新的视野范围内一个旧视野范围分块都没有！
        // 这时放开限制进行 BFS，直到找到第一个可见的分块
        // （因为我们认为新位置还是会具有空间上的相近性，BFS 应该会比随便找可见分块更好）
        if (InsightChunkIdsNext.Count == 0)
        {
            foreach (var chunk in InsightChunkIdsNow.Select(_chunkService.GetById))
                SearchNeighbor(chunk, _visitedChunkIds); // 搜索所有外缘邻居

            while (_chunkQueryQueue.Count > 0)
            {
                var chunkId = _chunkQueryQueue.Dequeue();
                var chunk = _chunkService.GetById(chunkId);
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
            var chunk = _chunkService.GetById(chunkId);
            if (!IsChunkInsight(chunk, camera)) continue;
            if (!InsightChunkIdsNext.Add(chunkId)) continue;
            ShowChunk(chunk);
            SearchNeighbor(chunk);
        }

        // 清理好各个数据结构，等下一次调用直接使用
        _chunkQueryQueue.Clear();
        _visitedChunkIds.Clear();
        InsightChunkIdsNow.Clear();
        UpdateInSightSetNextIdx();
        // 显示外缘分块
        InitOutRimChunks();
    }

    private void SearchNeighbor(Chunk chunk, HashSet<int> filterSet = null)
    {
        foreach (var neighbor in _chunkService.GetNeighbors(chunk))
        {
            if (filterSet?.Contains(neighbor.Id) ?? false) continue;
            if (_visitedChunkIds.Add(neighbor.Id))
                _chunkQueryQueue.Enqueue(neighbor.Id);
        }
    }

    // 显示的分块向外多生成一圈，防止缺失进入视野的边缘瓦片
    private void InitOutRimChunks()
    {
        foreach (var rim in from chunkId in InsightChunkIdsNow
                 select _chunkService.GetById(chunkId)
                 into chunk
                 from neighbor in _chunkService.GetNeighbors(chunk)
                 where !InsightChunkIdsNow.Contains(neighbor.Id)
                 select neighbor)
        {
            if (_rimChunkIds.Add(rim.Id))
                ShowChunk(_chunkService.GetById(rim.Id));
        }
    }

    private void ShowChunk(Chunk chunk)
    {
        if (!_usingChunks.ContainsKey(chunk.Id))
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

            hexGridChunk.UsedBy(chunk.Id, CalcLod(
                chunk.Pos.DistanceTo(ToLocal(GetViewport().GetCamera3D().GlobalPosition))));
            _usingChunks.Add(chunk.Id, hexGridChunk);
        }
        else
        {
            GD.PrintErr($"Chunk {chunk.Id} using!");
            throw new Exception("why!");
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
        _insightUpdateTime = 0;
    }

    public void InitChunkNodes()
    {
        var camera = GetViewport().GetCamera3D();
        foreach (var chunk in _chunkService.GetAll())
        {
            var id = chunk.Id;
            // 此时拿不到真正 focusBase 的位置，暂且用相机自己的代替
            if (!IsChunkInsight(chunk, camera))
                continue;
            ShowChunk(chunk);
            InsightChunkIdsNow.Add(id);
        }

        InitOutRimChunks();
    }

    private ChunkLod CalcLod(float distance)
    {
        var tileLen = _planetSettingService.Radius / _planetSettingService.Divisions;
        return distance > tileLen * 100 ? ChunkLod.JustHex :
            distance > tileLen * 50 ? ChunkLod.PlaneHex :
            distance > tileLen * 20 ? ChunkLod.SimpleHex :
            distance > tileLen * 10 ? ChunkLod.TerracesHex : ChunkLod.Full;
    }

    // 注意，判断是否在摄像机内，不是用 GetViewport().GetVisibleRect().HasPoint(camera.UnprojectPosition(chunk.Pos))
    // 因为后面要根据相机位置动态更新可见区域，上面方法这个仅仅是对应初始时的可见区域
    private bool IsChunkInsight(Chunk chunk, Camera3D camera) =>
        Mathf.Cos(chunk.Pos.Normalized().AngleTo(ToLocal(camera.GlobalPosition).Normalized()))
        > _planetSettingService.Radius / camera.GlobalPosition.Length()
        && camera.IsPositionInFrustum(ToGlobal(chunk.Pos));
}