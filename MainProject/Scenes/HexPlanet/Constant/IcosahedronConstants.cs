using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constant;

public static class IcosahedronConstants
{
    private static readonly float Sqrt5 = Mathf.Sqrt(5f); // √5
    private static readonly float Sqrt5divBy1 = 1f / Sqrt5; // 1/√5

    public static List<Vector3> Vertices =
    [
        new Vector3(0f, 1f, 0f), // 0
        new Vector3(2f * Sqrt5divBy1, Sqrt5divBy1, 0f),
        new Vector3((5f - Sqrt5) / 10f, Sqrt5divBy1, Mathf.Sqrt((5f + Sqrt5) / 10f)),
        new Vector3((-5f - Sqrt5) / 10f, Sqrt5divBy1, Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((-5f - Sqrt5) / 10f, Sqrt5divBy1, -Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((5f - Sqrt5) / 10f, Sqrt5divBy1, -Mathf.Sqrt((5f + Sqrt5) / 10f)),
        new Vector3(0f, -1f, 0f), // 6
        new Vector3(-2f * Sqrt5divBy1, -Sqrt5divBy1, 0f),
        new Vector3((-5f + Sqrt5) / 10f, -Sqrt5divBy1, -Mathf.Sqrt((5f + Sqrt5) / 10f)),
        new Vector3((5f + Sqrt5) / 10f, -Sqrt5divBy1, -Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((5f + Sqrt5) / 10f, -Sqrt5divBy1, Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((-5f + Sqrt5) / 10f, -Sqrt5divBy1, Mathf.Sqrt((5f + Sqrt5) / 10f)),
    ];

    // 每 4 个面一组可以组成从上到下，顺时针旋转的一条
    // 每个面第一个索引是非水平边的那个点
    public static List<int> Indices =
    [
        0, 1, 2, // 0
        10, 2, 1,
        1, 9, 10,
        6, 10, 9,
        0, 2, 3, // 4
        11, 3, 2,
        2, 10, 11,
        6, 11, 10,
        0, 3, 4, // 8
        7, 4, 3,
        3, 11, 7,
        6, 7, 11,
        0, 4, 5, // 12
        8, 5, 4,
        4, 7, 8,
        6, 8, 7,
        0, 5, 1, // 16
        9, 1, 5,
        5, 8, 9,
        6, 9, 8,
    ];
}