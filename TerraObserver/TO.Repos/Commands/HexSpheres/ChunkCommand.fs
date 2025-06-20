namespace TO.Repos.Commands.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Components.HexSpheres.Chunks
open TO.Domains.Components.HexSpheres.Points
open TO.Domains.Alias.HexSpheres.Points
open TO.Domains.Enums.HexSpheres.Chunks

type AddChunk = PointId -> Vector3 -> NeighborCenterIds -> ChunkId

[<Interface>]
type IChunkCommand =
    abstract add: EntityStore -> AddChunk

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:55:30
module ChunkCommand =
    let add (store: EntityStore) : AddChunk =
        fun (centerId: PointId) (pos: Vector3) (neighborCenterIds: NeighborCenterIds) ->
            store
                .CreateEntity(
                    PointCenterId centerId, // 注意，此处对应的是中心点投射到单位球上的 Point id。
                    PointNeighborCenterIds neighborCenterIds, // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
                    ChunkPos pos,
                    ChunkLod ChunkLodEnum.JustHex,
                    ChunkInsight false
                )
                .Id
