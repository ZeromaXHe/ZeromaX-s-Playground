namespace TO.FSharp.Repos.Types.PointRepoT

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
type AddPoint = Chunky -> Vector3 -> SphereAxial -> int
// 因为入参有 inref，只能使用委托；因为使用委托，所以不能使用柯里化
type GetNeighborCenterPointIds =
    delegate of Chunky * FaceComponent list * PointComponent inref -> int ResizeArray

type CreateVpTree = Chunky -> unit
type SearchNearestCenterPos = Vector3 -> Chunky -> Vector3
type TruncatePoints = unit -> unit
