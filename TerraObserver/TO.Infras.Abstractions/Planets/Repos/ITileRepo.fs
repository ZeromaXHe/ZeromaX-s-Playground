namespace TO.Infras.Abstractions.Planets.Repos

open Friflo.Engine.ECS
open TO.Infras.Abstractions.Planets.Models.Faces
open TO.Infras.Abstractions.Planets.Models.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-28 09:22:28
[<Interface>]
type ITileRepo =
    abstract TryHeadByCenterId: centerId: int -> Entity option

    abstract Add:
        centerId: int *
        chunkId: int *
        hexFaces: FaceComponent array *
        hexFaceIds: int array *
        neighborCenterIds: int array ->
            int

    abstract AllSeq: unit -> TileComponent seq
    abstract Truncate: unit -> unit
