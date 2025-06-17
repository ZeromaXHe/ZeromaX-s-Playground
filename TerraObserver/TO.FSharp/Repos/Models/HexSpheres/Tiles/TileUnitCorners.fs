namespace TO.FSharp.Repos.Models.HexSpheres.Tiles

open System.Collections
open System.Collections.Generic
open Friflo.Engine.ECS
open Godot
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Points

/// 地块单位角落
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:13:06
[<Struct>]
type TileUnitCorners =
    interface IComponent
    val Length: int
    val Corner0: Vector3
    val Corner1: Vector3
    val Corner2: Vector3
    val Corner3: Vector3
    val Corner4: Vector3
    val Corner5: Vector3

    new(corners: Vector3 array) =
        if corners.Length <> 5 && corners.Length <> 6 then
            failwith "TileUnitCorners must init with length 5 or 6"

        { Length = corners.Length
          Corner0 = corners[0]
          Corner1 = corners[1]
          Corner2 = corners[2]
          Corner3 = corners[3]
          Corner4 = corners[4]
          Corner5 = if corners.Length > 5 then corners[5] else corners[0] }

    // 只读的索引属性
    // 写在接口里的话，就没法直接用中括号了
    member this.Item
        with get idx =
            if idx < 0 || idx >= this.Length then
                failwith "TileUnitCorners invalid index"

            match idx with
            | 0 -> this.Corner0
            | 1 -> this.Corner1
            | 2 -> this.Corner2
            | 3 -> this.Corner3
            | 4 -> this.Corner4
            | 5 -> this.Corner5
            | _ -> failwith "TileUnitCorners invalid index"

    interface Vector3 IWithLength with
        override this.Length = this.Length
        override this.GetEnumerator() : Vector3 IEnumerator = new WithLengthEnumerator<Vector3>(this)
        override this.GetEnumerator() : IEnumerator = new WithLengthEnumerator<Vector3>(this)
        override this.Item idx = this[idx]
    // 获取地块的形状角落顶点（顺时针顺序）
    member this.GetCorners(unitCentroid: Vector3, radius: float32, ?size: float32) =
        let size = defaultArg size 1f

        this
        |> Seq.map (fun unitCorner -> Math3dUtil.ProjectToSphere(unitCentroid.Lerp(unitCorner, size), radius))

    // 按照 tile 高度查询 idx (顺时针第一个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    member this.GetFirstCorner(unitCentroid: Vector3, idx: int, ?radius: float32, ?size: float32) =
        let radius = defaultArg radius 1f
        let size = defaultArg size 1f
        Math3dUtil.ProjectToSphere(unitCentroid.Lerp(this[idx], size), radius)
    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    member this.GetSecondCorner(unitCentroid: Vector3, idx: int, ?radius: float32, ?size: float32) =
        this.GetFirstCorner(unitCentroid, HexIndexUtil.nextIdx this.Length idx, ?radius = radius, ?size = size)
    // 按照 tile 高度查询 idx (顺时针第一个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    member this.GetFirstSolidCorner(unitCentroid: Vector3, idx: int, ?radius: float32, ?size: float32) =
        let radius = defaultArg radius 1f
        let size = defaultArg size 1f
        this.GetFirstCorner(unitCentroid, idx, radius, size * HexMetrics.SolidFactor)
    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    member this.GetSecondSolidCorner(unitCentroid: Vector3, idx: int, ?radius: float32, ?size: float32) =
        let radius = defaultArg radius 1f
        let size = defaultArg size 1f
        this.GetSecondCorner(unitCentroid, idx, radius, size * HexMetrics.SolidFactor)

    member this.GetEdgeMiddle(unitCentroid: Vector3, idx: int, ?radius: float32, ?size: float32) =
        let corner1 = this.GetFirstCorner(unitCentroid, idx, ?radius = radius, ?size = size)
        let nextIdx = HexIndexUtil.nextIdx this.Length idx

        let corner2 =
            this.GetFirstCorner(unitCentroid, nextIdx, ?radius = radius, ?size = size)

        corner1.Lerp(corner2, 0.5f)

    member this.GetSolidEdgeMiddle(unitCentroid: Vector3, idx: int, ?radius: float32, ?size: float32) =
        let radius = defaultArg radius 1f
        let size = defaultArg size 1f
        this.GetEdgeMiddle(unitCentroid, idx, radius, size * HexMetrics.SolidFactor)

    member this.GetNeighborCommonCorners
        (neighborCenterIds: PointNeighborCenterIds, unitCentroid: Vector3, queryingCenterId: PointId, ?radius: float32) =
        let radius = defaultArg radius 1f
        let idx = neighborCenterIds |> Seq.findIndex (fun id -> id = queryingCenterId)

        if idx = -1 then
            []
        else
            let nextIdx = HexIndexUtil.nextIdx this.Length idx

            [ this.GetFirstCorner(unitCentroid, idx, radius)
              this.GetFirstCorner(unitCentroid, nextIdx, radius) ]

    // 水面
    member this.GetFirstWaterCorner(unitCentroid: Vector3, idx: int, ?radius: float32, ?size: float32) =
        let size = defaultArg size 1f
        this.GetFirstCorner(unitCentroid, idx, ?radius = radius, size = size * HexMetrics.waterFactor)

    member this.GetSecondWaterCorner(unitCentroid: Vector3, idx: int, ?radius: float32, ?size: float32) =
        let size = defaultArg size 1f
        this.GetSecondCorner(unitCentroid, idx, ?radius = radius, size = size * HexMetrics.waterFactor)
