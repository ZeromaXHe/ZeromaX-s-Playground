namespace TO.Domains.Functions.HexSpheres.Components.Tiles

open Godot
open TO.Domains.Functions.HexMetrics
open TO.Domains.Functions.HexSpheres.Components.Points
open TO.Domains.Functions.Maths
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Points
open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:41:29
module TileUnitCorners =
    let item idx (this: TileUnitCorners) =
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

    let getSeq (this: TileUnitCorners) =
        if this.Length > 5 then
            seq {
                this.Corner0
                this.Corner1
                this.Corner2
                this.Corner3
                this.Corner4
                this.Corner5
            }
        else
            seq {
                this.Corner0
                this.Corner1
                this.Corner2
                this.Corner3
                this.Corner4
            }

    // 获取地块的形状角落顶点（顺时针顺序）
    let getCornersWithSize (unitCentroid: Vector3) (radius: float32) (size: float32) (this: TileUnitCorners) =
        getSeq this
        |> Seq.map (fun unitCorner -> Math3dUtil.ProjectToSphere(unitCentroid.Lerp(unitCorner, size), radius))

    let getCorners (unitCentroid: Vector3) (radius: float32) (this: TileUnitCorners) =
        getCornersWithSize unitCentroid radius 1f this
    // 按照 tile 高度查询 idx (顺时针第一个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    let getFirstCornerWithRadiusAndSize
        (unitCentroid: Vector3)
        (idx: int)
        (radius: float32)
        (size: float32)
        (this: TileUnitCorners)
        =
        Math3dUtil.ProjectToSphere(unitCentroid.Lerp(item idx this, size), radius)

    let getFirstCornerWithRadius (unitCentroid: Vector3) (idx: int) (radius: float32) (this: TileUnitCorners) =
        getFirstCornerWithRadiusAndSize unitCentroid idx radius 1f this

    let getFirstCorner (unitCentroid: Vector3) (idx: int) (this: TileUnitCorners) =
        getFirstCornerWithRadiusAndSize unitCentroid idx 1f 1f this
    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    let getSecondCornerWithRadiusAndSize
        (unitCentroid: Vector3)
        (idx: int)
        (radius: float32)
        (size: float32)
        (this: TileUnitCorners)
        =
        getFirstCornerWithRadiusAndSize unitCentroid
        <| HexIndexUtil.nextIdx idx this.Length
        <| radius
        <| size
        <| this

    let getSecondCornerWithRadius (unitCentroid: Vector3) (idx: int) (radius: float32) (this: TileUnitCorners) =
        getSecondCornerWithRadiusAndSize unitCentroid idx radius 1f this

    let getSecondCorner (unitCentroid: Vector3) (idx: int) (this: TileUnitCorners) =
        getSecondCornerWithRadiusAndSize unitCentroid idx 1f 1f this
    // 按照 tile 高度查询 idx (顺时针第一个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    let getFirstSolidCornerWithRadiusAndSize
        (unitCentroid: Vector3)
        (idx: int)
        (radius: float32)
        (size: float32)
        (this: TileUnitCorners)
        =
        getFirstCornerWithRadiusAndSize unitCentroid idx radius
        <| size * HexMetrics.SolidFactor
        <| this

    let getFirstSolidCornerWithRadius (unitCentroid: Vector3) (idx: int) (radius: float32) (this: TileUnitCorners) =
        getFirstSolidCornerWithRadiusAndSize unitCentroid idx radius 1f this

    let getFirstSolidCorner (unitCentroid: Vector3) (idx: int) (this: TileUnitCorners) =
        getFirstSolidCornerWithRadiusAndSize unitCentroid idx 1f 1f this
    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    let getSecondSolidCornerWithRadiusAndSize
        (unitCentroid: Vector3)
        (idx: int)
        (radius: float32)
        (size: float32)
        (this: TileUnitCorners)
        =
        getSecondCornerWithRadiusAndSize unitCentroid idx radius
        <| size * HexMetrics.SolidFactor
        <| this

    let getSecondSolidCornerWithRadius (unitCentroid: Vector3) (idx: int) (radius: float32) (this: TileUnitCorners) =
        getSecondSolidCornerWithRadiusAndSize unitCentroid idx radius 1f this

    let getSecondSolidCorner (unitCentroid: Vector3) (idx: int) (this: TileUnitCorners) =
        getSecondSolidCornerWithRadiusAndSize unitCentroid idx 1f 1f this
    // 获取边的中点
    let getEdgeMiddleWithSize
        (unitCentroid: Vector3)
        (idx: int)
        (radius: float32)
        (size: float32)
        (this: TileUnitCorners)
        =
        let corner1 = getFirstCornerWithRadiusAndSize unitCentroid idx radius size this
        let nextIdx = HexIndexUtil.nextIdx idx this.Length
        let corner2 = getFirstCornerWithRadiusAndSize unitCentroid nextIdx radius size this
        corner1.Lerp(corner2, 0.5f)

    let getEdgeMiddle (unitCentroid: Vector3) (idx: int) (radius: float32) (this: TileUnitCorners) =
        getEdgeMiddleWithSize unitCentroid idx radius 1f this
    // 获取核心边中点
    let getSolidEdgeMiddleWithRadiusAndSize
        (unitCentroid: Vector3)
        (idx: int)
        (radius: float32)
        (size: float32)
        (this: TileUnitCorners)
        =
        getEdgeMiddleWithSize unitCentroid idx radius
        <| size * HexMetrics.SolidFactor
        <| this

    let getSolidEdgeMiddleWithRadius (unitCentroid: Vector3) (idx: int) (radius: float32) (this: TileUnitCorners) =
        getSolidEdgeMiddleWithRadiusAndSize unitCentroid idx radius 1f this

    let getSolidEdgeMiddle (unitCentroid: Vector3) (idx: int) (this: TileUnitCorners) =
        getSolidEdgeMiddleWithRadiusAndSize unitCentroid idx 1f 1f this
    // 获取邻居公共角
    let getNeighborCommonCornersWithRadius
        (neighborCenterIds: PointNeighborCenterIds)
        (unitCentroid: Vector3)
        (queryingCenterId: PointId)
        (radius: float32)
        (this: TileUnitCorners)
        =

        let idx =
            PointNeighborCenterIds.getSeq neighborCenterIds
            |> Seq.findIndex (fun id -> id = queryingCenterId)

        if idx = -1 then
            []
        else
            let nextIdx = HexIndexUtil.nextIdx idx this.Length

            [ getFirstCornerWithRadius unitCentroid idx radius this
              getFirstCornerWithRadius unitCentroid nextIdx radius this ]

    let getNeighborCommonCorners
        (neighborCenterIds: PointNeighborCenterIds)
        (unitCentroid: Vector3)
        (queryingCenterId: PointId)
        (this: TileUnitCorners)
        =
        getNeighborCommonCornersWithRadius neighborCenterIds unitCentroid queryingCenterId 1f this

    // 获取第一个水面角
    let getFirstWaterCornerWithRadiusAndSize
        (unitCentroid: Vector3)
        (idx: int)
        (radius: float32)
        (size: float32)
        (this: TileUnitCorners)
        =
        getFirstCornerWithRadiusAndSize unitCentroid idx radius
        <| size * HexMetrics.waterFactor
        <| this

    let getFirstWaterCornerWithRadius (unitCentroid: Vector3) (idx: int) (radius: float32) (this: TileUnitCorners) =
        getFirstWaterCornerWithRadiusAndSize unitCentroid idx radius 1f this

    let getFirstWaterCorner (unitCentroid: Vector3) (idx: int) (this: TileUnitCorners) =
        getFirstWaterCornerWithRadiusAndSize unitCentroid idx 1f 1f this
    // 获取第二个水面角
    let getSecondWaterCornerWithRadiusAndSize
        (unitCentroid: Vector3)
        (idx: int)
        (radius: float32)
        (size: float32)
        (this: TileUnitCorners)
        =
        getSecondCornerWithRadiusAndSize unitCentroid idx radius
        <| size * HexMetrics.waterFactor
        <| this

    let getSecondWaterCornerWithRadius (unitCentroid: Vector3) (idx: int) (radius: float32) (this: TileUnitCorners) =
        getSecondWaterCornerWithRadiusAndSize unitCentroid idx radius 1f this

    let getSecondWaterCorner (unitCentroid: Vector3) (idx: int) (this: TileUnitCorners) =
        getSecondWaterCornerWithRadiusAndSize unitCentroid idx 1f 1f this
