using System.Collections.Generic;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constant;

public static class IcosahedronConstants
{
    private static readonly float Sqrt5 = Mathf.Sqrt(5f); // √5
    private static readonly float Sqrt5divBy1 = 1f / Sqrt5; // 1/√5

    public static List<Vector3> Vertices =
    [
        new Vector3(0f, 1f, 0f),
        new Vector3(2f * Sqrt5divBy1, Sqrt5divBy1, 0f),
        new Vector3((5f - Sqrt5) / 10f, Sqrt5divBy1, Mathf.Sqrt((5f + Sqrt5) / 10f)),
        new Vector3((-5f - Sqrt5) / 10f, Sqrt5divBy1, Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((-5f - Sqrt5) / 10f, Sqrt5divBy1, -Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((5f - Sqrt5) / 10f, Sqrt5divBy1, -Mathf.Sqrt((5f + Sqrt5) / 10f)),
        new Vector3(0f, -1f, 0f),
        new Vector3(-2f * Sqrt5divBy1, -Sqrt5divBy1, 0f),
        new Vector3((-5f + Sqrt5) / 10f, -Sqrt5divBy1, -Mathf.Sqrt((5f + Sqrt5) / 10f)),
        new Vector3((5f + Sqrt5) / 10f, -Sqrt5divBy1, -Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((5f + Sqrt5) / 10f, -Sqrt5divBy1, Mathf.Sqrt((5f - Sqrt5) / 10f)),
        new Vector3((-5f + Sqrt5) / 10f, -Sqrt5divBy1, Mathf.Sqrt((5f + Sqrt5) / 10f)),
    ];

    public static List<int> Indices =
    [
        0, 1, 2,
        0, 2, 3,
        0, 3, 4,
        0, 4, 5,
        0, 5, 1,
        1, 9, 10,
        1, 10, 2,
        2, 10, 11,
        2, 11, 3,
        3, 11, 7,
        3, 7, 4,
        4, 7, 8,
        4, 8, 5,
        5, 8, 9,
        5, 9, 1,
        6, 8, 7,
        6, 9, 8,
        6, 10, 9,
        6, 11, 10,
        6, 7, 11,
    ];
}