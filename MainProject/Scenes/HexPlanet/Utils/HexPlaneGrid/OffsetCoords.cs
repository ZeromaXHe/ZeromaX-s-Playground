using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils.HexPlaneGrid;

/// <summary>
/// 偏移坐标系
/// 
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 18:02
/// </summary>
public readonly struct OffsetCoords
{
    public enum Type
    {
        All,
        OddR, // Godot TileMap 的 Stacked 其实就是对应这种 odd-r
        EvenR, // Godot TileMap 的 Stacked Offset -> even-r
        OddQ,
        EvenQ,
        Error
    }

    public readonly Type Tp;
    public readonly int Col;
    public readonly int Row;

    private OffsetCoords(int col, int row, Type type)
    {
        Col = col;
        Row = row;
        Tp = type;
    }

    public OffsetCoords(int col, int row) : this(col, row, Type.All)
    {
    }

    public static OffsetCoords OddR(int col, int row) => new(col, row, Type.OddR);
    public static OffsetCoords EvenR(int col, int row) => new(col, row, Type.EvenR);
    public static OffsetCoords OddQ(int col, int row) => new(col, row, Type.OddQ);
    public static OffsetCoords EvenQ(int col, int row) => new(col, row, Type.EvenQ);

    /// <summary>
    /// 轴坐标系转换为 odd-r 坐标
    /// Implementation note:
    /// I use a&1 (bitwise and) instead of a%2 (modulo) to detect whether something is even (0) or odd (1),
    /// because it works with negative numbers too.
    /// 上面说的其实就是我自己在之前也碰到了的负数 bug，用按位与（&）来解决确实方便
    /// </summary>
    /// <param name="a">轴坐标系</param>
    /// <returns></returns>
    public static OffsetCoords OddR(AxialCoords a) => OddR(a.Q + (a.R - (a.R & 1)) / 2, a.R);

    // 轴坐标系转换为 even-r 坐标
    public static OffsetCoords EvenR(AxialCoords a) => EvenR(a.Q + (a.R + (a.R & 1)) / 2, a.R);

    // 轴坐标系转换为 odd-q 坐标
    public static OffsetCoords OddQ(AxialCoords a) => OddQ(a.Q, a.R + (a.Q - (a.Q & 1)) / 2);

    // 轴坐标系转换为 even-q 坐标
    public static OffsetCoords EvenQ(AxialCoords a) => EvenQ(a.Q, a.R + (a.Q + (a.Q & 1)) / 2);
    public static readonly OffsetCoords Error = new(int.MinValue, int.MinValue, Type.Error);

    private static readonly OffsetCoords[][] OddRDirDiff =
    [
        // 偶数行
        [
            OddR(1, 0), OddR(0, -1), OddR(-1, -1), // 右，右上，左上
            OddR(-1, 0), OddR(-1, 1), OddR(0, 1) // 左，左下，右下
        ],
        // 奇数行
        [
            OddR(1, 0), OddR(1, -1), OddR(0, -1), // 右，右下，左上
            OddR(-1, 0), OddR(0, 1), OddR(1, 1) // 左，左下，右上
        ]
    ];

    private static readonly OffsetCoords[][] EvenRDirDiff =
    [
        // 偶数行
        [
            EvenR(1, 0), EvenR(1, -1), EvenR(0, -1), // 右，右上，左上
            EvenR(-1, 0), EvenR(0, 1), EvenR(1, 1) // 左，左下，右下
        ],
        // 奇数行
        [
            EvenR(1, 0), EvenR(0, -1), EvenR(-1, -1), // 右，右下，左上
            EvenR(-1, 0), EvenR(-1, 1), EvenR(0, 1) // 左，左下，右上
        ]
    ];

    private static readonly OffsetCoords[][] OddQDirDiff =
    [
        // 偶数列
        [
            OddQ(1, 0), OddQ(1, -1), OddQ(0, -1), // 右下，右上，上
            OddQ(-1, -1), OddQ(-1, 0), OddQ(0, 1) // 左上，左下，下
        ],
        // 奇数列
        [
            OddQ(1, 1), OddQ(1, 0), OddQ(0, -1), // 右下，右上，上
            OddQ(-1, 0), OddQ(-1, 1), OddQ(0, 1) // 左上，左下，下
        ]
    ];

    private static readonly OffsetCoords[][] EvenQDirDiff =
    [
        // 偶数列
        [
            EvenQ(1, 1), EvenQ(1, 0), EvenQ(0, -1), // 右下，右上，上
            EvenQ(-1, 0), EvenQ(-1, 1), EvenQ(0, 1) // 左上，左下，下
        ],
        // 奇数列
        [
            EvenQ(1, 0), EvenQ(1, -1), EvenQ(0, -1), // 右下，右上，上
            EvenQ(-1, -1), EvenQ(-1, 0), EvenQ(0, 1) // 左上，左下，下
        ]
    ];

    private static OffsetCoords OddRDirectionVec(int parity, PointyTopDirection direction) =>
        OddRDirDiff[parity][(int)direction];

    private static OffsetCoords EvenRDirectionVec(int parity, PointyTopDirection direction) =>
        EvenRDirDiff[parity][(int)direction];

    private static OffsetCoords OddQDirectionVec(int parity, FlatTopDirection direction) =>
        OddQDirDiff[parity][(int)direction];

    private static OffsetCoords EvenQDirectionVec(int parity, FlatTopDirection direction) =>
        EvenQDirDiff[parity][(int)direction];

    public static OffsetCoords operator +(OffsetCoords o1, OffsetCoords o2)
    {
        if (o1.Tp == Type.Error || o2.Tp == Type.Error ||
            (o1.Tp != o2.Tp && o1.Tp != Type.All && o2.Tp != Type.All))
        {
            GD.PrintErr("OffsetCoords + | 相加类型错误");
            return Error;
        }

        var resType = o1.Tp != Type.All ? o1.Tp : o2.Tp;
        return new OffsetCoords(o1.Col + o2.Col, o1.Row + o2.Row, resType);
    }

    public static OffsetCoords operator -(OffsetCoords o1, OffsetCoords o2)
    {
        if (o1.Tp == Type.Error || o2.Tp == Type.Error ||
            (o1.Tp != o2.Tp && o1.Tp != Type.All && o2.Tp != Type.All))
        {
            GD.PrintErr("OffsetCoords - | 相减类型错误");
            return Error;
        }

        var resType = o1.Tp != Type.All ? o1.Tp : o2.Tp;
        return new OffsetCoords(o1.Col - o2.Col, o1.Row - o2.Row, resType);
    }

    public int DistanceTo(OffsetCoords o)
    {
        var a = ToAxial();
        var b = o.ToAxial();
        if (a.Equals(AxialCoords.Error) || b.Equals(AxialCoords.Error))
            return -1;
        return a.DistanceTo(b);
    }

    public OffsetCoords Neighbor(PointyTopDirection direction)
    {
        var parity = Row & 1;
        var res = Tp switch
        {
            Type.OddR => this + OddRDirectionVec(parity, direction),
            Type.EvenR => this + EvenRDirectionVec(parity, direction),
            _ => Error
        };
        if (res.Equals(Error)) GD.PrintErr("OffsetCoords.Neighbor | 类型和方向不匹配");
        return res;
    }

    public OffsetCoords Neighbor(FlatTopDirection direction)
    {
        var parity = Col & 1;
        var res = Tp switch
        {
            Type.OddQ => this + OddQDirectionVec(parity, direction),
            Type.EvenQ => this + EvenQDirectionVec(parity, direction),
            _ => Error
        };
        if (res.Equals(Error)) GD.PrintErr("OffsetCoords.Neighbor | 类型和方向不匹配");
        return res;
    }

    public AxialCoords ToAxial() =>
        Tp switch
        {
            Type.OddR => new AxialCoords(Col - (Row - (Row & 1)) / 2, Row),
            Type.EvenR => new AxialCoords(Col - (Row + (Row & 1)) / 2, Row),
            Type.OddQ => new AxialCoords(Col, Row - (Col - (Col & 1)) / 2),
            Type.EvenQ => new AxialCoords(Col, Row - (Col + (Col & 1)) / 2),
            _ => AxialCoords.Error
        };

    public Vector2I ToVec2I() => new(Col, Row);
}