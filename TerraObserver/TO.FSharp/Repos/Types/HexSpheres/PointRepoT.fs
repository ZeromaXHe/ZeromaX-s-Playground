namespace TO.FSharp.Repos.Types.HexSpheres.PointRepoT

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Commons.Structs.HexSphereGrid
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:12:30
type ForEachPointByChunky = Chunky -> PointComponent ForEachEntity -> unit
type TryHeadPointByPosition = Chunky -> Vector3 -> Entity option
type TryHeadEntityByPointId = PointId -> Entity option
type AddPoint = Chunky -> Vector3 -> SphereAxial -> PointId
type GetNeighborByIdAndIdx = int -> int -> Entity option
type GetNeighborIdx = int -> int -> int
type GetNeighborIdsById = int -> int seq
// 因为入参有 inref，只能使用委托；因为使用委托，所以不能使用柯里化
type GetNeighborPointIds = delegate of Chunky * FaceComponent list * PointComponent inref -> PointId ResizeArray

type CreateVpTree = Chunky -> unit
type SearchNearestCenterPos = Vector3 -> Chunky -> Vector3
type TruncatePoints = unit -> unit

type PointRepoDep =
    { ForEachByChunky: ForEachPointByChunky
      TryHeadByPosition: TryHeadPointByPosition
      TryHeadEntityByPointId: TryHeadEntityByPointId
      Add: AddPoint
      GetNeighborByIdAndIdx: GetNeighborByIdAndIdx
      GetNeighborIdx: GetNeighborIdx
      GetNeighborIdsById: GetNeighborIdsById
      GetNeighborCenterPointIds: GetNeighborPointIds
      CreateVpTree: CreateVpTree
      SearchNearestCenterPos: SearchNearestCenterPos
      Truncate: TruncatePoints }
