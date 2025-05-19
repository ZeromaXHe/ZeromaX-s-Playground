namespace TO.Infras.Planets

open Friflo.Engine.ECS
open Godot
open TO.Domains.Planets.Functions
open TO.Infras.Planets.Models.Faces
open TO.Infras.Planets.Models.Points
open TO.Infras.Planets.Repos
open TO.Domains.Planets.Types.FacePoint
open TO.Nodes.Abstractions.Planets.Models

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-15 14:33:15
type PlanetWorld() =
    let store = EntityStore()
    let pointRepo = PointRepo(store)
    let faceRepo = FaceRepo(store)
    let chunkRepo = ChunkRepo(store)

    let initPointFaceLinks chunky =
        let query = faceRepo.QueryFaces chunky

        query.ForEachEntity(fun faceComp faceEntity ->
            faceComp.TriVertices
            |> Array.iter (fun v ->
                // 给每个点建立它与所归属的面的关系
                pointRepo.QueryPointsByPosition chunky v
                |> Option.iter (fun pointEntity ->
                    let link = PointLinkFace(faceEntity)
                    pointEntity.AddRelation(&link) |> ignore)))

    let initPointsAndFaces chunky chunkDivisions =
        let time = Time.GetTicksMsec()
        let faceAdders, pointAdders = FacePointFunc.subdivideIcosahedron chunkDivisions

        faceAdders
        |> List.iter (fun a -> faceRepo.AddFace chunky a.TriVertices |> ignore)

        pointAdders
        |> List.iter (fun a -> pointRepo.AddPoint chunky a.Position a.Coords |> ignore)

        initPointFaceLinks chunky
        let chunkyType = if chunky then "Chunk" else "Tile" // 不能直接写在输出中
        // 三元表达式直接写在输出中，会报错：Invalid interpolated string.
        // Single quote or verbatim string literals may not be used in interpolated expressions in single quote or verbatim strings.
        // Consider using an explicit 'let' binding for the interpolation expression or use a triple quote string as the outer string literal.
        GD.Print($"--- InitPointsAndFaces for {chunkyType} cost: {Time.GetTicksMsec() - time} ms")

    member this.InitChunks(hexSphereConfigs: IHexSphereConfigs) =
        let time = Time.GetTicksMsec()
        initPointsAndFaces true hexSphereConfigs.ChunkDivisions
        let chunkyPoints = pointRepo.QueryPoints true

        chunkyPoints.ForEachEntity(fun pComp pEntity ->
            let hexFaceEntities = faceRepo.GetOrderedFaces pComp pEntity
            let hexFaces = hexFaceEntities |> List.map _.GetComponent<FaceComponent>()

            let neighborCenterIds =
                pointRepo.GetNeighborCenterPoints(true, hexFaces, &pComp)
                |> List.map _.Id
                |> List.toArray

            chunkRepo.AddChunk(
                pEntity.Id,
                pComp.Position * (hexSphereConfigs.Radius + hexSphereConfigs.MaxHeight),
                neighborCenterIds
            )
            |> ignore

            ())

        chunkRepo.CreateVpTree(
            (pointRepo.QueryPoints true).ToEntityList()
            |> Seq.map _.GetComponent<PointComponent>().Position
            |> Seq.toArray,
            fun p0 p1 -> p0.DistanceTo p1
        )

        GD.Print($"InitChunks chunkDivisions {hexSphereConfigs.ChunkDivisions}, cost: {Time.GetTicksMsec() - time} ms")

    member this.ClearOldData() =
        store.Entities |> Seq.iter _.DeleteEntity()
