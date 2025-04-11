using Godot;

namespace Commons.Utils.HexPlaneGrid;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 18:02
public readonly struct CubeCoords(int q, int r, int s)
{
    public readonly int Q = q;
    public readonly int R = r;
    public readonly int S = s;

    private static readonly CubeCoords[] DirectionVectors =
    [
        new CubeCoords(1, 0, -1), new CubeCoords(1, -1, 0), new CubeCoords(0, -1, 1), // 右（右下），右上，左上（上）
        new CubeCoords(-1, 0, 1), new CubeCoords(-1, 1, 0), new CubeCoords(0, 1, -1) // 左（左上），左下，右下（下）
    ];

    private static readonly CubeCoords[] DiagonalVectors =
    [
        new CubeCoords(1, 1, -2), new CubeCoords(2, -1, -1), new CubeCoords(1, -2, 1), // 右下（右），右上，上（左上）
        new CubeCoords(-1, -1, 2), new CubeCoords(-2, 1, 1), new CubeCoords(-1, 2, -1) // 左上（左），左下，下（右下）
    ];

    public static CubeCoords operator +(CubeCoords c1, CubeCoords c2) => new(c1.Q + c2.Q, c1.R + c2.R, c1.S + c2.S);
    public static CubeCoords operator -(CubeCoords c1, CubeCoords c2) => new(c1.Q - c2.Q, c1.R - c2.R, c1.S - c2.S);
    public static CubeCoords operator *(int factor, CubeCoords c) => new(c.Q * factor, c.R * factor, c.S * factor);
    public static CubeCoords operator *(CubeCoords c, int factor) => new(c.Q * factor, c.R * factor, c.S * factor);

    // 顺时针旋转
    public CubeCoords RotateRight() => new(-R, -S, -Q);

    public CubeCoords RotateRightAround(CubeCoords pivot)
    {
        var vec = this - pivot;
        return pivot + vec.RotateRight();
    }

    // 逆时针旋转
    public CubeCoords RotateLeft() => new(-S, -Q, -R);

    public CubeCoords RotateLeftAround(CubeCoords pivot)
    {
        var vec = this - pivot;
        return pivot + vec.RotateLeft();
    }

    /// <summary>
    /// q 轴对称
    /// PointyUp 时，是中心格斜线 / 方向对角线的轴
    /// FlatUp 时，是中心格横线 — 方向对角线的轴
    /// </summary>
    /// <returns></returns>
    public CubeCoords ReflectQ() => new(Q, S, R);

    /// <summary>
    /// r 轴对称
    /// PointyUp 时，是中心格竖线 | 方向对角线的轴
    /// FlatUp 时，是中心格斜线 / 方向对角线的轴
    /// </summary>
    /// <returns></returns>
    public CubeCoords ReflectR() => new(S, R, Q);

    /// <summary>
    /// s 轴对称
    /// PointyUp 时，是中心格反斜线 \ 方向对角线的轴
    /// FlatUp 时，是中心格反斜线 \ 方向对角线的轴
    /// </summary>
    /// <returns></returns>
    public CubeCoords ReflectS() => new(R, Q, S);

    public CubeCoords Scale(int factor) => this * factor;

    public int DistanceTo(CubeCoords c)
    {
        var diff = this - c;
        return (Mathf.Abs(diff.Q) + Mathf.Abs(diff.R) + Mathf.Abs(diff.S)) / 2;
        // 等效：Max(Mathf.Abs(diff.Q), Mathf.Abs(diff.R), Mathf.Abs(diff.S));
    }

    // 因为浮点数的问题，所以用的是 Vector3 代表浮点化的 Cube
    public Vector3 LerpToVec3(CubeCoords b, float t) =>
        new(Mathf.Lerp(Q, b.Q, t), Mathf.Lerp(R, b.R, t), Mathf.Lerp(S, b.S, t));

    // 因为浮点数的问题，所以用的是 Vector3 代表浮点化的 Cube
    public CubeCoords RoundVec3(Vector3 v)
    {
        var q = Mathf.RoundToInt(v.X);
        var r = Mathf.RoundToInt(v.Y);
        var s = Mathf.RoundToInt(v.Z);
        var qDiff = Mathf.Abs(q - v.X);
        var rDiff = Mathf.Abs(r - v.Y);
        var sDiff = Mathf.Abs(s - v.Z);
        if (qDiff > rDiff && qDiff > sDiff)
            q = -r - s;
        else if (rDiff > sDiff)
            r = -q - s;
        else
            s = -q - r;
        return new CubeCoords(q, r, s);
    }

    public IEnumerable<CubeCoords> LineDrawTo(CubeCoords c)
    {
        var n = DistanceTo(c);
        for (var i = 0; i < n + 1; i++)
            yield return RoundVec3(LerpToVec3(c, 1f / n * i));
    }

    public IEnumerable<CubeCoords> InRange(int n)
    {
        for (var q = -n; q <= n; q++)
        for (var r = Mathf.Max(-n, -q - n); r <= Mathf.Min(n, -q + n); r++)
            yield return this + new CubeCoords(q, r, -q - r);
    }

    public IEnumerable<CubeCoords> IntersectingRange(CubeCoords c, int n)
    {
        var qMin = Mathf.Max(Q, c.Q) - n;
        var qMax = Mathf.Min(Q, c.Q) + n;
        var rMin = Mathf.Max(R, c.R) - n;
        var rMax = Mathf.Min(R, c.R) + n;
        var sMin = Mathf.Max(S, c.S) - n;
        var sMax = Mathf.Min(S, c.S) + n;
        for (var q = qMin; q <= qMax; q++)
        for (var r = Mathf.Max(rMin, -q - sMax); r <= Mathf.Min(rMax, -q - sMin); r++)
            yield return new CubeCoords(q, r, -q - r);
    }

    // 环
    public IEnumerable<CubeCoords> Ring(int radius)
    {
        if (radius == 0)
        {
            yield return this;
            yield break;
        }

        var cube = this + DirectionVectors[(int)PointyTopDirection.LeftDown].Scale(radius);
        for (var i = 0; i < 6; i++)
        for (var j = 0; j < radius; j++)
        {
            yield return cube;
            cube = cube.Neighbor(i);
        }
    }

    // 螺旋
    public IEnumerable<CubeCoords> Spiral(int radius)
    {
        for (var i = 0; i < radius + 1; i++)
            foreach (var cube in Ring(i))
                yield return cube;
    }

    // 立方体坐标系相邻
    public CubeCoords Neighbor(int direction) => this + DirectionVectors[direction];

    // 立方体坐标系对角线相邻（相隔仅一格，角上连线出去的格子）
    public CubeCoords DiagonalNeighbor(int direction) => this + DiagonalVectors[direction];

    // 轴坐标
    public AxialCoords ToAxial() => new(Q, R);
    public static CubeCoords FromAxial(AxialCoords a) => new(a.Q, a.R, -a.Q - a.R);
}