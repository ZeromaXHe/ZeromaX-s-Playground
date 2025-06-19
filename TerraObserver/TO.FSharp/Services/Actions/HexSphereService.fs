namespace TO.FSharp.Services.Actions

open Friflo.Engine.ECS
open Godot
open TO.Abstractions.Planets
open TO.Domains.Components.HexSpheres.Chunks
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Components.HexSpheres.Points
open TO.Domains.Components.HexSpheres.Tiles
open TO.Domains.Functions.HexSpheres
open TO.Domains.Relations.HexSpheres.Chunks
open TO.FSharp.Repos.Commands.Friflos
open TO.FSharp.Repos.Commands.HexSpheres
open TO.FSharp.Repos.Data.Commons
open TO.FSharp.Repos.Data.HexSpheres
open TO.FSharp.Repos.Data.PathFindings
open TO.FSharp.Repos.Data.Shaders
open TO.FSharp.Repos.Queries.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 12:46:30
module HexSphereService =
    let clearOldData (env: #IEntityStore) =
        EntityStoreCommand.truncate<PointComponent> env
        EntityStoreCommand.truncate<FaceComponent> env
        EntityStoreCommand.truncate<TileUnitCentroid> env
        EntityStoreCommand.truncate<ChunkPos> env

    let private initChunks (viewEnv: #IPlanetEnv) (dataEnv: 'DE when 'DE :> IEntityStore and 'DE :> IChunkyVpTrees) =
        let time = Time.GetTicksMsec()
        let planet = viewEnv.Planet
        HexSphereInitCommand.initPointsAndFaces dataEnv true planet.ChunkDivisions

        let tag = ChunkFunction.chunkyTag true

        dataEnv.EntityStore
            .Query<PointComponent>()
            .AllTags(&tag)
            .ForEachEntity(fun pComp pEntity ->
                let _, _, neighborCenterIds =
                    HexSphereQuery.getHexFacesAndNeighborCenterIds dataEnv true &pComp &pEntity

                ChunkCommand.add dataEnv pEntity.Id
                <| pComp.Position * (planet.Radius + planet.MaxHeight)
                <| neighborCenterIds
                |> ignore)

        PointCommand.createVpTree dataEnv true

        GD.Print($"InitChunks chunkDivisions {planet.ChunkDivisions}, cost: {Time.GetTicksMsec() - time} ms")

    let private initTiles (viewEnv: #IPlanetEnv) (dataEnv: 'DE when 'DE :> IEntityStore and 'DE :> IChunkyVpTrees) =
        let mutable time = Time.GetTicksMsec()
        let planet = viewEnv.Planet
        HexSphereInitCommand.initPointsAndFaces dataEnv false planet.Divisions

        let tag = ChunkFunction.chunkyTag false

        dataEnv.EntityStore
            .Query<PointComponent>()
            .AllTags(&tag)
            .ForEachEntity(fun pComp pEntity ->
                let hexFaces, hexFaceIds, neighborCenterIds =
                    HexSphereQuery.getHexFacesAndNeighborCenterIds dataEnv false &pComp &pEntity

                HexSphereQuery.searchNearest dataEnv pComp.Position true // 找到最近的 Chunk
                |> Option.iter (fun chunk ->
                    let tileId =
                        TileCommand.add dataEnv pEntity.Id chunk.Id hexFaces hexFaceIds neighborCenterIds

                    let link = ChunkToTileId(tileId)
                    chunk.AddRelation(&link) |> ignore))

        let mutable time2 = Time.GetTicksMsec()
        GD.Print $"InitTiles cost: {time2 - time} ms"
        time <- time2

        PointCommand.createVpTree dataEnv false
        time2 <- Time.GetTicksMsec()
        GD.Print $"_tilePointVpTree Create cost: {time2 - time} ms"

    let initHexSphere
        (viewEnv: #IPlanetEnv)
        (dataEnv:
            'DE when 'DE :> IEntityStore and 'DE :> IChunkyVpTrees and 'DE :> ITileShaderData and 'DE :> ITileSearcher)
        =
        initChunks viewEnv dataEnv
        initTiles viewEnv dataEnv
        dataEnv.TileShaderData.Initialize(viewEnv.Planet)

        dataEnv.TileSearcher.InitSearchData
        <| dataEnv.EntityStore.Query<TileUnitCentroid>().Count
