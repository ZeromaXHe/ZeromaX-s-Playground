namespace TO.FSharp.Services.Functions

open Friflo.Engine.ECS
open Godot
open Godot.Abstractions.Extensions.Planets
open TO.FSharp.Commons.Constants.HexSpheres
open TO.FSharp.Commons.DataStructures
open TO.FSharp.Commons.Structs.HexSphereGrid
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Functions.HexSpheres
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.PathFindings
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Models.Shaders
open TO.FSharp.Repos.Models.HexSpheres.Tiles
open TO.FSharp.Services.Types.HexSphereServiceT

module private HexSphereInitializer =
    // 构造北部的第一个面
    let private initNorthTriangle (store: EntityStore) =
        fun chunky (edges: Vector3 array array) col divisions ->
            let nextCol = (col + 1) % 5
            let northEast = edges[col * 6] // 北极出来的靠东的边界
            let northWest = edges[nextCol * 6] // 北极出来的靠西的边界
            let tropicOfCancer = edges[col * 6 + 1] // 北回归线的边（E -> W）
            let mutable preLine = [| northEast[0] |] // 初始为北极点

            for i in 1..divisions do
                let nowLine =
                    if i = divisions then
                        tropicOfCancer
                    else
                        Math3dUtil.subdivide northEast[i] northWest[i] i

                if i = divisions then
                    PointRepo.add store chunky nowLine[0] <| SphereAxial(-divisions * col, 0)
                    |> ignore
                else
                    PointRepo.add store chunky nowLine[i]
                    <| SphereAxial(-divisions * col, i - divisions)
                    |> ignore

                for j in 0 .. i - 1 do
                    if j > 0 then
                        FaceRepo.add store chunky nowLine[j] preLine[j] preLine[j - 1] |> ignore

                        PointRepo.add
                            store
                            chunky
                            nowLine[j]
                            (if i = divisions then
                                 SphereAxial(-divisions * col - j, 0)
                             else
                                 SphereAxial(-divisions * col - j, i - divisions))
                        |> ignore

                    FaceRepo.add store chunky preLine[j] nowLine[j] nowLine[j + 1] |> ignore

                preLine <- nowLine

    // 赤道两个面（第二、三面）的构造
    let private initEquatorTwoTriangles (store: EntityStore) =
        fun chunky (edges: Vector3 array array) col divisions ->
            let nextCol = (col + 1) % 5
            let equatorWest = edges[nextCol * 6 + 3] // 向东南方斜跨赤道的靠西的边界
            let equatorMid = edges[col * 6 + 2] // 向西南方斜跨赤道的中间的边
            let equatorEast = edges[col * 6 + 3] // 向东南方斜跨赤道的靠东的边界
            let tropicOfCapricorn = edges[col * 6 + 4] // 南回归线的边（E -> W）
            let mutable preLineWest = edges[col * 6 + 1] // 北回归线的边（E -> W）
            let mutable preLineEast = [| equatorEast[0] |]

            for i in 1..divisions do
                let nowLineEast =
                    if i = divisions then
                        tropicOfCapricorn
                    else
                        Math3dUtil.subdivide equatorEast[i] equatorMid[i] i

                let nowLineWest = Math3dUtil.subdivide equatorMid[i] equatorWest[i] <| divisions - i
                // 构造东边面（第三面）
                PointRepo.add store chunky nowLineEast[0] <| SphereAxial(-divisions * col, i)
                |> ignore

                for j in 0 .. i - 1 do
                    if j > 0 then
                        FaceRepo.add store chunky nowLineEast[j] preLineEast[j] preLineEast[j - 1]
                        |> ignore

                        PointRepo.add store chunky nowLineEast[j]
                        <| SphereAxial(-divisions * col - j, i)
                        |> ignore

                    FaceRepo.add store chunky preLineEast[j] nowLineEast[j] nowLineEast[j + 1]
                    |> ignore
                // 构造西边面（第二面）
                if i < divisions then
                    PointRepo.add store chunky nowLineWest[0]
                    <| SphereAxial(-divisions * col - i, i)
                    |> ignore

                for j in 0 .. divisions - i do
                    if j > 0 then
                        FaceRepo.add store chunky preLineWest[j] nowLineWest[j - 1] nowLineWest[j]
                        |> ignore

                        if j < divisions - i then
                            PointRepo.add store chunky nowLineWest[j]
                            <| SphereAxial(-divisions * col - i - j, i)
                            |> ignore

                    FaceRepo.add store chunky nowLineWest[j] preLineWest[j + 1] preLineWest[j]
                    |> ignore

                preLineEast <- nowLineEast
                preLineWest <- nowLineWest

    // 构造南部的最后一面（列的第四面）
    let private initSouthTriangle (store: EntityStore) =
        fun chunky (edges: Vector3 array array) col divisions ->
            let nextCol = (col + 1) % 5
            let southWest = edges[nextCol * 6 + 5] // 向南方连接南极的靠西的边界
            let southEast = edges[col * 6 + 5] // 向南方连接南极的靠东的边界
            let mutable preLine = edges[col * 6 + 4] // 南回归线的边（E -> W）

            for i in 1..divisions do
                let nowLine = Math3dUtil.subdivide southEast[i] southWest[i] <| divisions - i

                if i < divisions then
                    PointRepo.add store chunky nowLine[0]
                    <| SphereAxial(-divisions * col - i, divisions + i)
                    |> ignore

                for j in 0 .. divisions - i do
                    if j > 0 then
                        FaceRepo.add store chunky preLine[j] nowLine[j - 1] nowLine[j] |> ignore

                        if j < divisions - i then
                            PointRepo.add store chunky nowLine[j]
                            <| SphereAxial(-divisions * col - i - j, divisions + i)
                            |> ignore

                    FaceRepo.add store chunky nowLine[j] preLine[j + 1] preLine[j] |> ignore

                preLine <- nowLine

    // 初始化 Point 和 Face
    let private subdivideIcosahedron store chunky divisions =
        let pn = IcosahedronConstant.vertices[0] // 北极点
        let ps = IcosahedronConstant.vertices[6] // 南极点
        // 轴坐标系（0,0）放在第一组竖列四面的北回归线最东端
        PointRepo.add store chunky pn <| SphereAxial(0, -divisions) |> ignore

        PointRepo.add store chunky ps <| SphereAxial(-divisions, 2 * divisions)
        |> ignore

        let edges = HexSphereUtil.genEdgeVectors divisions pn ps

        for col in 0..4 do
            initNorthTriangle store chunky edges col divisions
            initEquatorTwoTriangles store chunky edges col divisions
            initSouthTriangle store chunky edges col divisions

    let private initPointFaceLinks (store: EntityStore) =
        fun chunky ->
            let tag = PointRepo.chunkyTag chunky

            store
                .Query<FaceComponent>()
                .AllTags(&tag)
                .ForEachEntity(fun faceComp faceEntity ->
                    let relatePointToFace v =
                        // 给每个点建立它与所归属的面的关系
                        PointRepo.tryHeadByPosition store chunky v
                        |> Option.iter (fun pointEntity ->
                            let relation = PointToFaceId(faceEntity.Id)
                            pointEntity.AddRelation(&relation) |> ignore)

                    relatePointToFace faceComp.Vertex1
                    relatePointToFace faceComp.Vertex2
                    relatePointToFace faceComp.Vertex3)

    let private initPointsAndFaces (store: EntityStore) =
        fun chunky divisions ->
            let time = Time.GetTicksMsec()
            subdivideIcosahedron store chunky divisions
            initPointFaceLinks store chunky
            let chunkyType = if chunky then "Chunk" else "Tile" // 不能直接写在输出中
            // 三元表达式直接写在输出中，会报错：Invalid interpolated string.
            // Single quote or verbatim string literals may not be used in interpolated expressions in single quote or verbatim strings.
            // Consider using an explicit 'let' binding for the interpolation expression or use a triple quote string as the outer string literal.
            GD.Print($"--- InitPointsAndFaces for {chunkyType} cost: {Time.GetTicksMsec() - time} ms")

    let searchNearest (store: EntityStore) (chunkVpTree: Vector3 VpTree) (tileVpTree: Vector3 VpTree) =
        fun (pos: Vector3) chunky ->
            let center = PointRepo.searchNearestCenterPos chunkVpTree tileVpTree pos chunky

            PointRepo.tryHeadByPosition store chunky center
            |> Option.bind (fun pointEntity -> PointRepo.tryHeadEntityByCenterId store pointEntity.Id)

    let getHexFacesAndNeighborCenterIds (store: EntityStore) =
        fun chunky (pComp: PointComponent inref) (pEntity: Entity inref) ->
            let hexFaceEntities = FaceRepo.getOrderedFaces store pComp pEntity
            let hexFaces = hexFaceEntities |> List.map _.GetComponent<FaceComponent>()

            let neighborCenterIds =
                PointRepo.getNeighborCenterPointIds store chunky hexFaces &pComp |> Seq.toArray

            (hexFaces |> List.toArray, hexFaceEntities |> List.map _.Id |> List.toArray, neighborCenterIds)

    let initChunks (planet: IPlanet) (store: EntityStore) (chunkVpTree: Vector3 VpTree) (tileVpTree: Vector3 VpTree) =
        let time = Time.GetTicksMsec()
        initPointsAndFaces store true planet.ChunkDivisions

        let tag = PointRepo.chunkyTag true

        store
            .Query<PointComponent>()
            .AllTags(&tag)
            .ForEachEntity(fun pComp pEntity ->
                let _, _, neighborCenterIds =
                    getHexFacesAndNeighborCenterIds store true &pComp &pEntity

                ChunkRepo.add store pEntity.Id
                <| pComp.Position * (planet.Radius + planet.MaxHeight)
                <| neighborCenterIds
                |> ignore)

        PointRepo.createVpTree store chunkVpTree tileVpTree true

        GD.Print($"InitChunks chunkDivisions {planet.ChunkDivisions}, cost: {Time.GetTicksMsec() - time} ms")

    let initTiles (planet: IPlanet) (store: EntityStore) (chunkVpTree: Vector3 VpTree) (tileVpTree: Vector3 VpTree) =
        let mutable time = Time.GetTicksMsec()
        initPointsAndFaces store false planet.Divisions

        let tag = PointRepo.chunkyTag false

        store
            .Query<PointComponent>()
            .AllTags(&tag)
            .ForEachEntity(fun pComp pEntity ->
                let hexFaces, hexFaceIds, neighborCenterIds =
                    getHexFacesAndNeighborCenterIds store false &pComp &pEntity

                searchNearest store chunkVpTree tileVpTree pComp.Position true // 找到最近的 Chunk
                |> Option.iter (fun chunk ->
                    let tileId =
                        TileRepo.add store pEntity.Id chunk.Id hexFaces hexFaceIds neighborCenterIds

                    let link = ChunkToTileId(tileId)
                    chunk.AddRelation(&link) |> ignore))

        let mutable time2 = Time.GetTicksMsec()
        GD.Print $"InitTiles cost: {time2 - time} ms"
        time <- time2

        PointRepo.createVpTree store chunkVpTree tileVpTree false
        time2 <- Time.GetTicksMsec()
        GD.Print $"_tilePointVpTree Create cost: {time2 - time} ms"

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 12:46:30
module HexSphereService =
    let clearOldData (store: EntityStore) : ClearOldData =
        fun () ->
            let cb = store.GetCommandBuffer()
            store.Query<PointComponent>().ForEachEntity(fun _ e -> cb.DeleteEntity(e.Id))
            store.Query<FaceComponent>().ForEachEntity(fun _ e -> cb.DeleteEntity(e.Id))
            store.Query<TileUnitCentroid>().ForEachEntity(fun _ e -> cb.DeleteEntity(e.Id))
            store.Query<ChunkPos>().ForEachEntity(fun _ e -> cb.DeleteEntity(e.Id))
            FrifloEcsUtil.commitCommands store

    let initHexSphere
        (planet: IPlanet)
        (tileShaderData: TileShaderData)
        (tileSearcher: TileSearcher)
        (store: EntityStore)
        (chunkVpTree: Vector3 VpTree)
        (tileVpTree: Vector3 VpTree)
        : InitHexSphere =
        fun () ->
            HexSphereInitializer.initChunks planet store chunkVpTree tileVpTree
            HexSphereInitializer.initTiles planet store chunkVpTree tileVpTree
            tileShaderData.Initialize(planet)
            tileSearcher.InitSearchData <| store.Query<TileUnitCentroid>().Count

    let getDependency
        (planet: IPlanet)
        (tileShaderData: TileShaderData)
        (tileSearcher: TileSearcher)
        (store: EntityStore)
        (chunkVpTree: Vector3 VpTree)
        (tileVpTree: Vector3 VpTree)
        : HexSphereServiceDep =
        { InitHexSphere = initHexSphere planet tileShaderData tileSearcher store chunkVpTree tileVpTree
          ClearOldData = clearOldData store }
