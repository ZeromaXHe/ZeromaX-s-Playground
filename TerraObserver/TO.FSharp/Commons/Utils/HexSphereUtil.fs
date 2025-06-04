namespace TO.FSharp.Commons.Utils

open Godot
open TO.FSharp.Commons.Constants.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-01 18:27:01
module HexSphereUtil =
    let genEdgeVectors divisions pn ps =
        let points = IcosahedronConstant.vertices
        let indices = IcosahedronConstant.indices
        let edges: Vector3 array array = Array.zeroCreate 30 // 30 条边
        // 初始化所有的边上的点位置
        for col in 0..4 do
            // p1 到 p4 映射到平面上是竖列四面组中间的四个点，中间 Z 字型边按顺序连接：p2，p1，p3，p4
            let p1 = points[indices[col * 4][1]]
            let p2 = points[indices[col * 4][2]]
            let p3 = points[indices[col * 4 + 1][0]]
            let p4 = points[indices[col * 4 + 2][1]]
            // 每个竖列四面组有六个属于它的边（右边两个三趾鸡爪形的从上到下的边，列左边界的三条边不属于它）
            edges[col * 6] <- Math3dUtil.subdivide pn p1 divisions // 从左上到右下
            edges[col * 6 + 1] <- Math3dUtil.subdivide p1 p2 divisions // 从右往左
            edges[col * 6 + 2] <- Math3dUtil.subdivide p1 p3 divisions // 从右上到左下
            edges[col * 6 + 3] <- Math3dUtil.subdivide p1 p4 divisions // 从左上到右下
            edges[col * 6 + 4] <- Math3dUtil.subdivide p4 p3 divisions // 从右往左
            edges[col * 6 + 5] <- Math3dUtil.subdivide p4 ps divisions // 从右上到左下

        edges