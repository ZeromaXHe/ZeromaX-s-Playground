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

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 12:46:30
module HexSphereService =
    let private initPointFaceLinks
        (forEachFaceByChunky: ForEachFaceByChunky)
        (tryHeadPointByPosition: TryHeadPointByPosition)
        =
        fun chunky ->
            forEachFaceByChunky chunky
            <| ForEachEntity<FaceComponent>(fun faceComp faceEntity ->
                let relatePointToFace v =
                    // 给每个点建立它与所归属的面的关系
                    tryHeadPointByPosition chunky v
                    |> Option.iter (fun pointEntity ->
                        let relation = PointToFaceId(faceEntity.Id)
                        pointEntity.AddRelation(&relation) |> ignore)

                relatePointToFace faceComp.Vertex1
                relatePointToFace faceComp.Vertex2
                relatePointToFace faceComp.Vertex3)

    let private initPointsAndFaces
        (forEachFaceByChunky: ForEachFaceByChunky)
        (tryHeadPointByPosition: TryHeadPointByPosition)
        (addPoint: AddPoint)
        (addFace: AddFace)
        =
        fun chunky chunkDivisions ->
            let time = Time.GetTicksMsec()

            let pointAdder pos coords = addPoint chunky pos coords |> ignore

            let faceAdder v1 v2 v3 = addFace chunky v1 v2 v3 |> ignore
            FacePointDomain.subdivideIcosahedron pointAdder faceAdder chunkDivisions

            initPointFaceLinks forEachFaceByChunky tryHeadPointByPosition chunky
            let chunkyType = if chunky then "Chunk" else "Tile" // 不能直接写在输出中
            // 三元表达式直接写在输出中，会报错：Invalid interpolated string.
            // Single quote or verbatim string literals may not be used in interpolated expressions in single quote or verbatim strings.
            // Consider using an explicit 'let' binding for the interpolation expression or use a triple quote string as the outer string literal.
            GD.Print($"--- InitPointsAndFaces for {chunkyType} cost: {Time.GetTicksMsec() - time} ms")

    let private searchNearest
        (searchNearestCenterPos: SearchNearestCenterPos)
        (tryHeadPointByPosition: TryHeadPointByPosition)
        (tryHeadChunkByCenterId: TryHeadChunkByCenterId)
        (tryHeadTileByCenterId: TryHeadTileByCenterId)
        =
        fun (pos: Vector3) chunky ->
            let center = searchNearestCenterPos pos chunky

            tryHeadPointByPosition chunky center
            |> Option.bind (fun pointEntity ->
                if chunky then
                    tryHeadChunkByCenterId pointEntity.Id
                else
                    tryHeadTileByCenterId pointEntity.Id)

    let private getHexFacesAndNeighborCenterIds
        (getOrderedFaces: GetOrderedFaces)
        (getNeighborCenterPointIds: GetNeighborCenterPointIds)
        =
        fun chunky (pComp: PointComponent inref) (pEntity: Entity inref) ->
            let hexFaceEntities = getOrderedFaces pComp pEntity
            let hexFaces = hexFaceEntities |> List.map _.GetComponent<FaceComponent>()

            let neighborCenterIds =
                getNeighborCenterPointIds.Invoke(chunky, hexFaces, &pComp) |> Seq.toArray

            (hexFaces |> List.toArray, hexFaceEntities |> List.map _.Id |> List.toArray, neighborCenterIds)

    let private initChunks
        (planet: IPlanet)
        (forEachFaceByChunky: ForEachFaceByChunky)
        (forEachPointByChunky: ForEachPointByChunky)
        (tryHeadPointByPosition: TryHeadPointByPosition)
        (getOrderedFaces: GetOrderedFaces)
        (getNeighborCenterPointIds: GetNeighborCenterPointIds)
        (createVpTree: CreateVpTree)
        (addPoint: AddPoint)
        (addFace: AddFace)
        (addChunk: AddChunk)
        =
        let time = Time.GetTicksMsec()
        initPointsAndFaces forEachFaceByChunky tryHeadPointByPosition addPoint addFace true planet.ChunkDivisions

        forEachPointByChunky true
        <| ForEachEntity<PointComponent>(fun pComp pEntity ->
            let _, _, neighborCenterIds =
                getHexFacesAndNeighborCenterIds getOrderedFaces getNeighborCenterPointIds true &pComp &pEntity

            addChunk pEntity.Id
            <| pComp.Position * (planet.Radius + planet.MaxHeight)
            <| neighborCenterIds
            |> ignore)

        createVpTree true

        GD.Print($"InitChunks chunkDivisions {planet.ChunkDivisions}, cost: {Time.GetTicksMsec() - time} ms")

    let private initTiles
        (planet: IPlanet)
        (forEachFaceByChunky: ForEachFaceByChunky)
        (forEachPointByChunky: ForEachPointByChunky)
        (searchNearestCenterPos: SearchNearestCenterPos)
        (tryHeadPointByPosition: TryHeadPointByPosition)
        (tryHeadChunkByCenterId: TryHeadChunkByCenterId)
        (tryHeadTileByCenterId: TryHeadTileByCenterId)
        (getOrderedFaces: GetOrderedFaces)
        (getNeighborCenterPointIds: GetNeighborCenterPointIds)
        (createVpTree: CreateVpTree)
        (addPoint: AddPoint)
        (addFace: AddFace)
        (addTile: AddTile)
        =
        let mutable time = Time.GetTicksMsec()
        initPointsAndFaces forEachFaceByChunky tryHeadPointByPosition addPoint addFace false planet.Divisions

        forEachPointByChunky false
        <| ForEachEntity<PointComponent>(fun pComp pEntity ->
            let hexFaces, hexFaceIds, neighborCenterIds =
                getHexFacesAndNeighborCenterIds getOrderedFaces getNeighborCenterPointIds false &pComp &pEntity

            searchNearest
                searchNearestCenterPos
                tryHeadPointByPosition
                tryHeadChunkByCenterId
                tryHeadTileByCenterId
                pComp.Position
                true // 找到最近的 Chunk
            |> Option.iter (fun chunk ->
                let tileId = addTile pEntity.Id chunk.Id hexFaces hexFaceIds neighborCenterIds
                let link = ChunkToTileId(tileId)
                chunk.AddRelation(&link) |> ignore))

        let mutable time2 = Time.GetTicksMsec()
        GD.Print $"InitTiles cost: {time2 - time} ms"
        time <- time2

        createVpTree false
        time2 <- Time.GetTicksMsec()
        GD.Print $"_tilePointVpTree Create cost: {time2 - time} ms"

    let initHexSphere
        (planet: IPlanet)
        (forEachFaceByChunky: ForEachFaceByChunky)
        (forEachPointByChunky: ForEachPointByChunky)
        (searchNearestCenterPos: SearchNearestCenterPos)
        (tryHeadPointByPosition: TryHeadPointByPosition)
        (tryHeadChunkByCenterId: TryHeadChunkByCenterId)
        (tryHeadTileByCenterId: TryHeadTileByCenterId)
        (getOrderedFaces: GetOrderedFaces)
        (getNeighborCenterPointIds: GetNeighborCenterPointIds)
        (createVpTree: CreateVpTree)
        (addPoint: AddPoint)
        (addFace: AddFace)
        (addTile: AddTile)
        (addChunk: AddChunk)
        (allTilesSeq: AllTilesSeq)
        =
        initChunks
            planet
            forEachFaceByChunky
            forEachPointByChunky
            tryHeadPointByPosition
            getOrderedFaces
            getNeighborCenterPointIds
            createVpTree
            addPoint
            addFace
            addChunk

        initTiles
            planet
            forEachFaceByChunky
            forEachPointByChunky
            searchNearestCenterPos
            tryHeadPointByPosition
            tryHeadChunkByCenterId
            tryHeadTileByCenterId
            getOrderedFaces
            getNeighborCenterPointIds
            createVpTree
            addPoint
            addFace
            addTile

        allTilesSeq ()

    let clearOldData
        (truncatePoints: TruncatePoints)
        (truncateFaces: TruncateFaces)
        (truncateTiles: TruncateTiles)
        (truncateChunks: TruncateChunks)
        =
        fun () ->
            truncatePoints ()
            truncateFaces ()
            truncateTiles ()
            truncateChunks ()
