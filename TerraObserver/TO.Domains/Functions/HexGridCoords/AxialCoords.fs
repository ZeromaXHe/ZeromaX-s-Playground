namespace TO.Domains.Functions.HexGridCoords

open System
open Godot
open TO.Domains.Functions.Maths
open TO.Domains.Types.HexGridCoords

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:02:29
module AxialCoords =
    let toString (a: AxialCoords) = $"{a.Q},{a.R}"
    let from (v: Vector2I) = AxialCoords(v.X, v.Y)
    let toVector2I (a: AxialCoords) = Vector2I(a.Q, a.R)
    let error = AxialCoords(Int32.MinValue, Int32.MinValue)

    let dirVectors =
        [| AxialCoords(1, 0)
           AxialCoords(1, -1)
           AxialCoords(0, -1) // 右（右下），右上，左上（上）
           AxialCoords(-1, 0)
           AxialCoords(-1, 1)
           AxialCoords(0, 1) |] // 左（左上），左下，右下（下）

    let diagonalVectors =
        [| AxialCoords(1, 1)
           AxialCoords(2, -1)
           AxialCoords(1, -2) // 右下（右），右上，上（左上）
           AxialCoords(-1, -1)
           AxialCoords(-2, 1)
           AxialCoords(-1, 2) |] // 左上（左），左下，下（右下）

    let add (a2: AxialCoords) (a1: AxialCoords) = AxialCoords(a1.Q + a2.Q, a1.R + a2.R)
    let minus (a2: AxialCoords) (a1: AxialCoords) = AxialCoords(a1.Q - a2.Q, a1.R - a2.R)
    let times (scale: int) (a: AxialCoords) = AxialCoords(a.Q * scale, a.R * scale)
    // 顺时针旋转 60 度（别用 Right 命名，容易误导圆弧下半部分的思考）
    let rotateClockwise (a: AxialCoords) = AxialCoords(-a.R, a.Q + a.R)

    let rotateClockwiseAround pivot (a: AxialCoords) =
        a |> minus pivot |> rotateClockwise |> add pivot
    // 逆时针旋转 60 度（别用 Left 命名，容易误导圆弧下半部分的思考）
    let rotateCounterClockwise (a: AxialCoords) = AxialCoords(a.Q + a.R, -a.Q)

    let rotateCounterClockwiseAround pivot (a: AxialCoords) =
        a |> minus pivot |> rotateCounterClockwise |> add pivot

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
    let reflectQ (a: AxialCoords) = AxialCoords(a.Q, -a.Q - a.R)
    /// <summary>
    /// r 轴对称<br/>
    /// PointyUp 时，是中心格竖线 | 方向对角线的轴<br/>
    /// FlatUp 时，是中心格斜线 / 方向对角线的轴
    /// </summary>
    /// <returns></returns>
    let reflectR (a: AxialCoords) = AxialCoords(-a.Q - a.R, a.R)
    /// <summary>
    /// s 轴对称<br/>
    /// PointyUp 时，是中心格反斜线 \ 方向对角线的轴<br/>
    /// FlatUp 时，是中心格反斜线 \ 方向对角线的轴
    /// </summary>
    /// <returns></returns>
    let reflectS (a: AxialCoords) = AxialCoords(a.R, a.Q)
    /// 轴坐标系相邻
    let neighbor (direction: int) (a: AxialCoords) = a |> add dirVectors[direction]
    /// 轴坐标系对角线相邻（相隔仅一格，角上连线出去的格子）
    let diagonalNeighbor (direction: int) (a: AxialCoords) = a |> add diagonalVectors[direction]

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
    /// <param name="aTo"></param>
    /// <param name="aFrom"></param>
    /// <returns></returns>
    let distanceTo (aTo: AxialCoords) (aFrom: AxialCoords) =
        let diff = aFrom |> minus aTo
        (Mathf.Abs diff.Q + Mathf.Abs(diff.Q + diff.R) + Mathf.Abs diff.R) / 2
    // 因为浮点数的问题，所以用的是 Vector2 代表浮点化的 Axial
    let private lerpToVec2 (b: AxialCoords) (t: float32) (a: AxialCoords) =
        Vector2(Mathf.Lerp(float32 a.Q, float32 b.Q, t), Mathf.Lerp(float32 a.R, float32 b.R, t))
    // 因为浮点数的问题，所以用的是 Vector2 代表浮点化的 Axial
    let private roundVec2 (v: Vector2) =
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

    let lineDrawTo (aTo: AxialCoords) (aFrom: AxialCoords) =
        let dist = aFrom |> distanceTo aTo
        seq { for i in 0..dist -> aFrom |> lerpToVec2 aTo (1f / float32 (dist * i)) |> roundVec2 }

    let inRange (n: int) (a: AxialCoords) =
        seq {
            for q in -n .. n do
                for r in Mathf.Max(-n, -q - n) .. Mathf.Min(n, -q + n) do
                    a |> add <| AxialCoords(q, r)
        }

    let intersectingRange (a2: AxialCoords) (n: int) (a1: AxialCoords) =
        let qMin = Mathf.Max(a1.Q, a2.Q) - n
        let qMax = Mathf.Min(a1.Q, a2.Q) + n
        let rMin = Mathf.Max(a1.R, a2.R) - n
        let rMax = Mathf.Min(a1.R, a2.R) + n
        let sMin = Mathf.Max(-a1.Q - a1.R, -a2.Q - a2.R) - n
        let sMax = Mathf.Min(-a1.Q - a1.R, -a2.Q - a2.R) + n

        seq {
            for q in qMin..qMax do
                for r in Mathf.Max(rMin, -q - sMax) .. Mathf.Min(rMax, -q - sMin) do
                    AxialCoords(q, r)
        }
    // 环
    let ring (radius: int) (a: AxialCoords) =
        if radius = 0 then
            seq { a }
        else
            let mutable axial =
                a |> add <| dirVectors[int PointyTopDirection.LeftDown] |> times radius

            seq {
                for i in 0..5 do
                    for _ in 0 .. radius - 1 do
                        yield axial
                        axial <- axial |> neighbor i
            }
    // 螺旋
    let spiral (radius: int) (a: AxialCoords) =
        seq {
            for i in 0..radius do
                for axial in ring i a do
                    axial
        }
    // 顶点朝上的六边形中心像素位置（假定 (0,0) 六边形的中心在原点 (0,0)）
    let pointyTopCenter (size: float32) (a: AxialCoords) =
        Vector2(MathConstant.sqrt3 * float32 a.Q + MathConstant.sqrt3 * 0.5f * float32 a.R, 1.5f * float32 a.R)
        * size

    // 平边朝上的六边形中心像素位置（假定 (0,0) 六边形的中心在原点 (0,0)）
    let flatTopCenter (size: float32) (a: AxialCoords) =
        Vector2(1.5f * float32 a.Q, MathConstant.sqrt3 * 0.5f * float32 a.Q + MathConstant.sqrt3 * float32 a.R)
        * size
    // 像素位置转为顶点朝上的六边形坐标
    let fromPointyTopPixel (point: Vector2) (size: float32) =
        roundVec2
        <| Vector2(MathConstant.sqrt3 / 3f * point.X - point.Y / 3f, 2f / 3f * point.Y)
           / size
    // 像素位置转为平边朝上的六边形坐标
    let fromFlatTopPixel (point: Vector2) (size: float32) =
        roundVec2
        <| Vector2(2f / 3f * point.X, -point.X / 3f + MathConstant.sqrt3 / 3f * point.Y)
           / size
