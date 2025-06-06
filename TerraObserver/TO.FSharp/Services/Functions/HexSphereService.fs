namespace TO.FSharp.Services.Functions

open System
open Friflo.Engine.ECS
open Godot
open Godot.Abstractions.Extensions.Planets
open TO.FSharp.Commons.Constants.HexSpheres
open TO.FSharp.Commons.Structs.HexSphereGrid
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Models.HexSpheres.Tiles
open TO.FSharp.Repos.Types.ChunkRepoT
open TO.FSharp.Repos.Types.FaceRepoT
open TO.FSharp.Repos.Types.PointRepoT
open TO.FSharp.Repos.Types.TileRepoT
open TO.FSharp.Services.Types.HexSphereServiceT

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 12:46:30
module HexSphereService =
    // 构造北部的第一个面
    let private initNorthTriangle (point: PointRepoDep) (face: FaceRepoDep) =
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
                    point.Add chunky nowLine[0] <| SphereAxial(-divisions * col, 0) |> ignore
                else
                    point.Add chunky nowLine[i] <| SphereAxial(-divisions * col, i - divisions)
                    |> ignore

                for j in 0 .. i - 1 do
                    if j > 0 then
                        face.Add chunky nowLine[j] preLine[j] preLine[j - 1] |> ignore

                        point.Add
                            chunky
                            nowLine[j]
                            (if i = divisions then
                                 SphereAxial(-divisions * col - j, 0)
                             else
                                 SphereAxial(-divisions * col - j, i - divisions))
                        |> ignore

                    face.Add chunky preLine[j] nowLine[j] nowLine[j + 1] |> ignore

                preLine <- nowLine

    // 赤道两个面（第二、三面）的构造
    let private initEquatorTwoTriangles (point: PointRepoDep) (face: FaceRepoDep) =
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
                point.Add chunky nowLineEast[0] <| SphereAxial(-divisions * col, i) |> ignore

                for j in 0 .. i - 1 do
                    if j > 0 then
                        face.Add chunky nowLineEast[j] preLineEast[j] preLineEast[j - 1] |> ignore

                        point.Add chunky nowLineEast[j] <| SphereAxial(-divisions * col - j, i)
                        |> ignore

                    face.Add chunky preLineEast[j] nowLineEast[j] nowLineEast[j + 1] |> ignore
                // 构造西边面（第二面）
                if i < divisions then
                    point.Add chunky nowLineWest[0] <| SphereAxial(-divisions * col - i, i)
                    |> ignore

                for j in 0 .. divisions - i do
                    if j > 0 then
                        face.Add chunky preLineWest[j] nowLineWest[j - 1] nowLineWest[j] |> ignore

                        if j < divisions - i then
                            point.Add chunky nowLineWest[j] <| SphereAxial(-divisions * col - i - j, i)
                            |> ignore

                    face.Add chunky nowLineWest[j] preLineWest[j + 1] preLineWest[j] |> ignore

                preLineEast <- nowLineEast
                preLineWest <- nowLineWest

    // 构造南部的最后一面（列的第四面）
    let private initSouthTriangle (point: PointRepoDep) (face: FaceRepoDep) =
        fun chunky (edges: Vector3 array array) col divisions ->
            let nextCol = (col + 1) % 5
            let southWest = edges[nextCol * 6 + 5] // 向南方连接南极的靠西的边界
            let southEast = edges[col * 6 + 5] // 向南方连接南极的靠东的边界
            let mutable preLine = edges[col * 6 + 4] // 南回归线的边（E -> W）

            for i in 1..divisions do
                let nowLine = Math3dUtil.subdivide southEast[i] southWest[i] <| divisions - i

                if i < divisions then
                    point.Add chunky nowLine[0] <| SphereAxial(-divisions * col - i, divisions + i)
                    |> ignore

                for j in 0 .. divisions - i do
                    if j > 0 then
                        face.Add chunky preLine[j] nowLine[j - 1] nowLine[j] |> ignore

                        if j < divisions - i then
                            point.Add chunky nowLine[j]
                            <| SphereAxial(-divisions * col - i - j, divisions + i)
                            |> ignore

                    face.Add chunky nowLine[j] preLine[j + 1] preLine[j] |> ignore

                preLine <- nowLine

    // 初始化 Point 和 Face
    let private subdivideIcosahedron (point: PointRepoDep) (face: FaceRepoDep) =
        fun chunky divisions ->
            let pn = IcosahedronConstant.vertices[0] // 北极点
            let ps = IcosahedronConstant.vertices[6] // 南极点
            // 轴坐标系（0,0）放在第一组竖列四面的北回归线最东端
            point.Add chunky pn <| SphereAxial(0, -divisions) |> ignore
            point.Add chunky ps <| SphereAxial(-divisions, 2 * divisions) |> ignore
            let edges = HexSphereUtil.genEdgeVectors divisions pn ps

            for col in 0..4 do
                initNorthTriangle point face chunky edges col divisions
                initEquatorTwoTriangles point face chunky edges col divisions
                initSouthTriangle point face chunky edges col divisions

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
        fun chunky divisions ->
            let time = Time.GetTicksMsec()
            subdivideIcosahedron point face chunky divisions
            initPointFaceLinks point face chunky
            let chunkyType = if chunky then "Chunk" else "Tile" // 不能直接写在输出中
            // 三元表达式直接写在输出中，会报错：Invalid interpolated string.
            // Single quote or verbatim string literals may not be used in interpolated expressions in single quote or verbatim strings.
            // Consider using an explicit 'let' binding for the interpolation expression or use a triple quote string as the outer string literal.
            GD.Print($"--- InitPointsAndFaces for {chunkyType} cost: {Time.GetTicksMsec() - time} ms")

    let private searchNearest (point: PointRepoDep) =
        fun (pos: Vector3) chunky ->
            let center = point.SearchNearestCenterPos pos chunky

            point.TryHeadByPosition chunky center
            |> Option.bind (fun pointEntity -> point.TryHeadEntityByCenterId pointEntity.Id)

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

            searchNearest point pComp.Position true // 找到最近的 Chunk
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

    let private addFaceIndex (surfaceTool: SurfaceTool) i0 i1 i2 =
        surfaceTool.AddIndex i0
        surfaceTool.AddIndex i1
        surfaceTool.AddIndex i2

    let private generateMesh (planet: IPlanet) (tiles: (TileUnitCentroid * TileUnitCorners) seq) =
        for child in planet.GetChildren() do
            child.QueueFree()

        let meshIns = new MeshInstance3D()
        let surfaceTool = new SurfaceTool()
        surfaceTool.Begin Mesh.PrimitiveType.Triangles
        surfaceTool.SetSmoothGroup UInt32.MaxValue
        let mutable vi = 0

        for unitCentroid, unitCorners in tiles do
            surfaceTool.SetColor <| Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf())

            let points =
                unitCorners.GetCorners(unitCentroid.UnitCentroid, planet.Radius) |> Seq.toArray

            for point in points do
                surfaceTool.AddVertex point

            addFaceIndex surfaceTool vi <| vi + 1 <| vi + 2
            addFaceIndex surfaceTool vi <| vi + 2 <| vi + 3
            addFaceIndex surfaceTool vi <| vi + 3 <| vi + 4

            if points.Length > 5 then
                addFaceIndex surfaceTool vi <| vi + 4 <| vi + 5

            vi <- vi + points.Length

        surfaceTool.GenerateNormals()
        let material = new StandardMaterial3D()
        material.VertexColorUseAsAlbedo <- true
        surfaceTool.SetMaterial material
        meshIns.Mesh <- surfaceTool.Commit()
        // 貌似不能只传一个参数，尴尬……
        planet.AddChild(meshIns)


    let private clearOldData (point: PointRepoDep) (face: FaceRepoDep) (tile: TileRepoDep) (chunk: ChunkRepoDep) =
        point.Truncate()
        face.Truncate()
        tile.Truncate()
        chunk.Truncate()

    let initHexSphere
        (point: PointRepoDep)
        (face: FaceRepoDep)
        (tile: TileRepoDep)
        (chunk: ChunkRepoDep)
        : InitHexSphere =
        fun (planet: IPlanet) ->
            clearOldData point face tile chunk
            initChunks planet point face chunk
            initTiles planet point face tile chunk
            generateMesh planet <| tile.CentroidAndCornersSeq()

    let getDependency
        (point: PointRepoDep)
        (face: FaceRepoDep)
        (tile: TileRepoDep)
        (chunk: ChunkRepoDep)
        : HexSphereServiceDep =
        { InitHexSphere = initHexSphere point face tile chunk }
