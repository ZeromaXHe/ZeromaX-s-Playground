namespace TO.Commons.Constants

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-15 21:43:15
[<AbstractClass; Sealed>]
type IcosahedronConstant =
    static let sqrt5 = Mathf.Sqrt(5f) // √5
    static let sqrt5divBy1 = 1f / sqrt5 // 1/√5

    static member Vertices =
        [ Vector3(0f, 1f, 0f) // 0
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
          Vector3((-5f + sqrt5) / 10f, -sqrt5divBy1, Mathf.Sqrt((5f + sqrt5) / 10f)) ]

    // 每 4 个面一组可以组成从上到下，顺时针旋转的一条
    // 每个面第一个索引是非水平边的那个点
    static member Indices =
        [ 0; 1; 2; // 0
          10; 2; 1; // 1
          1; 9; 10; // 2
          6; 10; 9; // 3
          0; 2; 3; // 4
          11; 3; 2; // 5
          2; 10; 11; // 6
          6; 11; 10; // 7
          0; 3; 4; // 8
          7; 4; 3; // 9
          3; 11; 7; // 10
          6; 7; 11; // 11
          0; 4; 5; // 12
          8; 5; 4; // 13
          4; 7; 8; // 14
          6; 8; 7; // 15
          0; 5; 1; // 16
          9; 1; 5; // 17
          5; 8; 9; // 18
          6; 9; 8] // 19
