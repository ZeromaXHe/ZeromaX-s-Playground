namespace TO.Domains.Types.Chunks

open System.Collections.Generic
open TO.Domains.Types.Godots
open TO.Domains.Types.HexSpheres.Components.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 20:58:29
[<Interface>]
[<AllowNullLiteral>]
type IChunk =
    inherit INode3D
    abstract GetTerrain: unit -> IHexMesh
    abstract GetRivers: unit -> IHexMesh
    abstract GetRoads: unit -> IHexMesh
    abstract GetWater: unit -> IHexMesh
    abstract GetWaterShore: unit -> IHexMesh
    abstract GetEstuary: unit -> IHexMesh
    abstract GetWalls: unit -> IHexMesh
    abstract Lod: ChunkLodEnum with get, set
    abstract EditingTileIds: int HashSet
