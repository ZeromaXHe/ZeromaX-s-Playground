namespace TO.Presenters.Views.Chunks

open Godot
open TO.Abstractions.Views.Chunks
open TO.Domains.Enums.HexSpheres.Chunks
open TO.Domains.Enums.Meshes

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 17:40:22
[<AbstractClass>]
type HexGridChunkFS() =
    inherit Node3D()
    // =====【Export】=====
    abstract GetTerrain: unit -> IHexMesh
    abstract GetRivers: unit -> IHexMesh
    abstract GetRoads: unit -> IHexMesh
    abstract GetWater: unit -> IHexMesh
    abstract GetWaterShore: unit -> IHexMesh
    abstract GetEstuary: unit -> IHexMesh
    // =====【属性】=====
    member val Id = 0 with get, set
    member val Lod = ChunkLodEnum.JustHex with get, set

    member this.HideOutOfSight() =
        this.Hide()
        this.ClearOldData()
        this.Id <- 0 // 重置 id，归还给池子

    member this.ShowMesh(meshes: Mesh array) =
        this.GetTerrain().ShowMesh(meshes[int MeshType.Terrain])
        this.GetWater().ShowMesh(meshes[int MeshType.Water])
        this.GetWaterShore().ShowMesh(meshes[int MeshType.WaterShore])
        this.GetEstuary().ShowMesh(meshes[int MeshType.Estuary])

    member this.GetMeshes() =
        [| this.GetTerrain().Mesh
           this.GetWater().Mesh
           this.GetWaterShore().Mesh
           this.GetEstuary().Mesh |]

    member this.ApplyNewData() =
        this.GetTerrain().Apply()
        this.GetRivers().Apply()
        this.GetRoads().Apply()
        this.GetWater().Apply()
        this.GetWaterShore().Apply()
        this.GetEstuary().Apply()

    member this.ClearOldData() =
        this.GetTerrain().Clear()
        this.GetRivers().Clear()
        this.GetRoads().Clear()
        this.GetWater().Clear()
        this.GetWaterShore().Clear()
        this.GetEstuary().Clear()
