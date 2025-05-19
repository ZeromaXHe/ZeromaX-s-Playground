namespace TO.Domains.Planets.Functions

open Godot
open TO.Commons.Constants
open TO.Commons.Structs.HexSphereGrid
open TO.Commons.Utils
open TO.Domains.Planets.Types.FacePoint

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 10:26:18
module FacePointFunc =
    let private genEdgeVectors divisions pn ps =
        let points = IcosahedronConstant.Vertices
        let indices = IcosahedronConstant.Indices
        let edges: Vector3 array array = Array.zeroCreate 30 // 30 条边
        // 初始化所有的边上的点位置
        for col in 0..4 do
            // p1 到 p4 映射到平面上是竖列四面组中间的四个点，中间 Z 字型边按顺序连接：p2，p1，p3，p4
            let p1 = points[indices[col * 12 + 1]]
            let p2 = points[indices[col * 12 + 2]]
            let p3 = points[indices[col * 12 + 3]]
            let p4 = points[indices[col * 12 + 7]]
            // 每个竖列四面组有六个属于它的边（右边两个三趾鸡爪形的从上到下的边，列左边界的三条边不属于它）
            edges[col * 6] <- Math3dUtil.Subdivide(pn, p1, divisions)
            edges[col * 6 + 1] <- Math3dUtil.Subdivide(p1, p2, divisions)
            edges[col * 6 + 2] <- Math3dUtil.Subdivide(p1, p3, divisions)
            edges[col * 6 + 3] <- Math3dUtil.Subdivide(p1, p4, divisions)
            edges[col * 6 + 4] <- Math3dUtil.Subdivide(p4, p3, divisions)
            edges[col * 6 + 5] <- Math3dUtil.Subdivide(p4, ps, divisions)

        edges

    // 构造北部的第一个面
    let private initNorthTriangle (edges: Vector3 array array) col divisions faceAndPointAdders =
        let nextCol = (col + 1) % 5
        let northEast = edges[col * 6] // 北极出来的靠东的边界
        let northWest = edges[nextCol * 6] // 北极出来的靠西的边界
        let tropicOfCancer = edges[col * 6 + 1] // 北回归线的边（E -> W）
        let mutable preLine = [| northEast[0] |] // 初始为北极点

        let mutable (faceAdders: FaceAdder list, pointAdders: PointAdder list) =
            faceAndPointAdders

        for i in 1..divisions do
            let nowLine =
                if i = divisions then
                    tropicOfCancer
                else
                    Math3dUtil.Subdivide(northEast[i], northWest[i], i)

            pointAdders <-
                if i = divisions then
                    { Position = nowLine[0]
                      Coords = SphereAxial(-divisions * col, 0) }
                    :: pointAdders
                else
                    { Position = nowLine[i]
                      Coords = SphereAxial(-divisions * col, i - divisions) }
                    :: pointAdders

            for j in 0 .. i - 1 do
                if j > 0 then
                    faceAdders <- { TriVertices = [| nowLine[j]; preLine[j]; preLine[j - 1] |] } :: faceAdders

                    pointAdders <-
                        if (i = divisions) then
                            { Position = nowLine[j]
                              Coords = SphereAxial(-divisions * col - j, 0) }
                            :: pointAdders
                        else
                            { Position = nowLine[j]
                              Coords = SphereAxial(-divisions * col - j, i - divisions) }
                            :: pointAdders

                faceAdders <- { TriVertices = [| preLine[j]; nowLine[j]; nowLine[j + 1] |] } :: faceAdders

            preLine <- nowLine

        (faceAdders, pointAdders)

    // 赤道两个面（第二、三面）的构造
    let private initEquatorTwoTriangles (edges: Vector3 array array) col divisions faceAndPointAdders =
        let nextCol = (col + 1) % 5
        let equatorWest = edges[nextCol * 6 + 3] // 向东南方斜跨赤道的靠西的边界
        let equatorMid = edges[col * 6 + 2] // 向西南方斜跨赤道的中间的边
        let equatorEast = edges[col * 6 + 3] // 向东南方斜跨赤道的靠东的边界
        let tropicOfCapricorn = edges[col * 6 + 4] // 南回归线的边（E -> W）
        let mutable preLineWest = edges[col * 6 + 1] // 北回归线的边（E -> W）
        let mutable preLineEast = [| equatorEast[0] |]

        let mutable (faceAdders: FaceAdder list, pointAdders: PointAdder list) =
            faceAndPointAdders

        for i in 1..divisions do
            let nowLineEast =
                if i = divisions then
                    tropicOfCapricorn
                else
                    Math3dUtil.Subdivide(equatorEast[i], equatorMid[i], i)

            let nowLineWest = Math3dUtil.Subdivide(equatorMid[i], equatorWest[i], divisions - i)
            // 构造东边面（第三面）
            pointAdders <-
                { Position = nowLineEast[0]
                  Coords = SphereAxial(-divisions * col, i) }
                :: pointAdders

            for j in 0 .. i - 1 do
                if j > 0 then
                    faceAdders <-
                        { TriVertices = [| nowLineEast[j]; preLineEast[j]; preLineEast[j - 1] |] }
                        :: faceAdders

                    pointAdders <-
                        { Position = nowLineEast[j]
                          Coords = SphereAxial(-divisions * col - j, i) }
                        :: pointAdders

                faceAdders <-
                    { TriVertices = [| preLineEast[j]; nowLineEast[j]; nowLineEast[j + 1] |] }
                    :: faceAdders
            // 构造西边面（第二面）
            if i < divisions then
                pointAdders <-
                    { Position = nowLineWest[0]
                      Coords = SphereAxial(-divisions * col - i, i) }
                    :: pointAdders

            for j in 0 .. divisions - i do
                if j > 0 then
                    faceAdders <-
                        { TriVertices = [| preLineWest[j]; nowLineWest[j - 1]; nowLineWest[j] |] }
                        :: faceAdders

                    if j < divisions - i then
                        pointAdders <-
                            { Position = nowLineWest[j]
                              Coords = SphereAxial(-divisions * col - i - j, i) }
                            :: pointAdders

                faceAdders <-
                    { TriVertices = [| nowLineWest[j]; preLineWest[j + 1]; preLineWest[j] |] }
                    :: faceAdders

            preLineEast <- nowLineEast
            preLineWest <- nowLineWest

        (faceAdders, pointAdders)

    // 构造南部的最后一面（列的第四面）
    let private initSouthTriangle (edges: Vector3 array array) col divisions faceAndPointAdders =
        let nextCol = (col + 1) % 5
        let southWest = edges[nextCol * 6 + 5] // 向南方连接南极的靠西的边界
        let southEast = edges[col * 6 + 5] // 向南方连接南极的靠东的边界
        let mutable preLine = edges[col * 6 + 4] // 南回归线的边（E -> W）

        let mutable (faceAdders: FaceAdder list, pointAdders: PointAdder list) =
            faceAndPointAdders

        for i in 1..divisions do
            let nowLine = Math3dUtil.Subdivide(southEast[i], southWest[i], divisions - i)

            if i < divisions then
                pointAdders <-
                    { Position = nowLine[0]
                      Coords = SphereAxial(-divisions * col - i, divisions + i) }
                    :: pointAdders

            for j in 0 .. divisions - i do
                if j > 0 then
                    faceAdders <- { TriVertices = [| preLine[j]; nowLine[j - 1]; nowLine[j] |] } :: faceAdders

                    if j < divisions - i then
                        pointAdders <-
                            { Position = nowLine[j]
                              Coords = SphereAxial(-divisions * col - i - j, divisions + i) }
                            :: pointAdders

                faceAdders <- { TriVertices = [| nowLine[j]; preLine[j + 1]; preLine[j] |] } :: faceAdders

            preLine <- nowLine

        (faceAdders, pointAdders)

    // 初始化 Point 和 Face
    let subdivideIcosahedron divisions =
        let pn = IcosahedronConstant.Vertices[0] // 北极点
        let ps = IcosahedronConstant.Vertices[6] // 南极点

        let mutable faceAdders: FaceAdder list = []
        let mutable pointAdders: PointAdder list = []
        // 轴坐标系（0,0）放在第一组竖列四面的北回归线最东端
        pointAdders <-
            { Position = pn
              Coords = SphereAxial(0, -divisions) }
            :: pointAdders

        pointAdders <-
            { Position = ps
              Coords = SphereAxial(-divisions, 2 * divisions) }
            :: pointAdders

        let edges = genEdgeVectors divisions pn ps
        let mutable result = (faceAdders, pointAdders)

        for col in 0..4 do
            result <-
                result
                |> (initNorthTriangle edges col divisions
                    >> initEquatorTwoTriangles edges col divisions
                    >> initSouthTriangle edges col divisions)

        result
