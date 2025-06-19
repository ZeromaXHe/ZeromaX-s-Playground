namespace TO.FSharp.Repos.Commands.HexSpheres

open Friflo.Engine.ECS
open Godot
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Constants.HexSpheres
open TO.Domains.Functions.HexSpheres
open TO.Domains.Relations.HexSpheres.Points
open TO.Domains.Structs.HexSphereGrids
open TO.Domains.Utils.Commons
open TO.Domains.Utils.HexSpheres
open TO.FSharp.Repos.Data.Commons
open TO.FSharp.Repos.Queries.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 21:10:19
module HexSphereInitCommand =
    // 构造北部的第一个面
    let private initNorthTriangle env =
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
                    PointCommand.add env chunky nowLine[0] <| SphereAxial(-divisions * col, 0)
                    |> ignore
                else
                    PointCommand.add env chunky nowLine[i]
                    <| SphereAxial(-divisions * col, i - divisions)
                    |> ignore

                for j in 0 .. i - 1 do
                    if j > 0 then
                        FaceCommand.add env chunky nowLine[j] preLine[j] preLine[j - 1] |> ignore

                        PointCommand.add
                            env
                            chunky
                            nowLine[j]
                            (if i = divisions then
                                 SphereAxial(-divisions * col - j, 0)
                             else
                                 SphereAxial(-divisions * col - j, i - divisions))
                        |> ignore

                    FaceCommand.add env chunky preLine[j] nowLine[j] nowLine[j + 1] |> ignore

                preLine <- nowLine

    // 赤道两个面（第二、三面）的构造
    let private initEquatorTwoTriangles env =
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
                PointCommand.add env chunky nowLineEast[0] <| SphereAxial(-divisions * col, i)
                |> ignore

                for j in 0 .. i - 1 do
                    if j > 0 then
                        FaceCommand.add env chunky nowLineEast[j] preLineEast[j] preLineEast[j - 1]
                        |> ignore

                        PointCommand.add env chunky nowLineEast[j]
                        <| SphereAxial(-divisions * col - j, i)
                        |> ignore

                    FaceCommand.add env chunky preLineEast[j] nowLineEast[j] nowLineEast[j + 1]
                    |> ignore
                // 构造西边面（第二面）
                if i < divisions then
                    PointCommand.add env chunky nowLineWest[0]
                    <| SphereAxial(-divisions * col - i, i)
                    |> ignore

                for j in 0 .. divisions - i do
                    if j > 0 then
                        FaceCommand.add env chunky preLineWest[j] nowLineWest[j - 1] nowLineWest[j]
                        |> ignore

                        if j < divisions - i then
                            PointCommand.add env chunky nowLineWest[j]
                            <| SphereAxial(-divisions * col - i - j, i)
                            |> ignore

                    FaceCommand.add env chunky nowLineWest[j] preLineWest[j + 1] preLineWest[j]
                    |> ignore

                preLineEast <- nowLineEast
                preLineWest <- nowLineWest

    // 构造南部的最后一面（列的第四面）
    let private initSouthTriangle env =
        fun chunky (edges: Vector3 array array) col divisions ->
            let nextCol = (col + 1) % 5
            let southWest = edges[nextCol * 6 + 5] // 向南方连接南极的靠西的边界
            let southEast = edges[col * 6 + 5] // 向南方连接南极的靠东的边界
            let mutable preLine = edges[col * 6 + 4] // 南回归线的边（E -> W）

            for i in 1..divisions do
                let nowLine = Math3dUtil.subdivide southEast[i] southWest[i] <| divisions - i

                if i < divisions then
                    PointCommand.add env chunky nowLine[0]
                    <| SphereAxial(-divisions * col - i, divisions + i)
                    |> ignore

                for j in 0 .. divisions - i do
                    if j > 0 then
                        FaceCommand.add env chunky preLine[j] nowLine[j - 1] nowLine[j] |> ignore

                        if j < divisions - i then
                            PointCommand.add env chunky nowLine[j]
                            <| SphereAxial(-divisions * col - i - j, divisions + i)
                            |> ignore

                    FaceCommand.add env chunky nowLine[j] preLine[j + 1] preLine[j] |> ignore

                preLine <- nowLine

    // 初始化 Point 和 Face
    let private subdivideIcosahedron env chunky divisions =
        let pn = IcosahedronConstant.vertices[0] // 北极点
        let ps = IcosahedronConstant.vertices[6] // 南极点
        // 轴坐标系（0,0）放在第一组竖列四面的北回归线最东端
        PointCommand.add env chunky pn <| SphereAxial(0, -divisions) |> ignore

        PointCommand.add env chunky ps <| SphereAxial(-divisions, 2 * divisions)
        |> ignore

        let edges = HexSphereUtil.genEdgeVectors divisions pn ps

        for col in 0..4 do
            initNorthTriangle env chunky edges col divisions
            initEquatorTwoTriangles env chunky edges col divisions
            initSouthTriangle env chunky edges col divisions

    let private initPointFaceLinks (dataEnv: #IEntityStore) =
        fun chunky ->
            let tag = ChunkFunction.chunkyTag chunky

            dataEnv.EntityStore
                .Query<FaceComponent>()
                .AllTags(&tag)
                .ForEachEntity(fun faceComp faceEntity ->
                    let relatePointToFace v =
                        // 给每个点建立它与所归属的面的关系
                        PointQuery.tryHeadByPosition dataEnv chunky v
                        |> Option.iter (fun pointEntity ->
                            let relation = PointToFaceId(faceEntity.Id)
                            pointEntity.AddRelation(&relation) |> ignore)

                    relatePointToFace faceComp.Vertex1
                    relatePointToFace faceComp.Vertex2
                    relatePointToFace faceComp.Vertex3)

    let initPointsAndFaces env =
        fun chunky divisions ->
            let time = Time.GetTicksMsec()
            subdivideIcosahedron env chunky divisions
            initPointFaceLinks env chunky
            let chunkyType = if chunky then "Chunk" else "Tile" // 不能直接写在输出中
            // 三元表达式直接写在输出中，会报错：Invalid interpolated string.
            // Single quote or verbatim string literals may not be used in interpolated expressions in single quote or verbatim strings.
            // Consider using an explicit 'let' binding for the interpolation expression or use a triple quote string as the outer string literal.
            GD.Print($"--- InitPointsAndFaces for {chunkyType} cost: {Time.GetTicksMsec() - time} ms")
