namespace TO.Domains.Types.HexSpheres.Components

open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Faces

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 05:36:30
type AddTile = PointId -> ChunkId -> FaceComponent array -> HexFaceIds -> NeighborCenterIds -> TileId

[<Interface>]
type ITileCommand =
    abstract AddTile: AddTile