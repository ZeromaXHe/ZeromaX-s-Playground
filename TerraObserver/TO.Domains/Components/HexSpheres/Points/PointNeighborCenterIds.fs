namespace TO.Domains.Components.HexSpheres.Points

open Friflo.Engine.ECS
open TO.Domains.Alias.HexSpheres.Points


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

    member this.Item
        with get idx =
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

module PointNeighborCenterIds =
    let getSeq (this: PointNeighborCenterIds) =
        let s =
            seq {
                this.NeighborCenterId0
                this.NeighborCenterId1
                this.NeighborCenterId2
                this.NeighborCenterId3
                this.NeighborCenterId4
            }

        if this.Length > 5 then
            Seq.append s <| seq { this.NeighborCenterId5 }
        else
            s

    // 邻居
    let getNeighborIdx (this: PointNeighborCenterIds) (neighborCenterId: PointId) =
        getSeq this |> Seq.findIndex (fun id -> id = neighborCenterId)

    let isNeighbor (this: PointNeighborCenterIds) (neighborCenterId: PointId) =
        getSeq this |> Seq.contains neighborCenterId
