namespace ProjectFS.HexPlanet

open Godot

// 正二十面体
module Icosahedron =
    let sqrt5 = Mathf.Sqrt 5f // √5
    let sqrt5divBy1 = 1f / sqrt5 // 1/√5
    // 有北极和南极顶点的坐标系
    // a1 与 ak (2 <= k <= 6) 共边
    // 对于 2 <= k <= 6 的顶点，它们与 a1, a{2+(k-1)%5}, a{2+(k+2)%5}, a{8+k%5}, a{8+(k+1)%5} 共边
    // a7 与 ak (8 <= k <= 12) 共边
    // 对于 8 <= k <= 12 的顶点，它们与 a7, a{8+(k-2)%5}, a{8+(k+1)%5}, a{2+(k-1)%5}, a{2+k%5} 共边
    // 参考： https://math.stackexchange.com/questions/2174594/co-ordinates-of-the-vertices-an-icosahedron-relative-to-its-centroid
    let vertices =
        [| Vector3(0f, 1f, 0f)
           Vector3(2f * sqrt5divBy1, sqrt5divBy1, 0f)
           Vector3((5f - sqrt5) / 10f, sqrt5divBy1, Mathf.Sqrt((5f + sqrt5) / 10f))
           Vector3((-5f - sqrt5) / 10f, sqrt5divBy1, Mathf.Sqrt((5f - sqrt5) / 10f))
           Vector3((-5f - sqrt5) / 10f, sqrt5divBy1, - Mathf.Sqrt((5f - sqrt5) / 10f))
           Vector3((5f - sqrt5) / 10f, sqrt5divBy1, - Mathf.Sqrt((5f + sqrt5) / 10f))
           Vector3(0f, -1f, 0f)
           Vector3(-2f * sqrt5divBy1, -sqrt5divBy1, 0f)
           Vector3((-5f + sqrt5) / 10f, -sqrt5divBy1, - Mathf.Sqrt((5f + sqrt5) / 10f))
           Vector3((5f + sqrt5) / 10f, -sqrt5divBy1, - Mathf.Sqrt((5f - sqrt5) / 10f))
           Vector3((5f + sqrt5) / 10f, -sqrt5divBy1, Mathf.Sqrt((5f - sqrt5) / 10f))
           Vector3((-5f + sqrt5) / 10f, -sqrt5divBy1, Mathf.Sqrt((5f + sqrt5) / 10f)) |]

    // Godot 中三角形图元模式的正面使用顺时针 缠绕顺序。
    let indices =
        [| 0; 1; 2
           0; 2; 3
           0; 3; 4
           0; 4; 5
           0; 5; 1
           1; 9; 10
           1; 10; 2
           2; 10; 11
           2; 11; 3
           3; 11; 7
           3; 7; 4
           4; 7; 8
           4; 8; 5
           5; 8; 9
           5; 9; 1
           6; 8; 7
           6; 9; 8
           6; 10; 9
           6; 11; 10
           6; 7; 11 |]
