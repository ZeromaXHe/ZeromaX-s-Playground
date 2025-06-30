namespace TO.Domains.Types.Chunks

open System.Collections.Generic
open Godot
open TO.Domains.Types.HexSpheres

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

type ClearOldData = unit -> unit
type AddUsingChunk = ChunkId -> IHexGridChunk -> unit
type OnChunkLoaderProcessed = unit -> unit
type InitChunkNodes = unit -> unit
type OnHexGridChunkProcessed = IHexGridChunk -> unit
type UpdateInsightChunks = unit -> unit

[<Interface>]
type IChunkLoaderCommand =
    abstract ClearChunkLoaderOldData: ClearOldData
    abstract AddUsingChunk: AddUsingChunk
    abstract OnChunkLoaderProcessed: OnChunkLoaderProcessed
    abstract InitChunkNodes: InitChunkNodes
    abstract OnHexGridChunkProcessed: OnHexGridChunkProcessed
    abstract UpdateInsightChunks: UpdateInsightChunks
