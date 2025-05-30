namespace TO.FSharp.Repos.Models.HexSpheres.Chunks

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-16 19:30:16
[<Struct>]
type ChunkComponent =
    interface IIndexedComponent<int> with
        member this.GetIndexedValue() = this.CenterId

    val CenterId: CenterId // 注意，此处对应的是中心点投射到单位球上的 Point id。
    val Pos: Vector3 // 这里存储是实际地块中心位置（带有星球半径）
    // val HexFaceIds: int array // 已确保顺序为顺时针方向
    val NeighborCenterIds: NeighborCenterIds // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]

    new(centerId: CenterId, pos: Vector3, neighborCenterIds: NeighborCenterIds) =
        { CenterId = centerId
          Pos = pos
          NeighborCenterIds = neighborCenterIds }

[<Struct>]
type ChunkToTileId =
    interface IRelation<int> with
        member this.GetRelationKey() = this.TileId

    val TileId: int
    new(tileId: int) = { TileId = tileId }

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 13:43:18
[<Struct>]
type TagChunk =
    interface ITag

type ChunkyDep =
    { Store: EntityStore
      TagChunk: Tags
      TagTile: Tags }
