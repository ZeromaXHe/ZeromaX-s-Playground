namespace TO.FSharp.Repos.Models.HexSpheres.Tiles

open Friflo.Engine.ECS
open TO.FSharp.Repos.Models.HexSpheres.Points

type TileId = int
type ChunkId = int
type HexFaceIds = FaceId array

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 20:11:18
[<Struct>]
type TagTile =
    interface ITag
