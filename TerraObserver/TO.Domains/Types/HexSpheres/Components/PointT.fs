namespace TO.Domains.Types.HexSpheres.Components

open Friflo.Engine.ECS
open Godot
open TO.Domains.Types.HexGridCoords
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Faces
open TO.Domains.Types.HexSpheres.Components.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 05:03:30
type TryHeadPointByPosition = Chunky -> Vector3 -> Entity option
type TryHeadEntityByPointCenterId = PointId -> Entity option
type GetPointNeighborByIdAndIdx = int -> int -> Entity option
type GetPointNeighborIdx = int -> int -> int
type GetPointNeighborIdsById = int -> int seq
type GetNeighborCenterPointIds = Chunky -> FaceComponent list -> PointComponent -> int ResizeArray
type SearchNearestPointCenterPos = Vector3 -> Chunky -> Vector3

[<Interface>]
type IPointQuery =
    abstract TryHeadByPosition: TryHeadPointByPosition
    abstract TryHeadEntityByCenterId: TryHeadEntityByPointCenterId
    abstract GetNeighborByIdAndIdx: GetPointNeighborByIdAndIdx
    abstract GetNeighborIdx: GetPointNeighborIdx
    abstract GetNeighborIdsById: GetPointNeighborIdsById
    abstract GetNeighborCenterPointIds: GetNeighborCenterPointIds
    abstract SearchNearestCenterPos: SearchNearestPointCenterPos

type AddPoint = Chunky -> Vector3 -> SphereAxial -> PointId
type CreateVpTree = Chunky -> unit

[<Interface>]
type IPointCommand =
    abstract AddPoint: AddPoint
    abstract CreateVpTree: CreateVpTree