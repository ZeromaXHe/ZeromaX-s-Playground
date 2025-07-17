namespace TO.Domains.Functions.HexSpheres.Components.Points

open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:27:29
module PointNeighborCenterIds =
    let item idx (this: PointNeighborCenterIds) =
        if idx < 0 || idx >= this.Length then
            failwith "TileNeighborCenterIds invalid index"

        match idx with
        | 0 -> this.NeighborCenterId0
        | 1 -> this.NeighborCenterId1
        | 2 -> this.NeighborCenterId2
        | 3 -> this.NeighborCenterId3
        | 4 -> this.NeighborCenterId4
        | 5 -> this.NeighborCenterId5
        | _ -> failwith "TileNeighborCenterIds invalid index"

    // 邻居
    let getNeighborIdx (neighborCenterId: PointId) (this: PointNeighborCenterIds) =
        if this.NeighborCenterId0 = neighborCenterId then
            0
        elif this.NeighborCenterId1 = neighborCenterId then
            1
        elif this.NeighborCenterId2 = neighborCenterId then
            2
        elif this.NeighborCenterId3 = neighborCenterId then
            3
        elif this.NeighborCenterId4 = neighborCenterId then
            4
        elif this.Length = 6 && this.NeighborCenterId5 = neighborCenterId then
            5
        else
            -1

    let isNeighbor (neighborCenterId: PointId) (this: PointNeighborCenterIds) =
        this.NeighborCenterId0 = neighborCenterId
        || this.NeighborCenterId1 = neighborCenterId
        || this.NeighborCenterId2 = neighborCenterId
        || this.NeighborCenterId3 = neighborCenterId
        || this.NeighborCenterId4 = neighborCenterId
        || (this.Length = 6 && this.NeighborCenterId5 = neighborCenterId)
