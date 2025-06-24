namespace TO.Presenters.Views.Chunks

open TO.Abstractions.Views.Chunks
open TO.Domains.Alias.HexSpheres.Chunks
open TO.Presenters.Views.Chunks

type ClearOldData = unit -> unit
type AddUsingChunk = ChunkId -> IHexGridChunk -> unit
type HideChunk = ChunkId -> unit
type SetProcess = bool -> unit
type EnqueueQueryIfNotVisited = ChunkId -> unit

[<Interface>]
type IChunkLoaderCommand =
    abstract ClearOldData: ClearOldData
    abstract AddUsingChunk: AddUsingChunk
    abstract HideChunk: HideChunk
    abstract SetProcess: SetProcess
    abstract EnqueueQueryIfNotVisited: EnqueueQueryIfNotVisited

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 10:27:24
module ChunkLoaderCommand =
    let clearOldData (chunkLoader: IChunkLoader) : ClearOldData =
        fun () ->
            // 清空分块
            for child in chunkLoader.GetChildren() do
                child.QueueFree()

            chunkLoader.UsingChunks.Clear()
            chunkLoader.UnusedChunks.Clear()
            // 清空动态加载分块相关数据结构
            chunkLoader.ChunkQueryQueue.Clear()
            chunkLoader.VisitedChunkIds.Clear()
            chunkLoader.RimChunkIds.Clear()
            chunkLoader.InsightChunkIdsNow.Clear()
            chunkLoader.ResetInsightSetIdx()

    let addUsingChunk (chunkLoader: IChunkLoader) : AddUsingChunk =
        fun chunkId chunk -> chunkLoader.UsingChunks.Add(chunkId, chunk)

    let hideChunk (chunkLoader: IChunkLoader) : HideChunk =
        fun chunkId ->
            match ChunkLoaderQuery.tryGetUsingChunk chunkLoader chunkId with
            | true, chunk ->
                chunk.Hide()
                chunkLoader.UsingChunks.Remove chunkId |> ignore
                chunkLoader.UnusedChunks.Enqueue chunk
            | false, _ -> ()

    let setProcess (chunkLoader: IChunkLoader) : SetProcess = chunkLoader.SetProcess

    let enqueueQueryIfNotVisited (chunkLoader: IChunkLoader) : EnqueueQueryIfNotVisited =
        fun (chunkId: ChunkId) ->
            if chunkLoader.VisitedChunkIds.Add chunkId then
                chunkLoader.ChunkQueryQueue.Enqueue chunkId
