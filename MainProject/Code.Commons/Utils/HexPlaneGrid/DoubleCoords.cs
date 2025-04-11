using Godot;

namespace Commons.Utils.HexPlaneGrid;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 18:02
public struct DoubleCoords
{
    public enum Type
    {
        All,
        Height,
        Width,
        Error
    }

    public readonly Type Tp;
    public readonly int Col;
    public readonly int Row;

    private DoubleCoords(int col, int row, Type type)
    {
        Col = col;
        Row = row;
        Tp = type;
    }

    public DoubleCoords(int col, int row) : this(col, row, Type.All)
    {
    }

    public static readonly DoubleCoords Error = new(int.MinValue, int.MinValue, Type.Error);
    public static DoubleCoords Height(int col, int row) => new(col, row, Type.Height);
    public static DoubleCoords Width(int col, int row) => new(col, row, Type.Width);
    public static DoubleCoords Height(AxialCoords a) => Height(a.Q, 2 * a.R + a.Q);
    public static DoubleCoords Width(AxialCoords a) => Width(2 * a.Q + a.R, a.R);

    private static readonly DoubleCoords[] WidthDirVectors =
    [
        Width(2, 0), Width(1, -1), Width(-1, -1), // 右，右上，左上
        Width(-2, 0), Width(-1, 1), Width(1, 1), // 左，左下，右下
    ];

    private static readonly DoubleCoords[] HeightDirVectors =
    [
        Height(1, 1), Height(1, -1), Height(0, -2), // 右下，右上，上
        Height(-1, -1), Height(-1, 1), Height(0, 2), // 左上，左下，下
    ];

    private static DoubleCoords WidthDirVector(PointyTopDirection direction) => WidthDirVectors[(int)direction];
    private static DoubleCoords HeightDirVector(FlatTopDirection direction) => HeightDirVectors[(int)direction];

    public static DoubleCoords operator +(DoubleCoords o1, DoubleCoords o2)
    {
        if (o1.Tp == Type.Error || o2.Tp == Type.Error ||
            (o1.Tp != o2.Tp && o1.Tp != Type.All && o2.Tp != Type.All))
        {
            GD.PrintErr("OffsetCoords + | 相加类型错误");
            return Error;
        }

        var resType = o1.Tp != Type.All ? o1.Tp : o2.Tp;
        return new DoubleCoords(o1.Col + o2.Col, o1.Row + o2.Row, resType);
    }

    public static DoubleCoords operator -(DoubleCoords o1, DoubleCoords o2)
    {
        if (o1.Tp == Type.Error || o2.Tp == Type.Error ||
            (o1.Tp != o2.Tp && o1.Tp != Type.All && o2.Tp != Type.All))
        {
            GD.PrintErr("OffsetCoords + | 相加类型错误");
            return Error;
        }

        var resType = o1.Tp != Type.All ? o1.Tp : o2.Tp;
        return new DoubleCoords(o1.Col - o2.Col, o1.Row - o2.Row, resType);
    }

    public int DistanceTo(DoubleCoords d)
    {
        var diff = this - d;
        if (Error.Equals(diff) || Tp == Type.All)
            return -1;
        if (Tp == Type.Width)
            return diff.Row + Mathf.Max(0, (diff.Col - diff.Row) / 2);
        return diff.Col + Mathf.Max(0, (diff.Row - diff.Col) / 2);
    }

    public DoubleCoords Neighbor(PointyTopDirection direction)
    {
        if (Tp == Type.Width)
            return this + WidthDirVector(direction);
        GD.PrintErr("DoubleCoords.Neighbor | 类型和方向不匹配");
        return Error;
    }

    public DoubleCoords Neighbor(FlatTopDirection direction)
    {
        if (Tp == Type.Height)
            return this + HeightDirVector(direction);
        GD.PrintErr("DoubleCoords.Neighbor | 类型和方向不匹配");
        return Error;
    }

    public AxialCoords ToAxial() =>
        Tp switch
        {
            Type.Width => new AxialCoords(Col, (Row - Col) / 2),
            Type.Height => new AxialCoords((Col - Row) / 2, Row),
            _ => AxialCoords.Error
        };
}