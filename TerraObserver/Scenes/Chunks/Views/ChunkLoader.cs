using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using Godot.Abstractions.Extensions.Cameras;
using Godot.Abstractions.Extensions.Chunks;
using Godot.Abstractions.Extensions.Planets;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:24:42
[Tool]
public partial class ChunkLoader : Node3D, IChunkLoader
{
    #region 依赖

    public IPlanet Planet { get; set; } = null!;
    
    #endregion

    #region 事件和 Export 属性

    public event Action? Processed;
    public event Action<IHexGridChunk>? HexGridChunkGenerated; 
    [Export] private PackedScene? _gridChunkScene;

    #endregion

    #region 内部属性、变量

    // 表示当前可视分块 Set 的 _insightChunkIds 索引
    private int _insightSetIdx;
    public void ReSetInsightSetIdx() => _insightSetIdx = 0;
    public void UpdateInsightSetNextIdx() => _insightSetIdx ^= 1;
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

    public Dictionary<int, IHexGridChunk>? UsingChunks { get; } = new();
    private Queue<IHexGridChunk>? UnusedChunks { get; } = new();
    public Dictionary<int, HexGridChunk> Repo { get; } = new();

    #endregion

    #region 生命周期

    public override void _Process(double delta) => Processed?.Invoke();

    #endregion
    
    public void ClearOldData()
    {
        // 清空分块
        foreach (var child in GetChildren())
            child.QueueFree();
        UsingChunks?.Clear();
        UnusedChunks?.Clear();
        // 清空动态加载分块相关数据结构
        ChunkQueryQueue.Clear();
        VisitedChunkIds.Clear();
        RimChunkIds.Clear();
        InsightChunkIdsNow.Clear();
        ReSetInsightSetIdx();
    }

    public Stopwatch Stopwatch { get; } = new();

    public bool IsChunkUsing(int chunkId) => UsingChunks?.ContainsKey(chunkId) ?? false;

    public bool TryGetUsingChunk(int chunkId, [MaybeNullWhen(false)] out IHexGridChunk chunk)
    {
        if (UsingChunks?.TryGetValue(chunkId, out chunk) ?? false)
            return true;
        chunk = null;
        return false;
    }

    public IEnumerable<IHexGridChunk> GetAllUsingChunk() => UsingChunks?.Values ?? Enumerable.Empty<IHexGridChunk>();
    public void AddUsingChunk(int chunkId, IHexGridChunk chunk) => UsingChunks!.Add(chunkId, chunk);

    public IHexGridChunk GetUnusedChunk()
    {
        if (UnusedChunks is not null && UnusedChunks.Count != 0)
            return UnusedChunks!.Dequeue();
        // 没有空闲分块的话，初始化新的
        var hexGridChunk = _gridChunkScene!.Instantiate<HexGridChunk>();
        hexGridChunk.Name = $"HexGridChunk{GetChildCount()}";
        AddChild(hexGridChunk); // 必须先加入场景树，让 _Ready() 先于 Init() 执行
        HexGridChunkGenerated?.Invoke(hexGridChunk);
        return hexGridChunk;
    }

    public void HideChunk(int chunkId)
    {
        if (!TryGetUsingChunk(chunkId, out var usingChunk)) return;
        usingChunk.HideOutOfSight();
        UsingChunks!.Remove(chunkId);
        UnusedChunks!.Enqueue(usingChunk);
    }

    public ChunkLodEnum CalcLod(float distance)
    {
        var tileLen = Planet.Radius / Planet.Divisions;
        return distance > tileLen * 160 ? ChunkLodEnum.JustHex :
            distance > tileLen * 80 ? ChunkLodEnum.PlaneHex :
            distance > tileLen * 40 ? ChunkLodEnum.SimpleHex :
            distance > tileLen * 20 ? ChunkLodEnum.TerracesHex : ChunkLodEnum.Full;
    }

    // 注意，判断是否在摄像机内，不是用 GetViewport().GetVisibleRect().HasPoint(camera.UnprojectPosition(chunk.Pos))
    // 因为后面要根据相机位置动态更新可见区域，上面方法这个仅仅是对应初始时的可见区域
    public bool IsChunkInsight(Vector3 chunkPos, Camera3D camera) =>
        Mathf.Cos(chunkPos.Normalized().AngleTo(camera.GlobalPosition.Normalized()))
        > Planet.Radius / camera.GlobalPosition.Length()
        && camera.IsPositionInFrustum(chunkPos);
}