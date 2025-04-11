using Godot;

namespace Commons.Utils.HexPlaneGrid;

/// <summary>
/// 轴坐标系（对应 Godot TileMap 里面的 Stairs Right）
/// S = -Q - R
/// 
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 18:02
/// </summary>
/// <param name="q"></param>
/// <param name="r"></param>
public readonly struct AxialCoords(int q, int r)
{
    public readonly int Q = q;
    public readonly int R = r;
    public override string ToString() => $"({Q}, {R})";
    public static AxialCoords From(Vector2I v) => new(v.X, v.Y);
    public Vector2I ToVector2I() => new(Q, R);
    public static readonly AxialCoords Error = new(int.MinValue, int.MinValue);

    private static readonly AxialCoords[] DirVectors =
    [
        new AxialCoords(1, 0), new AxialCoords(1, -1), new AxialCoords(0, -1), // 右（右下），右上，左上（上）
        new AxialCoords(-1, 0), new AxialCoords(-1, 1), new AxialCoords(0, 1), // 左（左上），左下，右下（下）
    ];

    private static readonly AxialCoords[] DiagonalVectors =
    [
        new AxialCoords(1, 1), new AxialCoords(2, -1), new AxialCoords(1, -2), // 右下（右），右上，上（左上）
        new AxialCoords(-1, -1), new AxialCoords(-2, 1), new AxialCoords(-1, 2) // 左上（左），左下，下（右下）
    ];

    public static AxialCoords operator +(AxialCoords a1, AxialCoords a2) => new(a1.Q + a2.Q, a1.R + a2.R);

    public static AxialCoords operator -(AxialCoords a1, AxialCoords a2) => new(a1.Q - a2.Q, a1.R - a2.R);
    public static AxialCoords operator *(AxialCoords a, int scale) => new(a.Q * scale, a.R * scale);
    public static AxialCoords operator *(int scale, AxialCoords a) => new(a.Q * scale, a.R * scale);

    // 顺时针旋转 60 度（别用 Right 命名，容易误导圆弧下半部分的思考）
    public AxialCoords RotateClockwise() => new(-R, Q + R);

    public AxialCoords RotateClockwiseAround(AxialCoords pivot)
    {
        var vec = this - pivot;
        return pivot + vec.RotateClockwise();
    }

    // 逆时针旋转 60 度（别用 Left 命名，容易误导圆弧下半部分的思考）
    public AxialCoords RotateCounterClockwise() => new(Q + R, -Q);

    public AxialCoords RotateCounterClockwiseAround(AxialCoords pivot)
    {
        var vec = this - pivot;
        return pivot + vec.RotateCounterClockwise();
    }

    /// <summary>
    /// q 轴对称
    /// PointyUp 时，是中心格斜线 / 方向对角线的轴
    /// FlatUp 时，是中心格横线 — 方向对角线的轴
    /// 
    /// 注：求垂直于 q 轴的轴对称点，可以 this.Scale(-1).ReflectQ()
    /// 求对于不过中心点的 q 轴的对称点，可以在轴上取一个参考点 pivot，
    /// 然后 pivot + (this - pivot).ReflectQ())
    /// </summary>
    /// <returns></returns>
    public AxialCoords ReflectQ() => new(Q, -Q - R);

    /// <summary>
    /// r 轴对称
    /// PointyUp 时，是中心格竖线 | 方向对角线的轴
    /// FlatUp 时，是中心格斜线 / 方向对角线的轴
    /// </summary>
    /// <returns></returns>
    public AxialCoords ReflectR() => new(-Q - R, R);

    /// <summary>
    /// s 轴对称
    /// PointyUp 时，是中心格反斜线 \ 方向对角线的轴
    /// FlatUp 时，是中心格反斜线 \ 方向对角线的轴
    /// </summary>
    /// <returns></returns>
    public AxialCoords ReflectS() => new(R, Q);

    public AxialCoords Scale(int factor) => factor * this;

    // 轴坐标系相邻
    public AxialCoords Neighbor(int direction) => this + DirVectors[direction];

    // 轴坐标系对角线相邻（相隔仅一格，角上连线出去的格子）
    public AxialCoords DiagonalNeighbor(int direction) => this + DiagonalVectors[direction];

    /// <summary>
    /// 距离计算
    /// 
    /// There are lots of different ways to write hex distance in axial coordinates.
    /// No matter which way you write it, axial hex distance is derived from the Mahattan distance on cubes.
    /// For example, the “difference of differences” formula results
    /// from writing a.q + a.r - b.q - b.r as a.q - b.q + a.r - b.r,
    /// and using “max” form instead of the “divide by two” form of cube_distance.
    /// They're all equivalent once you see the connection to cube coordinates.
    /// 有很多不同的方法可以在轴坐标中写出六边形距离。无论你用哪种方式写，轴向六边形距离都是从立方体上的曼哈顿距离得出的。
    /// 例如，“差值之差”公式是将 a.q + a.r - b.q - b.r 写成 a.q - b.q + a.r - b.r，
    /// 并使用“max”形式而不是 cube_distance 的“除以 2”形式得出的。一旦您看到与立方体坐标的联系，它们都是等效的。
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public int DistanceTo(AxialCoords a)
    {
        var diff = this - a;
        return (Mathf.Abs(diff.Q) + Mathf.Abs(diff.Q + diff.R) + Mathf.Abs(diff.R)) / 2;
    }

    // 因为浮点数的问题，所以用的是 Vector2 代表浮点化的 Axial
    public Vector2 LerpToVec2(AxialCoords b, float t) =>
        new(Mathf.Lerp(Q, b.Q, t), Mathf.Lerp(R, b.R, t));

    // 因为浮点数的问题，所以用的是 Vector2 代表浮点化的 Axial
    public AxialCoords RoundVec2(Vector2 v)
    {
        var q = Mathf.RoundToInt(v.X);
        var r = Mathf.RoundToInt(v.Y);
        var s = Mathf.RoundToInt(-v.X - v.Y);
        var qDiff = Mathf.Abs(q - v.X);
        var rDiff = Mathf.Abs(r - v.Y);
        var sDiff = Mathf.Abs(s + v.X + v.Y);
        if (qDiff > rDiff && qDiff > sDiff)
            q = -r - s;
        else if (rDiff > sDiff)
            r = -q - s;
        return new AxialCoords(q, r);
    }

    public IEnumerable<AxialCoords> LineDrawTo(AxialCoords c)
    {
        var n = DistanceTo(c);
        for (var i = 0; i < n + 1; i++)
            yield return RoundVec2(LerpToVec2(c, 1f / n * i));
    }

    public IEnumerable<AxialCoords> InRange(int n)
    {
        for (var q = -n; q <= n; q++)
        for (var r = Mathf.Max(-n, -q - n); r <= Mathf.Min(n, -q + n); r++)
            yield return this + new AxialCoords(q, r);
    }

    public IEnumerable<AxialCoords> IntersectingRange(AxialCoords a, int n)
    {
        var qMin = Mathf.Max(Q, a.Q) - n;
        var qMax = Mathf.Min(Q, a.Q) + n;
        var rMin = Mathf.Max(R, a.R) - n;
        var rMax = Mathf.Min(R, a.R) + n;
        var sMin = Mathf.Max(-Q - R, -a.Q - a.R) - n;
        var sMax = Mathf.Min(-Q - R, -a.Q - a.R) + n;
        for (var q = qMin; q <= qMax; q++)
        for (var r = Mathf.Max(rMin, -q - sMax); r <= Mathf.Min(rMax, -q - sMin); r++)
            yield return new AxialCoords(q, r);
    }

    // 环
    public IEnumerable<AxialCoords> Ring(int radius)
    {
        if (radius == 0)
        {
            yield return this;
            yield break;
        }

        var axial = this + DirVectors[(int)PointyTopDirection.LeftDown].Scale(radius);
        for (var i = 0; i < 6; i++)
        for (var j = 0; j < radius; j++)
        {
            yield return axial;
            axial = axial.Neighbor(i);
        }
    }

    // 螺旋
    public IEnumerable<AxialCoords> Spiral(int radius)
    {
        for (var i = 0; i < radius + 1; i++)
            foreach (var axial in Ring(i))
                yield return axial;
    }

    private static readonly float Sqrt3 = Mathf.Sqrt(3);

    // 顶点朝上的六边形中心像素位置（假定 (0,0) 六边形的中心在原点 (0,0)）
    public Vector2 PointyTopCenter(float size = 1f) => new Vector2(Sqrt3 * Q + Sqrt3 * 0.5f * R, 1.5f * R) * size;

    // 平边朝上的六边形中心像素位置（假定 (0,0) 六边形的中心在原点 (0,0)）
    public Vector2 FlatTopCenter(float size = 1f) => new Vector2(1.5f * Q, Sqrt3 * 0.5f * Q + Sqrt3 * R) * size;

    // 像素位置转为顶点朝上的六边形坐标
    public AxialCoords FromPointyTopPixel(Vector2 point, float size = 1f) =>
        RoundVec2(new Vector2(Sqrt3 / 3f * point.X - point.Y / 3f, 2f / 3f * point.Y) / size);

    // 像素位置转为平边朝上的六边形坐标
    public AxialCoords FromFlatTopPixel(Vector2 point, float size = 1f) =>
        RoundVec2(new Vector2(2f / 3f * point.X, -point.X / 3f + Sqrt3 / 3f * point.Y) / size);
}