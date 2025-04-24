using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Contexts;
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
        Context.RegisterToHolder<IChunkLoader>(this);
    }
    public NodeEvent NodeEvent { get; } = new(process: true);
    public override void _Process(double delta) => NodeEvent.EmitProcessed(delta);

    [Export] private PackedScene? _gridChunkScene;

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

    public IHexGridChunk InstantiateHexGridChunk()
    {
        // 没有空闲分块的话，初始化新的
        var hexGridChunk = _gridChunkScene!.Instantiate<HexGridChunk>();
        hexGridChunk.Name = $"HexGridChunk{GetChildCount()}";
        AddChild(hexGridChunk); // 必须先加入场景树，让 _Ready() 先于 Init() 执行
        return hexGridChunk;
    }

    public void ClearOldData()
    {
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
    
    private readonly Stopwatch _stopwatch = new();
    public void OnProcessed(double delta, Action<int> showChunk, Action<int> hideChunk)
    {
        _stopwatch.Restart();
        var allClear = true;
        var limitCount = Mathf.Min(20, LoadSet.Count);
#if MY_DEBUG
        var loadCount = 0;
#endif
        // 限制加载耗时（但加载优先级最高）
        while (limitCount > 0 && _stopwatch.ElapsedMilliseconds <= 14)
        {
            var chunkId = LoadSet.First();
            LoadSet.Remove(chunkId);
            showChunk.Invoke(chunkId);
            limitCount--;
#if MY_DEBUG
            loadCount++;
#endif
        }

        if (LoadSet.Count > 0)
            allClear = false;
        var loadTime = _stopwatch.ElapsedMilliseconds;
        var totalTime = loadTime;
        _stopwatch.Restart();

        limitCount = Math.Min(20, RefreshSet.Count);
#if MY_DEBUG
        var refreshCount = 0;
#endif
        // 限制刷新耗时（刷新优先级其次）
        while (limitCount > 0 && totalTime + _stopwatch.ElapsedMilliseconds <= 14)
        {
            var chunkId = RefreshSet.First();
            RefreshSet.Remove(chunkId);
            showChunk.Invoke(chunkId);
            limitCount--;
#if MY_DEBUG
            refreshCount++;
#endif
        }

        if (RefreshSet.Count > 0)
            allClear = false;
        var refreshTime = _stopwatch.ElapsedMilliseconds;
        totalTime += refreshTime;
        _stopwatch.Restart();

        limitCount = Math.Min(100, UnloadSet.Count);
#if MY_DEBUG
        var unloadCount = 0;
#endif
        // 限制卸载耗时（卸载优先级最低）
        while (limitCount > 0 && totalTime + _stopwatch.ElapsedMilliseconds <= 14)
        {
            var chunkId = UnloadSet.First();
            UnloadSet.Remove(chunkId);
            hideChunk.Invoke(chunkId);
            limitCount--;
#if MY_DEBUG
            unloadCount++;
#endif
        }

        if (UnloadSet.Count > 0)
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
}