namespace TO.Domains.Types.HexSpheres.Components.Points

open Friflo.Engine.ECS
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:16:06
[<Struct>]
type PointNeighborCenterIds =
    interface IComponent
    val Length: int
    val NeighborCenterId0: PointId
    val NeighborCenterId1: PointId
    val NeighborCenterId2: PointId
    val NeighborCenterId3: PointId
    val NeighborCenterId4: PointId
    val NeighborCenterId5: PointId

    new(centerIds: NeighborCenterIds) =
        if centerIds.Length <> 5 && centerIds.Length <> 6 then
            failwith "TileNeighborCenterIds must init with length 5 or 6"

        { Length = centerIds.Length
          NeighborCenterId0 = centerIds[0]
          NeighborCenterId1 = centerIds[1]
          NeighborCenterId2 = centerIds[2]
          NeighborCenterId3 = centerIds[3]
          NeighborCenterId4 = centerIds[4]
          NeighborCenterId5 = if centerIds.Length > 5 then centerIds[5] else centerIds[0] }
