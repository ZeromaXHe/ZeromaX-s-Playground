namespace TO.Domains.Types.Chunks

open System.Collections.Generic
open System.Diagnostics
open Godot.Abstractions.Bases

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 13:59:29
[<Interface>]
type IChunkLoader =
    inherit INode3D
    // =====【普通属性】=====
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

    abstract ResetInsightSetIdx: unit -> unit
    abstract UpdateInsightSetNextIdx: unit -> unit
    abstract GetUnusedChunk: unit -> IHexGridChunk
