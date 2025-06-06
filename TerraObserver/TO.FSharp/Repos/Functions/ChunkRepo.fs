namespace TO.FSharp.Repos.Functions

open Friflo.Engine.ECS
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Types.ChunkRepoT

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:55:30
module ChunkRepo =
    let add (store: EntityStore) : AddChunk =
        fun centerId pos neighborCenterIds ->
            store
                .CreateEntity(
                    PointCenterId centerId, // 注意，此处对应的是中心点投射到单位球上的 Point id。
                    ChunkComponent pos,
                    PointNeighborCenterIds neighborCenterIds // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
                )
                .Id

    let truncate (store: EntityStore) : TruncateChunks =
        fun () -> FrifloEcsUtil.truncate <| store.Query<ChunkComponent>()

    let getDependency store : ChunkRepoDep =
        { Add = add store
          Truncate = truncate store }
