namespace TO.Domains.Types.Chunks

open System.Collections.Generic
open System.Diagnostics
open Godot
open TO.Domains.Types.Godots
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 13:59:29
[<Interface>]
type IChunkLoader =
    inherit INode3D
    // =====【普通属性】=====
    abstract InsightSetIdx: int with get, set
    abstract InsightChunkIdsNow: int HashSet
    abstract InsightChunkIdsNext: int HashSet
    abstract ChunkQueryQueue: int Queue
    abstract VisitedChunkIds: int HashSet
    abstract RimChunkIds: int HashSet
    abstract UnloadSet: int HashSet
    abstract RefreshSet: int HashSet
    abstract LoadSet: int HashSet
    abstract UsingChunks: Dictionary<int, IHexGridChunk>
    abstract UnusedChunks: IHexGridChunk Queue
    abstract Stopwatch: Stopwatch
    abstract GetUnusedChunk: unit -> IHexGridChunk

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 22:57:29
type TryGetUsingChunk = ChunkId -> bool * IHexGridChunk
type GetAllUsingChunks = unit -> IHexGridChunk seq
type GetViewportCamera = unit -> Camera3D

[<Interface>]
type IChunkLoaderQuery =
    abstract ChunkLoader: IChunkLoader
    abstract TryGetUsingChunk: TryGetUsingChunk
    abstract GetAllUsingChunks: GetAllUsingChunks
    abstract GetViewportCamera: GetViewportCamera

type ResetInsightSetIdx = unit -> unit
type UpdateInsightSetNextIdx = unit -> unit
type ClearChunkLoaderOldData = unit -> unit
type AddUsingChunk = ChunkId -> IHexGridChunk -> unit
type OnChunkLoaderProcessed = unit -> unit
type InitChunkNodes = unit -> unit
type OnHexGridChunkProcessed = IHexGridChunk -> unit
type UpdateInsightChunks = unit -> unit

[<Interface>]
type IChunkLoaderCommand =
    abstract ResetInsightSetIdx: ResetInsightSetIdx
    abstract UpdateInsightSetNextIdx: UpdateInsightSetNextIdx
    abstract ClearChunkLoaderOldData: ClearChunkLoaderOldData
    abstract AddUsingChunk: AddUsingChunk
    abstract OnChunkLoaderProcessed: OnChunkLoaderProcessed
    abstract InitChunkNodes: InitChunkNodes
    abstract OnHexGridChunkProcessed: OnHexGridChunkProcessed
    abstract UpdateInsightChunks: UpdateInsightChunks
