namespace TO.FSharp.Services.Functions

open System
open Friflo.Engine.ECS
open Godot
open Godot.Abstractions.Extensions.Chunks
open Godot.Abstractions.Extensions.Planets
open TO.FSharp.Commons.Constants.HexSpheres
open TO.FSharp.Commons.Structs.HexSphereGrid
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.PathFindings
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Models.Shaders
open TO.FSharp.Repos.Models.HexSpheres.Tiles
open TO.FSharp.Repos.Types.HexSpheres.ChunkRepoT
open TO.FSharp.Repos.Types.HexSpheres.FaceRepoT
open TO.FSharp.Repos.Types.HexSpheres.PointRepoT
open TO.FSharp.Repos.Types.HexSpheres.TileRepoT
open TO.FSharp.Services.Types.HexSphereServiceT

module private HexSphereInitializer =
    // 构造北部的第一个面
    let private initNorthTriangle (addPoint: AddPoint) (addFace: AddFace) =
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
                    addPoint chunky nowLine[0] <| SphereAxial(-divisions * col, 0) |> ignore
                else
                    addPoint chunky nowLine[i] <| SphereAxial(-divisions * col, i - divisions)
                    |> ignore

                for j in 0 .. i - 1 do
                    if j > 0 then
                        addFace chunky nowLine[j] preLine[j] preLine[j - 1] |> ignore

                        addPoint
                            chunky
                            nowLine[j]
                            (if i = divisions then
                                 SphereAxial(-divisions * col - j, 0)
                             else
                                 SphereAxial(-divisions * col - j, i - divisions))
                        |> ignore

                    addFace chunky preLine[j] nowLine[j] nowLine[j + 1] |> ignore

                preLine <- nowLine

    // 赤道两个面（第二、三面）的构造
    let private initEquatorTwoTriangles (addPoint: AddPoint) (addFace: AddFace) =
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
                addPoint chunky nowLineEast[0] <| SphereAxial(-divisions * col, i) |> ignore

                for j in 0 .. i - 1 do
                    if j > 0 then
                        addFace chunky nowLineEast[j] preLineEast[j] preLineEast[j - 1] |> ignore

                        addPoint chunky nowLineEast[j] <| SphereAxial(-divisions * col - j, i) |> ignore

                    addFace chunky preLineEast[j] nowLineEast[j] nowLineEast[j + 1] |> ignore
                // 构造西边面（第二面）
                if i < divisions then
                    addPoint chunky nowLineWest[0] <| SphereAxial(-divisions * col - i, i) |> ignore

                for j in 0 .. divisions - i do
                    if j > 0 then
                        addFace chunky preLineWest[j] nowLineWest[j - 1] nowLineWest[j] |> ignore

                        if j < divisions - i then
                            addPoint chunky nowLineWest[j] <| SphereAxial(-divisions * col - i - j, i)
                            |> ignore

                    addFace chunky nowLineWest[j] preLineWest[j + 1] preLineWest[j] |> ignore

                preLineEast <- nowLineEast
                preLineWest <- nowLineWest

    // 构造南部的最后一面（列的第四面）
    let private initSouthTriangle (addPoint: AddPoint) (addFace: AddFace) =
        fun chunky (edges: Vector3 array array) col divisions ->
            let nextCol = (col + 1) % 5
            let southWest = edges[nextCol * 6 + 5] // 向南方连接南极的靠西的边界
            let southEast = edges[col * 6 + 5] // 向南方连接南极的靠东的边界
            let mutable preLine = edges[col * 6 + 4] // 南回归线的边（E -> W）

            for i in 1..divisions do
                let nowLine = Math3dUtil.subdivide southEast[i] southWest[i] <| divisions - i

                if i < divisions then
                    addPoint chunky nowLine[0] <| SphereAxial(-divisions * col - i, divisions + i)
                    |> ignore

                for j in 0 .. divisions - i do
                    if j > 0 then
                        addFace chunky preLine[j] nowLine[j - 1] nowLine[j] |> ignore

                        if j < divisions - i then
                            addPoint chunky nowLine[j]
                            <| SphereAxial(-divisions * col - i - j, divisions + i)
                            |> ignore

                    addFace chunky nowLine[j] preLine[j + 1] preLine[j] |> ignore

                preLine <- nowLine

    // 初始化 Point 和 Face
    let private subdivideIcosahedron (addPoint: AddPoint) (addFace: AddFace) =
        fun chunky divisions ->
            let pn = IcosahedronConstant.vertices[0] // 北极点
            let ps = IcosahedronConstant.vertices[6] // 南极点
            // 轴坐标系（0,0）放在第一组竖列四面的北回归线最东端
            addPoint chunky pn <| SphereAxial(0, -divisions) |> ignore
            addPoint chunky ps <| SphereAxial(-divisions, 2 * divisions) |> ignore
            let edges = HexSphereUtil.genEdgeVectors divisions pn ps

            for col in 0..4 do
                initNorthTriangle addPoint addFace chunky edges col divisions
                initEquatorTwoTriangles addPoint addFace chunky edges col divisions
                initSouthTriangle addPoint addFace chunky edges col divisions

    let private initPointFaceLinks
        (tryHeadPointByPosition: TryHeadPointByPosition)
        (forEachFaceByChunky: ForEachFaceByChunky)
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
        (addPoint: AddPoint)
        (addFace: AddFace)
        (tryHeadPointByPosition: TryHeadPointByPosition)
        (forEachFaceByChunky: ForEachFaceByChunky)
        =
        fun chunky divisions ->
            let time = Time.GetTicksMsec()
            subdivideIcosahedron addPoint addFace chunky divisions
            initPointFaceLinks tryHeadPointByPosition forEachFaceByChunky chunky
            let chunkyType = if chunky then "Chunk" else "Tile" // 不能直接写在输出中
            // 三元表达式直接写在输出中，会报错：Invalid interpolated string.
            // Single quote or verbatim string literals may not be used in interpolated expressions in single quote or verbatim strings.
            // Consider using an explicit 'let' binding for the interpolation expression or use a triple quote string as the outer string literal.
            GD.Print($"--- InitPointsAndFaces for {chunkyType} cost: {Time.GetTicksMsec() - time} ms")

    let searchNearest (point: PointRepoDep) =
        fun (pos: Vector3) chunky ->
            let center = point.SearchNearestCenterPos pos chunky

            point.TryHeadByPosition chunky center
            |> Option.bind (fun pointEntity -> point.TryHeadEntityByPointId pointEntity.Id)

    let getHexFacesAndNeighborCenterIds (point: PointRepoDep) (face: FaceRepoDep) =
        fun chunky (pComp: PointComponent inref) (pEntity: Entity inref) ->
            let hexFaceEntities = face.GetOrderedFaces pComp pEntity
            let hexFaces = hexFaceEntities |> List.map _.GetComponent<FaceComponent>()

            let neighborCenterIds =
                point.GetNeighborCenterPointIds.Invoke(chunky, hexFaces, &pComp) |> Seq.toArray

            (hexFaces |> List.toArray, hexFaceEntities |> List.map _.Id |> List.toArray, neighborCenterIds)

    let initChunks (planet: IPlanet) (point: PointRepoDep) (face: FaceRepoDep) (chunk: ChunkRepoDep) =
        let time = Time.GetTicksMsec()
        initPointsAndFaces point.Add face.Add point.TryHeadByPosition face.ForEachByChunky true planet.ChunkDivisions

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

    let initTiles (planet: IPlanet) (point: PointRepoDep) (face: FaceRepoDep) (tile: TileRepoDep) =
        let mutable time = Time.GetTicksMsec()
        initPointsAndFaces point.Add face.Add point.TryHeadByPosition face.ForEachByChunky false planet.Divisions

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

