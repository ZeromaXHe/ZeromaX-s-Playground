namespace TO.FSharp.Services.Functions

open Friflo.Engine.ECS
open Godot
open TO.Domains.Planets.Functions
open TO.FSharp.GodotAbstractions.Extensions.Planets
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Types.ChunkRepoT
open TO.FSharp.Repos.Types.FaceRepoT
open TO.FSharp.Repos.Types.PointRepoT
open TO.FSharp.Repos.Types.TileRepoT
open TO.FSharp.Services.Types.HexSphereServiceT

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 12:46:30
module HexSphereService =
    let private initPointFaceLinks (point: PointRepoDep) (face: FaceRepoDep) =
        fun chunky ->
            face.ForEachByChunky chunky
            <| ForEachEntity<FaceComponent>(fun faceComp faceEntity ->
                let relatePointToFace v =
                    // 给每个点建立它与所归属的面的关系
                    point.TryHeadByPosition chunky v
                    |> Option.iter (fun pointEntity ->
                        let relation = PointToFaceId(faceEntity.Id)
                        pointEntity.AddRelation(&relation) |> ignore)

                relatePointToFace faceComp.Vertex1
                relatePointToFace faceComp.Vertex2
                relatePointToFace faceComp.Vertex3)

    let private initPointsAndFaces (point: PointRepoDep) (face: FaceRepoDep) =
        fun chunky chunkDivisions ->
            let time = Time.GetTicksMsec()

            let pointAdder pos coords = point.Add chunky pos coords |> ignore

            let faceAdder v1 v2 v3 = face.Add chunky v1 v2 v3 |> ignore

            FacePointDomain.subdivideIcosahedron pointAdder faceAdder chunkDivisions

            initPointFaceLinks point face chunky
            let chunkyType = if chunky then "Chunk" else "Tile" // 不能直接写在输出中
            // 三元表达式直接写在输出中，会报错：Invalid interpolated string.
            // Single quote or verbatim string literals may not be used in interpolated expressions in single quote or verbatim strings.
            // Consider using an explicit 'let' binding for the interpolation expression or use a triple quote string as the outer string literal.
            GD.Print($"--- InitPointsAndFaces for {chunkyType} cost: {Time.GetTicksMsec() - time} ms")

    let private searchNearest (point: PointRepoDep) (tile: TileRepoDep) (chunk: ChunkRepoDep) =
        fun (pos: Vector3) chunky ->
            let center = point.SearchNearestCenterPos pos chunky

            point.TryHeadByPosition chunky center
            |> Option.bind (fun pointEntity ->
                if chunky then
                    chunk.TryHeadByCenterId pointEntity.Id
                else
                    tile.TryHeadByCenterId pointEntity.Id)

    let private getHexFacesAndNeighborCenterIds (point: PointRepoDep) (face: FaceRepoDep) =
        fun chunky (pComp: PointComponent inref) (pEntity: Entity inref) ->
            let hexFaceEntities = face.GetOrderedFaces pComp pEntity
            let hexFaces = hexFaceEntities |> List.map _.GetComponent<FaceComponent>()

            let neighborCenterIds =
                point.GetNeighborCenterPointIds.Invoke(chunky, hexFaces, &pComp) |> Seq.toArray

            (hexFaces |> List.toArray, hexFaceEntities |> List.map _.Id |> List.toArray, neighborCenterIds)

    let private initChunks (planet: IPlanet) (point: PointRepoDep) (face: FaceRepoDep) (chunk: ChunkRepoDep) =
        let time = Time.GetTicksMsec()
        initPointsAndFaces point face true planet.ChunkDivisions

        point.ForEachByChunky true
        <| ForEachEntity<PointComponent>(fun pComp pEntity ->
            let _, _, neighborCenterIds =
                getHexFacesAndNeighborCenterIds point face true &pComp &pEntity

            chunk.Add pEntity.Id
            <| pComp.Position * (planet.Radius + planet.MaxHeight)
            <| neighborCenterIds
            |> ignore)

        point.CreateVpTree true

        GD.Print($"InitChunks chunkDivisions {planet.ChunkDivisions}, cost: {Time.GetTicksMsec() - time} ms")

    let private initTiles
        (planet: IPlanet)
        (point: PointRepoDep)
        (face: FaceRepoDep)
        (tile: TileRepoDep)
        (chunk: ChunkRepoDep)
        =
        let mutable time = Time.GetTicksMsec()
        initPointsAndFaces point face false planet.Divisions

        point.ForEachByChunky false
        <| ForEachEntity<PointComponent>(fun pComp pEntity ->
            let hexFaces, hexFaceIds, neighborCenterIds =
                getHexFacesAndNeighborCenterIds point face false &pComp &pEntity

            searchNearest point tile chunk pComp.Position true // 找到最近的 Chunk
            |> Option.iter (fun chunk ->
                let tileId = tile.Add pEntity.Id chunk.Id hexFaces hexFaceIds neighborCenterIds

                let link = ChunkToTileId(tileId)
                chunk.AddRelation(&link) |> ignore))

        let mutable time2 = Time.GetTicksMsec()
        GD.Print $"InitTiles cost: {time2 - time} ms"
        time <- time2

        point.CreateVpTree false
        time2 <- Time.GetTicksMsec()
        GD.Print $"_tilePointVpTree Create cost: {time2 - time} ms"

    let initHexSphere
        (point: PointRepoDep)
        (face: FaceRepoDep)
        (tile: TileRepoDep)
        (chunk: ChunkRepoDep)
        : InitHexSphere =
        fun (planet: IPlanet) ->
            initChunks planet point face chunk
            initTiles planet point face tile chunk
            tile.AllSeq()

    let clearOldData
        (point: PointRepoDep)
        (face: FaceRepoDep)
        (tile: TileRepoDep)
        (chunk: ChunkRepoDep)
        : ClearOldData =
        fun () ->
            point.Truncate()
            face.Truncate()
            tile.Truncate()
            chunk.Truncate()

    let getDependency
        (point: PointRepoDep)
        (face: FaceRepoDep)
        (tile: TileRepoDep)
        (chunk: ChunkRepoDep)
        : HexSphereServiceDep =
        { InitHexSphere = initHexSphere point face tile chunk
          ClearOldData = clearOldData point face tile chunk }
