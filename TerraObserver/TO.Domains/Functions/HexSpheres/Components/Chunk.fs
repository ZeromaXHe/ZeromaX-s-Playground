namespace TO.Domains.Functions.HexSpheres.Components

open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexSpheres
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Chunks
open TO.Domains.Types.HexSpheres.Components.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 15:52:22
module Chunk =
    let calcLod (tileLen: float32) =
        function
        | distance when distance > tileLen * 160f -> ChunkLodEnum.JustHex
        | distance when distance > tileLen * 80f -> ChunkLodEnum.PlaneHex
        | distance when distance > tileLen * 40f -> ChunkLodEnum.SimpleHex
        | distance when distance > tileLen * 20f -> ChunkLodEnum.TerracesHex
        | _ -> ChunkLodEnum.Full

    // 注意，判断是否在摄像机内，不是用 GetViewport().GetVisibleRect().HasPoint(camera.UnprojectPosition(chunk.Pos))
    // 因为后面要根据相机位置动态更新可见区域，上面方法这个仅仅是对应初始时的可见区域
    let isChunkInsight (chunkPos: Vector3) (camera: Camera3D) (radius: float32) =
        let cosine = radius / camera.GlobalPosition.Length()

        Mathf.Cos(chunkPos.Normalized().AngleTo(camera.GlobalPosition.Normalized())) > cosine
        && camera.IsPositionInFrustum chunkPos

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 18:43:19
module ChunkQuery =
    let isHandlingLodGaps (env: 'E when 'E :> IEntityStoreQuery and 'E :> IPointQuery) : IsHandlingChunkLodGaps =
        fun (lod: ChunkLodEnum) (chunkId: ChunkId) ->
            (lod = ChunkLodEnum.PlaneHex
             && env.GetNeighborIdsById chunkId
                |> Seq.exists (fun id -> env.GetEntityById(id).GetComponent<ChunkLod>().Lod >= ChunkLodEnum.SimpleHex))
            || (lod = ChunkLodEnum.TerracesHex
                && env.GetNeighborIdsById chunkId
                   |> Seq.exists (fun id -> env.GetEntityById(id).GetComponent<ChunkLod>().Lod = ChunkLodEnum.Full))

    let getLod (env: #IEntityStoreQuery) : GetChunkLod =
        fun (chunkId: ChunkId) -> env.GetEntityById(chunkId).GetComponent<ChunkLod>().Lod

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:55:30
module ChunkCommand =
    let add (env: #IEntityStoreQuery) : AddChunk =
        fun (centerId: PointId) (pos: Vector3) (neighborCenterIds: NeighborCenterIds) ->
            env.EntityStore
                .CreateEntity(
                    PointCenterId centerId, // 注意，此处对应的是中心点投射到单位球上的 Point id。
                    PointNeighborCenterIds neighborCenterIds, // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
                    ChunkPos pos,
                    ChunkLod ChunkLodEnum.JustHex,
                    ChunkInsight false
                )
                .Id

    let updateChunkInsightAndLod
        (env: 'E when 'E :> IEntityStoreQuery and 'E :> IPlanetConfigQuery)
        : UpdateChunkInsightAndLod =
        fun (cameraPos: Vector3) (insight: bool) (cbOpt: CommandBuffer option) (chunkId: ChunkId) ->
            let tileLen = env.GetTileLen()
            let chunkPos = env.GetEntityById(chunkId).GetComponent<ChunkPos>().Pos

            let lodEnum =
                if insight then
                    Chunk.calcLod tileLen <| chunkPos.DistanceTo cameraPos
                else
                    ChunkLodEnum.JustHex

            let chunkInsight = ChunkInsight insight
            let chunkLod = ChunkLod lodEnum
            // 更新组件
            match cbOpt with
            | Some commandBuffer ->
                commandBuffer.AddComponent<ChunkInsight>(chunkId, &chunkInsight)
                commandBuffer.AddComponent<ChunkLod>(chunkId, &chunkLod)
            | None ->
                let chunk = env.GetEntityById(chunkId)
                chunk.AddComponent<ChunkInsight>(&chunkInsight) |> ignore
                chunk.AddComponent<ChunkLod>(&chunkLod) |> ignore
