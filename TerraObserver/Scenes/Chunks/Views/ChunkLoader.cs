using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using TO.Domains.Types.Chunks;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:24:42
[Tool]
public partial class ChunkLoader : Node3D, IChunkLoader
{
    #region 事件

    public event Action? Processed;
    public event Action<HexGridChunk>? HexGridChunkGenerated;

    #endregion

    #region Export 属性

    [Export] private PackedScene? _gridChunkScene;

    #endregion

    #region 普通属性

    private readonly HashSet<int>[] _insightChunkIds = [[], []];

    // 表示当前可视分块 Set 的 _insightChunkIds 索引
    public int InsightSetIdx { get; set; }
    public HashSet<int> InsightChunkIdsNow => _insightChunkIds[InsightSetIdx];
    public HashSet<int> InsightChunkIdsNext => _insightChunkIds[InsightSetIdx ^ 1];
    public Queue<int> ChunkQueryQueue { get; } = new();
    public HashSet<int> VisitedChunkIds { get; } = [];

    public HashSet<int> RimChunkIds { get; } = [];

    // 待卸载的分块 Id 集合（上轮显示本轮不显示的分块，包括边缘分块中不再显示的）
    public HashSet<int> UnloadSet { get; } = [];

    // 待刷新的分块 Id 集合（包括上轮显示本轮继续显示的分块，包括边缘分块中继续显示的）
    public HashSet<int> RefreshSet { get; } = [];

    // 待加载的分块 Id 集合（新显示分块）
    public HashSet<int> LoadSet { get; } = [];
    public Dictionary<int, IHexGridChunk> UsingChunks { get; } = new();
    public Queue<IHexGridChunk> UnusedChunks { get; } = new();
    public Stopwatch Stopwatch { get; } = new();

    #endregion

    #region 生命周期

    public override void _Process(double delta) => Processed?.Invoke();

    #endregion

    public IHexGridChunk GetUnusedChunk()
    {
        if (UnusedChunks.Count != 0)
            return UnusedChunks.Dequeue();
        // 没有空闲分块的话，初始化新的
        var hexGridChunk = _gridChunkScene!.Instantiate<HexGridChunk>();
        hexGridChunk.Name = $"HexGridChunk{GetChildCount()}";
        AddChild(hexGridChunk); // 必须先加入场景树，让 _Ready() 先于 Init() 执行
        HexGridChunkGenerated?.Invoke(hexGridChunk);
        return hexGridChunk;
    }
}