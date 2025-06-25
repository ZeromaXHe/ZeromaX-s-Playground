namespace TO.Presenters.Views.Chunks

open System.Collections.Generic
open System.Diagnostics
open Godot
open TO.Abstractions.Views.Chunks
open TO.Domains.Alias.HexSpheres.Chunks

type GetInsightChunkIdsNow = unit -> ChunkId HashSet
type GetChunkIdsNext = unit -> ChunkId HashSet
type GetChunkQueryQueue = unit -> ChunkId Queue
type GetVisitedChunkIds = unit -> ChunkId HashSet
type GetRimChunkIds = unit -> ChunkId HashSet
type GetUnloadSet = unit -> ChunkId HashSet
type GetRefreshSet = unit -> ChunkId HashSet
type GetLoadSet = unit -> ChunkId HashSet
type GetUsingChunks = unit -> Dictionary<ChunkId, IHexGridChunk>
type GetUnusedChunk = unit -> IHexGridChunk
type GetStopwatch = unit -> Stopwatch
type UpdateInsightSetNextIdx = unit -> unit
type TryGetUsingChunk = ChunkId -> bool * IHexGridChunk
type GetAllUsingChunks = unit -> IHexGridChunk seq
type GetViewportCamera = unit -> Camera3D

[<Interface>]
type IChunkLoaderQuery =
    abstract GetInsightChunkIdsNow: GetInsightChunkIdsNow
    abstract GetInsightChunkIdsNext: GetChunkIdsNext
    abstract GetChunkQueryQueue: GetChunkQueryQueue
    abstract GetVisitedChunkIds: GetVisitedChunkIds
    abstract GetRimChunkIds: GetRimChunkIds
    abstract GetUnloadSet: GetUnloadSet
    abstract GetRefreshSet: GetRefreshSet
    abstract GetLoadSet: GetLoadSet
    abstract GetUsingChunks: GetUsingChunks
    abstract GetStopwatch: GetStopwatch
    abstract GetUnusedChunk: GetUnusedChunk
    abstract UpdateInsightSetNextIdx: UpdateInsightSetNextIdx
    abstract TryGetUsingChunk: TryGetUsingChunk
    abstract GetAllUsingChunks: GetAllUsingChunks
    abstract GetViewportCamera: GetViewportCamera

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 09:58:24
module ChunkLoaderQuery =
    let getInsightChunkIdsNow (chunkLoader: IChunkLoader) : GetInsightChunkIdsNow =
        fun () -> chunkLoader.InsightChunkIdsNow

    let getInsightChunkIdsNext (chunkLoader: IChunkLoader) : GetChunkIdsNext =
        fun () -> chunkLoader.InsightChunkIdsNext

    let getChunkQueryQueue (chunkLoader: IChunkLoader) : GetChunkQueryQueue = fun () -> chunkLoader.ChunkQueryQueue

    let getVisitedChunkIds (chunkLoader: IChunkLoader) : GetVisitedChunkIds = fun () -> chunkLoader.VisitedChunkIds

    let getRimChunkIds (chunkLoader: IChunkLoader) : GetRimChunkIds = fun () -> chunkLoader.RimChunkIds
    let getUnloadSet (chunkLoader: IChunkLoader) : GetUnloadSet = fun () -> chunkLoader.UnloadSet
    let getRefreshSet (chunkLoader: IChunkLoader) : GetRefreshSet = fun () -> chunkLoader.RefreshSet
    let getLoadSet (chunkLoader: IChunkLoader) : GetLoadSet = fun () -> chunkLoader.LoadSet
    let getUsingChunks (chunkLoader: IChunkLoader) : GetUsingChunks = fun () -> chunkLoader.UsingChunks
    let getStopwatch (chunkLoader: IChunkLoader) : GetStopwatch = fun () -> chunkLoader.Stopwatch
    let updateInsightSetNextIdx (chunkLoader: IChunkLoader) : UpdateInsightSetNextIdx = fun () -> chunkLoader.UpdateInsightSetNextIdx()
    let getUnusedChunk (chunkLoader: IChunkLoader) : GetUnusedChunk = fun () -> chunkLoader.GetUnusedChunk()

    let tryGetUsingChunk (chunkLoader: IChunkLoader) : TryGetUsingChunk =
        fun chunkId -> chunkLoader.UsingChunks.TryGetValue(chunkId)

    let getAllUsingChunks (chunkLoader: IChunkLoader) : GetAllUsingChunks =
        fun () -> seq chunkLoader.UsingChunks.Values

    let getViewportCamera (chunkLoader: IChunkLoader) : GetViewportCamera =
        fun () -> chunkLoader.GetViewport().GetCamera3D()
