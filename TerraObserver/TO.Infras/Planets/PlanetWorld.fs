namespace TO.Infras.Planets

open Friflo.Engine.ECS
open Godot
open TO.Domains.Planets.Functions
open TO.Infras.Planets.Models.Chunks
open TO.Infras.Planets.Models.Faces
open TO.Infras.Planets.Models.Points
open TO.Infras.Planets.Models.Tiles
open TO.Infras.Planets.Repos
open TO.Nodes.Abstractions.Planets.Models

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-15 14:33:15
type PlanetWorld() =
    let store = EntityStore()
    let pointRepo = PointRepo(store)
    let faceRepo = FaceRepo(store)
    let tileRepo = TileRepo(store)
    let chunkRepo = ChunkRepo(store)

    let initPointFaceLinks chunky =
        let query = faceRepo.QueryAll chunky

        query.ForEachEntity(fun faceComp faceEntity ->
            let relatePointToFace v =
                // 给每个点建立它与所归属的面的关系
                pointRepo.QueryByPosition chunky v
                |> Option.iter (fun pointEntity ->
                    let relation = PointToFaceId(faceEntity)
                    pointEntity.AddRelation(&relation) |> ignore)

            relatePointToFace faceComp.Vertex1
            relatePointToFace faceComp.Vertex2
            relatePointToFace faceComp.Vertex3)

    let initPointsAndFaces chunky chunkDivisions =
        let time = Time.GetTicksMsec()

        let pointAdder pos coords =
            pointRepo.Add chunky pos coords |> ignore

        let faceAdder v1 v2 v3 = faceRepo.Add chunky v1 v2 v3 |> ignore
        FacePointFunc.subdivideIcosahedron pointAdder faceAdder chunkDivisions

        initPointFaceLinks chunky
        let chunkyType = if chunky then "Chunk" else "Tile" // 不能直接写在输出中
        // 三元表达式直接写在输出中，会报错：Invalid interpolated string.
        // Single quote or verbatim string literals may not be used in interpolated expressions in single quote or verbatim strings.
        // Consider using an explicit 'let' binding for the interpolation expression or use a triple quote string as the outer string literal.
        GD.Print($"--- InitPointsAndFaces for {chunkyType} cost: {Time.GetTicksMsec() - time} ms")

    let searchNearest (pos: Vector3) chunky =
        let center = pointRepo.SearchNearestCenterPos(pos, chunky)
        let pointEntityOpt = pointRepo.QueryByPosition chunky center

        pointEntityOpt
        |> Option.bind (fun pointEntity ->
            if chunky then
                chunkRepo.QueryByCenterId pointEntity.Id
            else
                tileRepo.QueryByCenterId pointEntity.Id)

    let searchNearestId (pos: Vector3) chunky =
        searchNearest pos chunky |> Option.map _.Id

    let getHexFacesAndNeighborCenterIds (chunky, pComp: PointComponent inref, pEntity: Entity inref) =
        let hexFaceEntities = faceRepo.GetOrderedFaces pComp pEntity
        let hexFaces = hexFaceEntities |> List.map _.GetComponent<FaceComponent>()

        let neighborCenterIds =
            pointRepo.GetNeighborCenterPoints(chunky, hexFaces, &pComp)
            |> Seq.map _.Id
            |> Seq.toArray

        (hexFaces |> List.toArray, hexFaceEntities |> List.map _.Id |> List.toArray, neighborCenterIds)

    let initChunks (hexSphereConfigs: IHexSphereConfigs) =
        let time = Time.GetTicksMsec()
        initPointsAndFaces true hexSphereConfigs.ChunkDivisions
        let chunkyPoints = pointRepo.QueryAllByChunky true

        chunkyPoints.ForEachEntity(fun pComp pEntity ->
            let _, _, neighborCenterIds =
                getHexFacesAndNeighborCenterIds (true, &pComp, &pEntity)

            chunkRepo.Add(
                pEntity.Id,
                pComp.Position * (hexSphereConfigs.Radius + hexSphereConfigs.MaxHeight),
                neighborCenterIds
            )
            |> ignore)

        pointRepo.CreateVpTree true

        GD.Print($"InitChunks chunkDivisions {hexSphereConfigs.ChunkDivisions}, cost: {Time.GetTicksMsec() - time} ms")

    let initTiles (hexSphereConfigs: IHexSphereConfigs) =
        let mutable time = Time.GetTicksMsec()
        initPointsAndFaces false hexSphereConfigs.Divisions
        let tilePoints = pointRepo.QueryAllByChunky false

        tilePoints.ForEachEntity(fun pComp pEntity ->
            let hexFaces, hexFaceIds, neighborCenterIds =
                getHexFacesAndNeighborCenterIds (false, &pComp, &pEntity)

            searchNearest pComp.Position true
            |> Option.iter (fun chunk ->
                let tileId =
                    tileRepo.Add(pEntity.Id, chunk.Id, hexFaces, hexFaceIds, neighborCenterIds)

                let link = ChunkToTileId(store.GetEntityById tileId)
                chunk.AddRelation(&link) |> ignore))

        let mutable time2 = Time.GetTicksMsec()
        GD.Print $"InitTiles cost: {time2 - time} ms"
        time <- time2

        pointRepo.CreateVpTree false
        time2 <- Time.GetTicksMsec()
        GD.Print $"_tilePointVpTree Create cost: {time2 - time} ms"

    member this.InitHexSphere(hexSphereConfigs: IHexSphereConfigs) =
        initChunks hexSphereConfigs
        initTiles hexSphereConfigs

        seq {
            for chunks in store.Query<TileComponent>().Chunks do
                let chunk, _ = chunks.Deconstruct()

                for i in 0 .. chunk.Length - 1 do
                    chunk.Span[i]
        }

    member this.ClearOldData() =
        store.Entities |> Seq.iter _.DeleteEntity()
