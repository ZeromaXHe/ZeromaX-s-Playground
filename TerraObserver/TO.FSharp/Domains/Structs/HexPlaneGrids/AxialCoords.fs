namespace TO.FSharp.Domains.Structs.HexPlaneGrids

open System
open Godot

type PointyTopDirection =
    | Right = 0
    | RightUp = 1
    | LeftUp = 2
    | Left = 3
    | LeftDown = 4
    | RightDown = 5

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-16 19:59:16
[<Struct>]
type AxialCoords =
    val Q: int
    val R: int
    new(q, r) = { Q = q; R = r }
    override this.ToString() = $"{this.Q},{this.R}"
    static member From(v: Vector2I) = AxialCoords(v.X, v.Y)
    member this.ToVector2I() = Vector2I(this.Q, this.R)
    static member Error = AxialCoords(Int32.MinValue, Int32.MinValue)

    static let sqrt3 = Mathf.Sqrt(3f)

    static let dirVectors =
        [| AxialCoords(1, 0)
           AxialCoords(1, -1)
           AxialCoords(0, -1) // 右（右下），右上，左上（上）
           AxialCoords(-1, 0)
           AxialCoords(-1, 1)
           AxialCoords(0, 1) |] // 左（左上），左下，右下（下）

    static let diagonalVectors =
        [| AxialCoords(1, 1)
           AxialCoords(2, -1)
           AxialCoords(1, -2) // 右下（右），右上，上（左上）
           AxialCoords(-1, -1)
           AxialCoords(-2, 1)
           AxialCoords(-1, 2) |] // 左上（左），左下，下（右下）

    static member (+)(a1: AxialCoords, a2: AxialCoords) = AxialCoords(a1.Q + a2.Q, a1.R + a2.R)
    static member (-)(a1: AxialCoords, a2: AxialCoords) = AxialCoords(a1.Q - a2.Q, a1.R - a2.R)
    static member (*)(a: AxialCoords, scale: int) = AxialCoords(a.Q * scale, a.R * scale)
    static member (*)(scale: int, a: AxialCoords) = AxialCoords(a.Q * scale, a.R * scale)
    // 顺时针旋转 60 度（别用 Right 命名，容易误导圆弧下半部分的思考）
    member this.RotateClockwise() = AxialCoords(-this.R, this.Q + this.R)

    member this.RotateClockwiseAround(pivot: AxialCoords) =
        pivot + (this - pivot).RotateClockwise()
    // 逆时针旋转 60 度（别用 Left 命名，容易误导圆弧下半部分的思考）
    member this.RotateCounterClockwise() = AxialCoords(this.Q + this.R, -this.Q)

    member this.RotateCounterClockwiseAround(pivot: AxialCoords) =
        pivot + (this - pivot).RotateCounterClockwise()

    /// <summary>
    /// q 轴对称<br/>
    /// PointyUp 时，是中心格斜线 / 方向对角线的轴<br/>
    /// FlatUp 时，是中心格横线 — 方向对角线的轴<br/>
    /// <br/>
    /// 注：求垂直于 q 轴的轴对称点，可以 this.Scale(-1).ReflectQ()<br/>
    /// 求对于不过中心点的 q 轴的对称点，可以在轴上取一个参考点 pivot，
    /// 然后 pivot + (this - pivot).ReflectQ())
    /// </summary>
    /// <returns></returns>
    member this.ReflectQ() = AxialCoords(this.Q, -this.Q - this.R)
    /// <summary>
    /// r 轴对称<br/>
    /// PointyUp 时，是中心格竖线 | 方向对角线的轴<br/>
    /// FlatUp 时，是中心格斜线 / 方向对角线的轴
    /// </summary>
    /// <returns></returns>
    member this.ReflectR() = AxialCoords(-this.Q - this.R, this.R)
    /// <summary>
    /// s 轴对称<br/>
    /// PointyUp 时，是中心格反斜线 \ 方向对角线的轴<br/>
    /// FlatUp 时，是中心格反斜线 \ 方向对角线的轴
    /// </summary>
    /// <returns></returns>
    member this.ReflectS() = AxialCoords(this.R, this.Q)
    member this.Scale(factor: int) = factor * this
    /// 轴坐标系相邻
    member this.Neighbor(direction: int) = this + dirVectors[direction]
    /// 轴坐标系对角线相邻（相隔仅一格，角上连线出去的格子）
    member this.DiagonalNeighbor(direction: int) = this + diagonalVectors[direction]

    /// <summary>
    /// 距离计算<br/>
    /// <br/>
    /// There are lots of different ways to write hex distance in axial coordinates.<br/>
    /// No matter which way you write it, axial hex distance is derived from the Mahattan distance on cubes.<br/>
    /// For example, the “difference of differences” formula results
    /// from writing a.q + a.r - b.q - b.r as a.q - b.q + a.r - b.r,
    /// and using “max” form instead of the “divide by two” form of cube_distance.<br/>
    /// They're all equivalent once you see the connection to cube coordinates.<br/>
    /// 有很多不同的方法可以在轴坐标中写出六边形距离。无论你用哪种方式写，轴向六边形距离都是从立方体上的曼哈顿距离得出的。<br/>
    /// 例如，“差值之差”公式是将 a.q + a.r - b.q - b.r 写成 a.q - b.q + a.r - b.r，
    /// 并使用“max”形式而不是 cube_distance 的“除以 2”形式得出的。一旦您看到与立方体坐标的联系，它们都是等效的。
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    member this.DistanceTo(a: AxialCoords) =
        let diff = this - a
        (Mathf.Abs diff.Q + Mathf.Abs(diff.Q + diff.R) + Mathf.Abs diff.R) / 2

    // 因为浮点数的问题，所以用的是 Vector2 代表浮点化的 Axial
    member this.LerpToVec2(b: AxialCoords, t: float32) =
        Vector2(Mathf.Lerp(float32 this.Q, float32 b.Q, t), Mathf.Lerp(float32 this.R, float32 b.R, t))
    // 因为浮点数的问题，所以用的是 Vector2 代表浮点化的 Axial
    member this.RoundVec2(v: Vector2) =
        let mutable q = Mathf.RoundToInt v.X
        let mutable r = Mathf.RoundToInt v.Y
        let s = Mathf.RoundToInt(-v.X - v.Y)
        let qDiff = Mathf.Abs(float32 q - v.X)
        let rDiff = Mathf.Abs(float32 r - v.Y)
        let sDiff = Mathf.Abs(float32 s + v.X + v.Y)

        if qDiff > rDiff && qDiff > sDiff then
            q <- -r - s
        elif rDiff > sDiff then
            r <- -q - s

        AxialCoords(q, r)

    member this.LineDrawTo(c: AxialCoords) =
        let ax = this // F# 闭包不能传 this 这种 byref
        let n = this.DistanceTo c
        seq { for i in 0..n -> ax.RoundVec2 <| ax.LerpToVec2(c, 1f / float32 (n * i)) }

    member this.InRange(n: int) =
        let ax = this // F# 闭包不能传 this 这种 byref

        seq {
            for q in -n .. n do
                for r in Mathf.Max(-n, -q - n) .. Mathf.Min(n, -q + n) do
                    ax + AxialCoords(q, r)
        }

    member this.IntersectingRange(a: AxialCoords, n: int) =
        let qMin = Mathf.Max(this.Q, a.Q) - n
        let qMax = Mathf.Min(this.Q, a.Q) + n
        let rMin = Mathf.Max(this.R, a.R) - n
        let rMax = Mathf.Min(this.R, a.R) + n
        let sMin = Mathf.Max(-this.Q - this.R, -a.Q - a.R) - n
        let sMax = Mathf.Min(-this.Q - this.R, -a.Q - a.R) + n

        seq {
            for q in qMin..qMax do
                for r in Mathf.Max(rMin, -q - sMax) .. Mathf.Min(rMax, -q - sMin) do
                    AxialCoords(q, r)
        }

    // 环
    member this.Ring(radius: int) =
        if radius = 0 then // F# 闭包不能传 this 这种 byref
            let ax = this
            seq { ax }
        else
            let mutable axial = this + dirVectors[int PointyTopDirection.LeftDown].Scale radius

            seq {
                for i in 0..5 do
                    for j in 0 .. radius - 1 do
                        yield axial
                        axial <- axial.Neighbor i
            }
    // 螺旋
    member this.Spiral(radius: int) =
        let ring = this.Ring

        seq {
            for i in 0..radius do
                for axial in ring i do
                    axial
        }
    // 顶点朝上的六边形中心像素位置（假定 (0,0) 六边形的中心在原点 (0,0)）
    member this.PointyTopCenter(?size: float32) =
        let size = defaultArg size 1f

        Vector2(sqrt3 * float32 this.Q + sqrt3 * 0.5f * float32 this.R, 1.5f * float32 this.R)
        * size
    // 平边朝上的六边形中心像素位置（假定 (0,0) 六边形的中心在原点 (0,0)）
    member this.FlatTopCenter(?size: float32) =
        let size = defaultArg size 1f

        Vector2(1.5f * float32 this.Q, sqrt3 * 0.5f * float32 this.Q + sqrt3 * float32 this.R)
        * size
    // 像素位置转为顶点朝上的六边形坐标
    member this.FromPointyTopPixel(point: Vector2, ?size: float32) =
        let size = defaultArg size 1f
        this.RoundVec2(Vector2(sqrt3 / 3f * point.X - point.Y / 3f, 2f / 3f * point.Y) / size)
    // 像素位置转为平边朝上的六边形坐标
    member this.FromFlatTopPixel(point: Vector2, ?size: float32) =
        let size = defaultArg size 1f
        this.RoundVec2(Vector2(2f / 3f * point.X, -point.X / 3f + sqrt3 / 3f * point.Y) / size)
