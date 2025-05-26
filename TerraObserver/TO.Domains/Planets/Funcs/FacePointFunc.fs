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
            edges[col * 6] <- Math3dUtil.Subdivide(pn, p1, divisions) // 从左上到右下
            edges[col * 6 + 1] <- Math3dUtil.Subdivide(p1, p2, divisions) // 从右往左
            edges[col * 6 + 2] <- Math3dUtil.Subdivide(p1, p3, divisions) // 从右上到左下
            edges[col * 6 + 3] <- Math3dUtil.Subdivide(p1, p4, divisions) // 从左上到右下
            edges[col * 6 + 4] <- Math3dUtil.Subdivide(p4, p3, divisions) // 从右往左
            edges[col * 6 + 5] <- Math3dUtil.Subdivide(p4, ps, divisions) // 从右上到左下

        edges

    // 构造北部的第一个面
    let private initNorthTriangle pointAdder faceAdder =
        fun (edges: Vector3 array array) col divisions ->
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
                        Math3dUtil.Subdivide(northEast[i], northWest[i], i)

                if i = divisions then
                    pointAdder nowLine[0] <| SphereAxial(-divisions * col, 0)
                else
                    pointAdder nowLine[i] <| SphereAxial(-divisions * col, i - divisions)

                for j in 0 .. i - 1 do
                    if j > 0 then
                        faceAdder nowLine[j] preLine[j] preLine[j - 1]

                        pointAdder
                            nowLine[j]
                            (if i = divisions then
                                 SphereAxial(-divisions * col - j, 0)
                             else
                                 SphereAxial(-divisions * col - j, i - divisions))

                    faceAdder preLine[j] nowLine[j] nowLine[j + 1]

                preLine <- nowLine

    // 赤道两个面（第二、三面）的构造
    let private initEquatorTwoTriangles pointAdder faceAdder =
        fun (edges: Vector3 array array) col divisions ->
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
                        Math3dUtil.Subdivide(equatorEast[i], equatorMid[i], i)

                let nowLineWest = Math3dUtil.Subdivide(equatorMid[i], equatorWest[i], divisions - i)
                // 构造东边面（第三面）
                pointAdder nowLineEast[0] <| SphereAxial(-divisions * col, i)

                for j in 0 .. i - 1 do
                    if j > 0 then
                        faceAdder nowLineEast[j] preLineEast[j] preLineEast[j - 1]
                        pointAdder nowLineEast[j] <| SphereAxial(-divisions * col - j, i)

                    faceAdder preLineEast[j] nowLineEast[j] nowLineEast[j + 1]
                // 构造西边面（第二面）
                if i < divisions then
                    pointAdder nowLineWest[0] <| SphereAxial(-divisions * col - i, i)

                for j in 0 .. divisions - i do
                    if j > 0 then
                        faceAdder preLineWest[j] nowLineWest[j - 1] nowLineWest[j]

                        if j < divisions - i then
                            pointAdder nowLineWest[j] <| SphereAxial(-divisions * col - i - j, i)

                    faceAdder nowLineWest[j] preLineWest[j + 1] preLineWest[j]

                preLineEast <- nowLineEast
                preLineWest <- nowLineWest

    // 构造南部的最后一面（列的第四面）
    let private initSouthTriangle pointAdder faceAdder =
        fun (edges: Vector3 array array) col divisions ->
            let nextCol = (col + 1) % 5
            let southWest = edges[nextCol * 6 + 5] // 向南方连接南极的靠西的边界
            let southEast = edges[col * 6 + 5] // 向南方连接南极的靠东的边界
            let mutable preLine = edges[col * 6 + 4] // 南回归线的边（E -> W）

            for i in 1..divisions do
                let nowLine = Math3dUtil.Subdivide(southEast[i], southWest[i], divisions - i)

                if i < divisions then
                    pointAdder nowLine[0] <| SphereAxial(-divisions * col - i, divisions + i)

                for j in 0 .. divisions - i do
                    if j > 0 then
                        faceAdder preLine[j] nowLine[j - 1] nowLine[j]

                        if j < divisions - i then
                            pointAdder nowLine[j] <| SphereAxial(-divisions * col - i - j, divisions + i)

                    faceAdder nowLine[j] preLine[j + 1] preLine[j]

                preLine <- nowLine

    // 初始化 Point 和 Face
    let subdivideIcosahedron pointAdder faceAdder =
        fun divisions ->
            let pn = IcosahedronConstant.Vertices[0] // 北极点
            let ps = IcosahedronConstant.Vertices[6] // 南极点
            // 轴坐标系（0,0）放在第一组竖列四面的北回归线最东端
            pointAdder pn <| SphereAxial(0, -divisions)
            pointAdder ps <| SphereAxial(-divisions, 2 * divisions)
            let edges = genEdgeVectors divisions pn ps

            for col in 0..4 do
                initNorthTriangle pointAdder faceAdder edges col divisions
                initEquatorTwoTriangles pointAdder faceAdder edges col divisions
                initSouthTriangle pointAdder faceAdder edges col divisions
