namespace TO.Domains.Functions.Chunks

open Godot
open TO.Domains.Types.Chunks
open TO.Domains.Types.HexMeshes
open TO.Domains.Types.HexSpheres.Components

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 16:21:30
module HexGridChunkCommand =
    let clearOldData (env: 'E when 'E :> IFeatureCommand and 'E :> ITileQuery) (this: IHexGridChunk) =
        this.GetTerrain() |> HexMeshCommand.clear
        this.GetRivers() |> HexMeshCommand.clear
        this.GetRoads() |> HexMeshCommand.clear
        this.GetWater() |> HexMeshCommand.clear
        this.GetWaterShore() |> HexMeshCommand.clear
        this.GetEstuary() |> HexMeshCommand.clear
        this.GetWalls() |> HexMeshCommand.clear

        for tile in env.GetTilesByChunkId this.Id do
            env.HideFeatures tile.Id false
            env.DeleteFeatures tile.Id false

    let hideOutOfSight env (this: IHexGridChunk) =
        this.Hide()
        clearOldData env this
        this.Id <- 0 // 重置 id，归还给池子

    let applyNewData (this: IHexGridChunk) =
        this.GetTerrain() |> HexMeshCommand.apply
        this.GetRivers() |> HexMeshCommand.apply
        this.GetRoads() |> HexMeshCommand.apply
        this.GetWater() |> HexMeshCommand.apply
        this.GetWaterShore() |> HexMeshCommand.apply
        this.GetEstuary() |> HexMeshCommand.apply
        this.GetWalls() |> HexMeshCommand.apply

    let showMesh (meshes: Mesh array) (this: IHexGridChunk) =
        this.GetTerrain() |> HexMeshCommand.showMesh meshes[int MeshType.Terrain]
        this.GetWater() |> HexMeshCommand.showMesh meshes[int MeshType.Water]
        this.GetWaterShore() |> HexMeshCommand.showMesh meshes[int MeshType.WaterShore]
        this.GetEstuary() |> HexMeshCommand.showMesh meshes[int MeshType.Estuary]

    let getMeshes (this: IHexGridChunk) =
        [| this.GetTerrain().Mesh
           this.GetWater().Mesh
           this.GetWaterShore().Mesh
           this.GetEstuary().Mesh |]
