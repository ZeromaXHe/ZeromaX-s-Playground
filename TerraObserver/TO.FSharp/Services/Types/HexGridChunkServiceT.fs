namespace TO.FSharp.Services.Types

open Godot.Abstractions.Extensions.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 07:59:08
type OnChunkLoaderProcessed = unit -> unit
type OnHexGridChunkProcessed = IHexGridChunk -> unit
type InitChunkNodes = unit -> unit
type UpdateInsightChunks = unit -> unit

type HexGridChunkServiceDep =
    { OnChunkLoaderProcessed: OnChunkLoaderProcessed
      OnHexGridChunkProcessed: OnHexGridChunkProcessed
      InitChunkNodes: InitChunkNodes
      UpdateInsightChunks: UpdateInsightChunks }
