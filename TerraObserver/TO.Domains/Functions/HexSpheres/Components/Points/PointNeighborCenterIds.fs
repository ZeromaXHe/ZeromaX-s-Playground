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

    let getSeq (this: PointNeighborCenterIds) =
        if this.Length = 5 then
            seq {
                this.NeighborCenterId0
                this.NeighborCenterId1
                this.NeighborCenterId2
                this.NeighborCenterId3
                this.NeighborCenterId4
            }
        else 
            seq {
                this.NeighborCenterId0
                this.NeighborCenterId1
                this.NeighborCenterId2
                this.NeighborCenterId3
                this.NeighborCenterId4
                this.NeighborCenterId5
            }

    // 邻居
    let getNeighborIdx (neighborCenterId: PointId) (this: PointNeighborCenterIds) =
        getSeq this |> Seq.findIndex (fun id -> id = neighborCenterId)

    let isNeighbor (neighborCenterId: PointId) (this: PointNeighborCenterIds) =
        getSeq this |> Seq.contains neighborCenterId
