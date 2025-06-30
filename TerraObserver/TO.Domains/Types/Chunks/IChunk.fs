namespace TO.Domains.Types.Chunks

open Godot
open Godot.Abstractions.Bases
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 20:58:29
[<Interface>]
type IChunk =
    inherit INode3D
    abstract GetTerrain: unit -> IHexMesh
    abstract GetRivers: unit -> IHexMesh
    abstract GetRoads: unit -> IHexMesh
    abstract GetWater: unit -> IHexMesh
    abstract GetWaterShore: unit -> IHexMesh
    abstract GetEstuary: unit -> IHexMesh
    abstract Lod: ChunkLodEnum with get, set
    abstract ShowMesh: Mesh array -> unit
    abstract GetMeshes: unit -> Mesh array
