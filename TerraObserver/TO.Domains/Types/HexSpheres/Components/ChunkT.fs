namespace TO.Domains.Types.HexSpheres.Components

open Friflo.Engine.ECS
open Godot
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 05:40:30
type IsHandlingChunkLodGaps = ChunkLodEnum -> ChunkId -> bool
type GetChunkLod = ChunkId -> ChunkLodEnum

[<Interface>]
type IChunkQuery =
    abstract IsHandlingLodGaps: IsHandlingChunkLodGaps
    abstract GetChunkLod: GetChunkLod

type AddChunk = PointId -> Vector3 -> NeighborCenterIds -> ChunkId
type UpdateChunkInsightAndLod = Vector3 -> bool -> CommandBuffer option -> ChunkId -> unit

[<Interface>]
type IChunkCommand =
    abstract AddChunk: AddChunk
    abstract UpdateChunkInsightAndLod: UpdateChunkInsightAndLod
