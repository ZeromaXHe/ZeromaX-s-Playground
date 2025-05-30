namespace TO.FSharp.Repos.Types.FaceRepoT

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:26:30
type ForEachFaceByChunky = Chunky -> FaceComponent ForEachEntity -> unit
type AddFace = Chunky -> Vector3 -> Vector3 -> Vector3 -> int
type GetOrderedFaces = PointComponent -> Entity -> Entity list
type TruncateFaces = unit -> unit

type FaceRepoDep =
    { ForEachByChunky: ForEachFaceByChunky
      Add: AddFace
      GetOrderedFaces: GetOrderedFaces
      Truncate: TruncateFaces }
