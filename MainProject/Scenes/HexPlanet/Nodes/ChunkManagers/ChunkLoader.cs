using System.Collections.Generic;
using Apps.Queries.Contexts;
using Contexts;
using Domains.Models.Entities.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;
using Nodes.Abstractions.ChunkManagers;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-26 20:33:11
[Tool]
public partial class ChunkLoader : Node3D, IChunkLoader
{
    public ChunkLoader()
    {
        NodeContext.Instance.RegisterSingleton<IChunkLoader>(this);
        Context.RegisterSingletonToHolder<IChunkLoader>(this);
    }

    private bool _ready;
    public NodeEvent NodeEvent { get; } = new(process: true);
    public override void _Ready() => _ready = true;
    public override void _Process(double delta) => NodeEvent.EmitProcessed(delta);

    public override void _ExitTree()
    {
        _ready = false;
        NodeContext.Instance.DestroySingleton<IChunkLoader>();
    }

    [Export] private PackedScene? _gridChunkScene;

    public Dictionary<int, IHexGridChunk> UsingChunks { get; } = new();
    public Queue<IHexGridChunk> UnusedChunks { get; } = [];

    // 表示当前可视分块 Set 的 _insightChunkIds 索引
    private int _insightSetIdx;
    public void ReSetInsightSetIdx() => _insightSetIdx = 0;
    public void UpdateInSightSetNextIdx() => _insightSetIdx ^= 1;
    private readonly HashSet<int>[] _insightChunkIds = [[], []];
    public HashSet<int> InsightChunkIdsNow => _insightChunkIds[_insightSetIdx];
    public HashSet<int> InsightChunkIdsNext => _insightChunkIds[_insightSetIdx ^ 1];
    public Queue<int> ChunkQueryQueue { get; } = [];
    public HashSet<int> VisitedChunkIds { get; } = [];

    public HashSet<int> RimChunkIds { get; } = [];

    // 待卸载的分块 Id 集合（上轮显示本轮不显示的分块，包括边缘分块中不再显示的）
    public HashSet<int> UnloadSet { get; } = [];

    // 待刷新的分块 Id 集合（包括上轮显示本轮继续显示的分块，包括边缘分块中继续显示的）
    public HashSet<int> RefreshSet { get; } = [];

    // 待加载的分块 Id 集合（新显示分块）
    public HashSet<int> LoadSet { get; } = [];

#if !FEATURE_NEW
    public void ExploreChunkFeatures(int chunkId, int tileId)
    {
        if (UsingChunks.TryGetValue(chunkId, out var chunk))
            chunk.ExploreFeatures(tileId);
    }
#endif

    public void OnChunkServiceRefreshChunk(int id)
    {
        // 现在地图生成器也会调用，这时候分块还没创建。
        // _ready 不可或缺，否则启动失败
        if (_ready && UsingChunks.TryGetValue(id, out var chunk))
            chunk.Refresh();
    }

    public void OnChunkServiceRefreshChunkTileLabel(int chunkId, int tileId, string text)
    {
        if (UsingChunks.TryGetValue(chunkId, out var chunk))
            chunk.RefreshTileLabel(tileId, text);
    }

    public void ShowChunk(Chunk chunk)
    {
        if (UsingChunks.TryGetValue(chunk.Id, out var usingChunk))
            usingChunk.UpdateLod(chunk.Lod, false);
        else
        {
            IHexGridChunk hexGridChunk;
            if (UnusedChunks.Count == 0)
            {
                // 没有空闲分块的话，初始化新的
                hexGridChunk = _gridChunkScene!.Instantiate<HexGridChunk>();
                hexGridChunk.Name = $"HexGridChunk{GetChildCount()}";
                AddChild(hexGridChunk as HexGridChunk); // 必须先加入场景树，让 _Ready() 先于 Init() 执行
            }
            else
                hexGridChunk = UnusedChunks.Dequeue();

            hexGridChunk.UsedBy(chunk);
            UsingChunks.Add(chunk.Id, hexGridChunk);
        }
    }

    // 将之前显示的分块隐藏掉（归还到分块池中）
    public void HideChunk(int chunkId)
    {
        var usingChunk = UsingChunks[chunkId];
        usingChunk.HideOutOfSight();
        UsingChunks.Remove(chunkId);
        UnusedChunks.Enqueue(usingChunk);
    }

    public void ClearOldData()
    {
        UsingChunks.Clear();
        UnusedChunks.Clear();
        // 清空分块
        foreach (var child in GetChildren())
            child.QueueFree();
        // 清空动态加载分块相关数据结构
        ChunkQueryQueue.Clear();
        VisitedChunkIds.Clear();
        RimChunkIds.Clear();
        InsightChunkIdsNow.Clear();
        ReSetInsightSetIdx();
    }
}