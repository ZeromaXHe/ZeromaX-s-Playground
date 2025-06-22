namespace TO.Presenters.Views.Chunks

open System.Collections.Generic
open System.Diagnostics
open Godot
open TO.Abstractions.Views.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 15:26:22
[<AbstractClass>]
type ChunkLoaderFS() =
    inherit Node3D()
    // 表示当前可视分块 Set 的 _insightChunkIds 索引
    let mutable insightSetIdx = 0
    let resetInsightSetIdx () = insightSetIdx <- 0
    let insightChunkIds: int HashSet array = [| HashSet<int>(); HashSet<int>() |]
    // =====【属性】=====
    member this.UpdateInsightSetNextIdx() = insightSetIdx <- insightSetIdx ^^^ 1
    member this.InsightChunkIdsNow = insightChunkIds[insightSetIdx]
    member this.InsightChunkIdsNext = insightChunkIds[insightSetIdx ^^^ 1]
    member val ChunkQueryQueue = Queue<int>()
    member val VisitedChunkIds = HashSet<int>()
    member val RimChunkIds = HashSet<int>()
    // 待卸载的分块 Id 集合（上轮显示本轮不显示的分块，包括边缘分块中不再显示的）
    member val UnloadSet = HashSet<int>()
    // 待刷新的分块 Id 集合（包括上轮显示本轮继续显示的分块，包括边缘分块中继续显示的）
    member val RefreshSet = HashSet<int>()
    // 待加载的分块 Id 集合（新显示分块）
    member val LoadSet = HashSet<int>()
    member val UsingChunks = Dictionary<int, IHexGridChunk>()
    member val UnusedChunks = Queue<IHexGridChunk>()
    member this.Stopwatch = Stopwatch()

    member this.ClearOldData() =
        // 清空分块
        for child in this.GetChildren() do
            child.QueueFree()

        this.UsingChunks.Clear()
        this.UnusedChunks.Clear()
        // 清空动态加载分块相关数据结构
        this.ChunkQueryQueue.Clear()
        this.VisitedChunkIds.Clear()
        this.RimChunkIds.Clear()
        this.InsightChunkIdsNow.Clear()
        resetInsightSetIdx ()

    member this.IsChunkUsing(chunkId: int) = this.UsingChunks.ContainsKey(chunkId)

    member this.TryGetUsingChunk(chunkId: int, chunk: IHexGridChunk outref) =
        match this.UsingChunks.TryGetValue(chunkId) with
        | true, result ->
            chunk <- result
            true
        | false, _ -> false

    member this.GetAllUsingChunk() = seq this.UsingChunks.Values
    member this.AddUsingChunk(chunkId: int, chunk: IHexGridChunk) = this.UsingChunks.Add(chunkId, chunk)
    // HexGridChunk 工厂
    abstract GetUnusedChunk: unit -> IHexGridChunk

    member this.HideChunk(chunkId: int) =
        match this.TryGetUsingChunk(chunkId) with
        | true, chunk ->
            chunk.Hide()
            this.UsingChunks.Remove chunkId |> ignore
            this.UnusedChunks.Enqueue chunk
        | false, _ -> ()