module private MeshGenerator =
    let private addFaceIndex (surfaceTool: SurfaceTool) i0 i1 i2 =
        surfaceTool.AddIndex i0
        surfaceTool.AddIndex i1
        surfaceTool.AddIndex i2

    let generateMesh (planet: IPlanet) (chunkLoader: IChunkLoader) (tiles: (TileUnitCentroid * TileUnitCorners) seq) =
        for child in chunkLoader.GetChildren() do
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
        chunkLoader.AddChild(meshIns)

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 12:46:30
module HexSphereService =
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

    let initHexSphere
        (planet: IPlanet)
        (chunkLoader: IChunkLoader)
        (tileShaderData: TileShaderData)
        (tileSearcher: TileSearcher)
        (point: PointRepoDep)
        (face: FaceRepoDep)
        (tile: TileRepoDep)
        (chunk: ChunkRepoDep)
        : InitHexSphere =
        fun () ->
            HexSphereInitializer.initChunks planet point face chunk
            HexSphereInitializer.initTiles planet point face tile
            tileShaderData.Initialize(planet)
            tileSearcher.InitSearchData(tile.Count())
            // MeshGenerator.generateMesh planet chunkLoader <| tile.CentroidAndCornersSeq()

    let getDependency
        (planet: IPlanet)
        (chunkLoader: IChunkLoader)
        (tileShaderData: TileShaderData)
        (tileSearcher: TileSearcher)
        (point: PointRepoDep)
        (face: FaceRepoDep)
        (tile: TileRepoDep)
        (chunk: ChunkRepoDep)
        : HexSphereServiceDep =
        { InitHexSphere = initHexSphere planet chunkLoader tileShaderData tileSearcher point face tile chunk
          ClearOldData = clearOldData point face tile chunk }
