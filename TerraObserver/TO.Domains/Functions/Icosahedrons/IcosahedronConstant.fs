namespace TO.Domains.Functions.Icosahedrons

open Godot
open TO.Domains.Functions.Maths.MathConstant

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-28 23:10:28
module IcosahedronConstant =
    let vertices =
        [| Vector3(0f, 1f, 0f) // 0
           Vector3(2f * sqrt5divBy1, sqrt5divBy1, 0f)
           Vector3((5f - sqrt5) / 10f, sqrt5divBy1, Mathf.Sqrt((5f + sqrt5) / 10f))
           Vector3((-5f - sqrt5) / 10f, sqrt5divBy1, Mathf.Sqrt((5f - sqrt5) / 10f))
           Vector3((-5f - sqrt5) / 10f, sqrt5divBy1, - Mathf.Sqrt((5f - sqrt5) / 10f))
           Vector3((5f - sqrt5) / 10f, sqrt5divBy1, - Mathf.Sqrt((5f + sqrt5) / 10f))
           Vector3(0f, -1f, 0f) // 6
           Vector3(-2f * sqrt5divBy1, -sqrt5divBy1, 0f)
           Vector3((-5f + sqrt5) / 10f, -sqrt5divBy1, - Mathf.Sqrt((5f + sqrt5) / 10f))
           Vector3((5f + sqrt5) / 10f, -sqrt5divBy1, - Mathf.Sqrt((5f - sqrt5) / 10f))
           Vector3((5f + sqrt5) / 10f, -sqrt5divBy1, Mathf.Sqrt((5f - sqrt5) / 10f))
           Vector3((-5f + sqrt5) / 10f, -sqrt5divBy1, Mathf.Sqrt((5f + sqrt5) / 10f)) |]

    // 每 4 个面一组可以组成从上到下，顺时针旋转的一条
    // 每个面第一个索引是非水平边的那个点
    let indices =
        // 突然发现 F# 强制数组元素过多时换行的特点，正好鼓励你建立多维数组
        [| [| 0; 1; 2 |] // 0
           [| 10; 2; 1 |]
           [| 1; 9; 10 |]
           [| 6; 10; 9 |]
           [| 0; 2; 3 |] // 4
           [| 11; 3; 2 |]
           [| 2; 10; 11 |]
           [| 6; 11; 10 |]
           [| 0; 3; 4 |] // 8
           [| 7; 4; 3 |]
           [| 3; 11; 7 |]
           [| 6; 7; 11 |]
           [| 0; 4; 5 |] // 12
           [| 8; 5; 4 |]
           [| 4; 7; 8 |]
           [| 6; 8; 7 |]
           [| 0; 5; 1 |] // 16
           [| 9; 1; 5 |]
           [| 5; 8; 9 |]
           [| 6; 9; 8 |] |]
