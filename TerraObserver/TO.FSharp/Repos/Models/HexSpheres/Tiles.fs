namespace TO.FSharp.Repos.Models.HexSpheres.Tiles

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Faces

type CenterId = int
type ChunkId = int
type HexFaces = FaceComponent array
type HexFaceIds = int array
type NeighborCenterIds = CenterId array

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-19 13:01:19
[<Struct>]
type TileComponent =
    interface int IIndexedComponent with
        member this.GetIndexedValue() = this.CenterId

    val CenterId: CenterId // 注意，此处对应的是中心点投射到单位球上的 Point id。
    val ChunkId: ChunkId
    val UnitCentroid: Vector3 // 单位重心（顶点坐标的算术平均）
    val UnitCorners: Vector3 array // 单位角落
    val HexFaceIds: HexFaceIds // 已确保顺序为顺时针方向
    // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
    val NeighborCenterIds: NeighborCenterIds

    new
        (
            centerId: CenterId,
            chunkId: ChunkId,
            unitCentroid: Vector3,
            unitCorners: Vector3 array,
            hexFaceIds: HexFaceIds,
            neighborCenterIds: NeighborCenterIds
        ) =
        { CenterId = centerId
          ChunkId = chunkId
          UnitCentroid = unitCentroid
          UnitCorners = unitCorners
          HexFaceIds = hexFaceIds
          NeighborCenterIds = neighborCenterIds }

    member this.IsPentagon() = this.HexFaceIds.Length = 5

    member this.PreviousIdx(idx: int) =
        if idx = 0 then this.HexFaceIds.Length - 1 else idx - 1

    member this.Previous2Idx(idx: int) =
        if idx <= 1 then
            this.HexFaceIds.Length - 2 + idx
        else
            idx - 2

    member this.NextIdx(idx: int) = (idx + 1) % this.HexFaceIds.Length
    member this.Next2Idx(idx: int) = (idx + 2) % this.HexFaceIds.Length
    member this.OppositeIdx(idx: int) = (idx + 3) % this.HexFaceIds.Length
    member this.GetCentroid(radius: float32) = this.UnitCentroid * radius
    // 获取地块的形状角落顶点（顺时针顺序）
    member this.GetCorners(radius: float32, ?size: float32) =
        let size = defaultArg size 1f
        let unitCentroid = this.UnitCentroid // 闭包里不能存在 this

        this.UnitCorners
        |> Array.map (fun unitCorner -> Math3dUtil.ProjectToSphere(unitCentroid.Lerp(unitCorner, size), radius))
    // 按照 tile 高度查询 idx (顺时针第一个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    member this.GetFirstCorner(idx: int, ?radius: float32, ?size: float32) =
        let radius = defaultArg radius 1f
        let size = defaultArg size 1f
        Math3dUtil.ProjectToSphere(this.UnitCentroid.Lerp(this.UnitCorners[idx], size), radius)
    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    member this.GetSecondCorner(idx: int, ?radius: float32, ?size: float32) =
        this.GetFirstCorner(this.NextIdx idx, ?radius = radius, ?size = size)
    // 按照 tile 高度查询 idx (顺时针第一个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    member this.GetFirstSolidCorner(idx: int, ?radius: float32, ?size: float32) =
        let radius = defaultArg radius 1f
        let size = defaultArg size 1f
        this.GetFirstCorner(idx, radius, size * HexMetrics.solidFactor)
    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    member this.GetSecondSolidCorner(idx: int, ?radius: float32, ?size: float32) =
        let radius = defaultArg radius 1f
        let size = defaultArg size 1f
        this.GetSecondCorner(idx, radius, size * HexMetrics.solidFactor)

    member this.GetEdgeMiddle(idx: int, ?radius: float32, ?size: float32) =
        let corner1 = this.GetFirstCorner(idx, ?radius = radius, ?size = size)
        let corner2 = this.GetFirstCorner(this.NextIdx idx, ?radius = radius, ?size = size)
        corner1.Lerp(corner2, 0.5f)

    member this.GetSolidEdgeMiddle(idx: int, ?radius: float32, ?size: float32) =
        let radius = defaultArg radius 1f
        let size = defaultArg size 1f
        this.GetEdgeMiddle(idx, radius, size * HexMetrics.solidFactor)

    member this.GetNeighborCommonCorners(neighbor: TileComponent, ?radius: float32) =
        let radius = defaultArg radius 1f

        let idx =
            this.NeighborCenterIds |> Array.findIndex (fun id -> id = neighbor.CenterId)

        if idx = -1 then
            []
        else
            [ this.GetFirstCorner(idx, radius)
              this.GetFirstCorner(this.NextIdx(idx), radius) ]
    // 水面
    member this.GetFirstWaterCorner(idx: int, ?radius: float32, ?size: float32) =
        this.GetFirstCorner(idx, ?radius = radius, ?size = size)

    member this.GetSecondWaterCorner(idx: int, ?radius: float32, ?size: float32) =
        this.GetSecondCorner(idx, ?radius = radius, ?size = size)
    // 邻居
    member this.GetNeighborIdx(neighbor: TileComponent) =
        this.NeighborCenterIds |> Array.findIndex (fun id -> id = neighbor.CenterId)

    member this.IsNeighbor(tile: TileComponent) =
        this.NeighborCenterIds |> Array.contains tile.CenterId


/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 20:11:18
[<Struct>]
type TagTile =
    interface ITag
