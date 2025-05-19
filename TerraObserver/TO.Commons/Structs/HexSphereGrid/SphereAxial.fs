namespace TO.Commons.Structs.HexSphereGrid

open Godot
open TO.Commons.Structs.HexPlaneGrid

type TypeEnum =
    // 正二十面体南北极顶点。
    // 索引：0 北极点，1 南极点
    | PoleVertices = 0
    // 正二十面体中间的十个顶点。
    // 索引：0、1 第一组竖向四面的中间右侧从北到南两点，2、3 第二组，以此类推，8、9 第五组（最后一组）
    | MidVertices = 1
    // 正二十面体 边上的普通点（可以简单用六边形坐标搜索邻居）
    // 索引：0 ~ 5 第一组竖向四面的从北到南六边（左侧三边不算），6 ~ 11 第二组，以此类推，24 ~ 29 第五组（最后一组）
    | Edges = 2
    // 正二十面体 边上的特殊点
    // 索引：0 ~ 5 第一组竖向四面的从北到南六边（左侧三边不算），6 ~ 11 第二组，以此类推，24 ~ 29 第五组（最后一组）
    // 索引 % 6 == 0 || 5（第一边和最后一边）时，相邻的面索引是当前面索引 - 4
    // 其它情况说明在南北回归线西边与 MidVertices 相邻
    | EdgesSpecial = 3
    // 正二十面体 面上的普通点（可以简单用六边形坐标搜索邻居）
    // 索引：0 ~ 3 第一组竖向四面从北到南，4 ~ 7 第二组，以此类推，16 ~ 19 第五组（最后一组）
    | Faces = 4
    // 正二十面体 面上的特殊点（需要用特殊规则搜索邻居
    // 索引：0 ~ 3 第一组竖向四面从北到南，4 ~ 7 第二组，以此类推，16 ~ 19 第五组（最后一组）
    // 相邻的面索引 + 4 即可
    | FacesSpecial = 5
    // 无效坐标
    | Invalid = 6

/// 六边形球面轴坐标
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-16 19:37:16
[<Struct>]
type SphereAxial =
    val Coords: AxialCoords
    val Type: TypeEnum
    val TypeIdx: int

    static let validateAxial (q: int, r: int) =
        if q > 0 || q <= -SphereAxial.Width then
            // q 在 (-Width, 0] 之间
            false
        elif r < -SphereAxial.Div || r > 2 * SphereAxial.Div then
            // r 在 (-ColWidth, 2 * ColWidth) 之间)
            false
        elif r = -SphereAxial.Div then
            // 北极点
            q = 0
        elif r = 2 * SphereAxial.Div then
            // 南极点
            q = -SphereAxial.Div
        elif r < 0 then
            -q % SphereAxial.Div < SphereAxial.Div + r
        elif r > SphereAxial.Div then
            -q % SphereAxial.Div > SphereAxial.Div - r
        else
            true
    // static let mutable div = 5
    // static member Div with get() = div and set value = div <- value
    static member val Div = 5 with get, set
    static member Width = SphereAxial.Div * 5

    new(q: int, r: int) =
        let coords = AxialCoords(q, r)

        if not <| validateAxial (q, r) then
            { Coords = coords
              Type = TypeEnum.Invalid
              TypeIdx = -1 }
        elif r = -SphereAxial.Div || r = 2 * SphereAxial.Div then
            { Coords = coords
              Type = TypeEnum.PoleVertices
              TypeIdx = if r = -SphereAxial.Div then 0 else 1 }
        elif r < 0 then
            if -q % SphereAxial.Div = 0 then
                { Coords = coords
                  Type = TypeEnum.EdgesSpecial
                  TypeIdx = -q / SphereAxial.Div * 6 }
            else
                { Coords = coords
                  Type =
                    if -q % SphereAxial.Div = SphereAxial.Div + r - 1 then
                        TypeEnum.FacesSpecial
                    else
                        TypeEnum.Faces
                  TypeIdx = -q / SphereAxial.Div * 4 }
        elif r = 0 then
            if -q % SphereAxial.Div = 0 then
                { Coords = coords
                  Type = TypeEnum.MidVertices
                  TypeIdx = -q / SphereAxial.Div * 2 }
            else
                { Coords = coords
                  Type =
                    if -q % SphereAxial.Div = SphereAxial.Div - 1 then
                        TypeEnum.EdgesSpecial
                    else
                        TypeEnum.Edges
                  TypeIdx = -q / SphereAxial.Div * 6 + 1 }
        elif r < SphereAxial.Div then
            if -q % SphereAxial.Div = 0 then
                { Coords = coords
                  Type = TypeEnum.Edges
                  TypeIdx = -q / SphereAxial.Div * 6 + 3 }
            elif -q % SphereAxial.Div = r then
                { Coords = coords
                  Type = TypeEnum.Edges
                  TypeIdx = -q / SphereAxial.Div * 6 + 2 }
            else
                { Coords = coords
                  Type = TypeEnum.Faces
                  TypeIdx = -q / SphereAxial.Div * 4 + if -q % SphereAxial.Div > r then 1 else 2 }
        elif r = SphereAxial.Div then
            if -q % SphereAxial.Div = 0 then
                { Coords = coords
                  Type = TypeEnum.MidVertices
                  TypeIdx = -q / SphereAxial.Div * 2 + 1 }
            else
                { Coords = coords
                  Type =
                    if -q % SphereAxial.Div = SphereAxial.Div - 1 then
                        TypeEnum.EdgesSpecial
                    else
                        TypeEnum.Edges
                  TypeIdx = -q / SphereAxial.Div * 6 + 4 }
        elif -q % SphereAxial.Div = r - SphereAxial.Div then
            { Coords = coords
              Type = TypeEnum.EdgesSpecial
              TypeIdx = -q / SphereAxial.Div * 6 + 5 }
        else
            { Coords = coords
              Type =
                if -q % SphereAxial.Div = SphereAxial.Div - 1 then
                    TypeEnum.FacesSpecial
                else
                    TypeEnum.Faces
              TypeIdx = -q / SphereAxial.Div * 4 + 3 }

    override this.ToString() =
        $"{this.Coords} {this.Type} {this.TypeIdx}"

    member this.SpecialNeighbor =
        this.Type = TypeEnum.EdgesSpecial || this.Type = TypeEnum.FacesSpecial
    // 正二十面体索引，0 ~ 19
    member this.Index =
        match this.Type with
        | TypeEnum.PoleVertices -> if this.TypeIdx = 0 then 0 else 3
        | TypeEnum.MidVertices -> this.TypeIdx / 2 * 4 + 1 + this.TypeIdx % 2
        | TypeEnum.Edges
        | TypeEnum.EdgesSpecial -> this.TypeIdx / 6 * 4 + (this.TypeIdx % 6 + 1) / 2
        | TypeEnum.Faces
        | TypeEnum.FacesSpecial -> this.TypeIdx / 4 * 4 + this.TypeIdx % 4
        | _ -> -1
    // 获取列索引，从右到左 0 ~ 4
    member this.Column =
        if this.Type = TypeEnum.PoleVertices && this.TypeIdx = 1 then
            0
        elif this.Type <> TypeEnum.Invalid then
            -this.Coords.Q / SphereAxial.Div
        else
            -1
    // 获取行索引，从上到下 0 ~ 3
    member this.Row =
        match this.Type with
        | TypeEnum.PoleVertices -> if this.TypeIdx = 0 then 0 else 3
        | TypeEnum.MidVertices -> 1 + this.TypeIdx % 2
        | TypeEnum.Edges
        | TypeEnum.EdgesSpecial -> (this.TypeIdx % 6 + 1) / 2
        | TypeEnum.Faces
        | TypeEnum.FacesSpecial -> this.TypeIdx % 4
        | _ -> -1

    // 在北边的 5 个面上
    member this.IsNorth5 = this.Row = 0
    // 在南边的 5 个面上
    member this.IsSouth5 = this.Row = 3
    // 属于极地十面
    member this.IsPole10 = this.IsNorth5 || this.IsSouth5
    // 属于赤道十面
    member this.IsEquator10 = not this.IsPole10
    member this.IsEquatorWest = this.Row = 1
    member this.IsEquatorEast = this.Row = 2
    member this.IsValid = this.Type <> TypeEnum.Invalid
    // 距离左边最近的边的 Q 差值
    // （当 Column 4 向左跨越回 Column 0 时，保持返回与普通情况一致性，即：将 Column 0 视作 6 的位置计算）
    member private this.LeftEdgeDiffQ() =
        match this.Row with
        | 0 -> this.Coords.Q + this.Coords.R + SphereAxial.Div + this.Column * SphereAxial.Div
        | 1 -> this.Coords.Q + SphereAxial.Div + this.Column * SphereAxial.Div
        | 2 -> this.Coords.Q + this.Coords.R + this.Column * SphereAxial.Div
        | 3 -> this.Coords.Q + SphereAxial.Div + this.Column * SphereAxial.Div
        | _ -> failwith "Invalid row" // TODO: 去掉异常
    // 距离右边最近的边的 Q 差值（向右不存在特殊情况）
    member private this.RightEdgeDiffQ() =
        match this.Row with
        | 0 -> -this.Column * SphereAxial.Div - this.Coords.Q
        | 1 -> -this.Column * SphereAxial.Div - this.Coords.Q - this.Coords.R
        | 2 -> -this.Column * SphereAxial.Div - this.Coords.Q
        | 3 -> -this.Column * SphereAxial.Div - this.Coords.Q - this.Coords.R + SphereAxial.Div
        | _ -> failwith "Invalid row" // TODO: 去掉异常
    // 左右边最近的边的 Q 差值（特殊情况的处理规则同左边情况）
    member private this.LeftRightEdgeDiffQ() =
        match this.Row with
        | 0 -> this.Coords.R + SphereAxial.Div
        | 1 -> SphereAxial.Div - this.Coords.R
        | 2 -> this.Coords.R
        | 3 -> 2 * SphereAxial.Div - this.Coords.R
        | _ -> failwith "Invalid row" // TODO: 去掉异常

    /// <summary>
    /// 获取原始正二十面体的三角形的三个顶点
    /// </summary>
    /// <returns>按照非平行于 XZ 平面的边的单独点第一个，然后两个平行边上的点按顺时针顺序排列，返回三个点的数组</returns>
    member private this.TriangleVertices() =
        let nextColumn = (this.Column + 1) % 5
        let sa = this // F# 闭包不能传 this 这种 byref

        match this.Row with
        | 0 ->
            seq {
                SphereAxial(0, -SphereAxial.Div)
                SphereAxial(-sa.Column * SphereAxial.Div, 0)
                SphereAxial(-nextColumn * SphereAxial.Div, 0)
            }
        | 1 ->
            seq {
                SphereAxial(-nextColumn * SphereAxial.Div, SphereAxial.Div)
                SphereAxial(-nextColumn * SphereAxial.Div, 0)
                SphereAxial(-sa.Column * SphereAxial.Div, 0)
            }
        | 2 ->
            seq {
                SphereAxial(-sa.Column * SphereAxial.Div, 0)
                SphereAxial(-sa.Column * SphereAxial.Div, SphereAxial.Div)
                SphereAxial(-nextColumn * SphereAxial.Div, SphereAxial.Div)
            }
        | 3 ->
            seq {
                SphereAxial(-SphereAxial.Div, 2 * SphereAxial.Div)
                SphereAxial(-nextColumn * SphereAxial.Div, SphereAxial.Div)
                SphereAxial(-sa.Column * SphereAxial.Div, SphereAxial.Div)
            }
        | _ -> null
    // 转经纬度
    member this.ToLonLat() =
        match this.Type with
        | TypeEnum.PoleVertices -> LonLatCoords(0f, 90f * (if this.TypeIdx = 0 then 1f else -1f))
        | TypeEnum.MidVertices ->
            let longitude = float32 (this.TypeIdx / 2) * 72f - float32 (this.TypeIdx % 2) * 36f

            let latitude =
                if this.TypeIdx % 2 = 0 then
                    29.141262794f
                else
                    -29.141262794f

            LonLatCoords(longitude, latitude)
        | TypeEnum.Edges
        | TypeEnum.EdgesSpecial
        | TypeEnum.Faces
        | TypeEnum.FacesSpecial ->
            let tri = this.TriangleVertices() |> Seq.toArray
            let triCoords = tri |> Array.map _.ToLonLat()

            let horizontalCoords1 =
                triCoords[0]
                    .Slerp(triCoords[1], float32 <| Mathf.Abs((this.Coords.R - tri[0].Coords.R) / SphereAxial.Div))

            let horizontalCoords2 =
                triCoords[0]
                    .Slerp(triCoords[2], float32 <| Mathf.Abs((this.Coords.R - tri[0].Coords.R) / SphereAxial.Div))

            horizontalCoords1.Slerp(
                horizontalCoords2,
                float32 (
                    (if this.Row % 2 = 1 then
                         this.LeftEdgeDiffQ()
                     else
                         this.RightEdgeDiffQ())
                    / this.LeftRightEdgeDiffQ()
                )
            )
        | _ -> failwith $"暂不支持的类型：{this.Type}" // TODO: 去掉异常
    // TODO：现在只能先分情况全写一遍了…… 有点蠢，后续优化
    member this.DistanceTo(sa: SphereAxial) =
        if this.Column = sa.Column then // 同一列可以直接按平面求距离
            this.Coords.DistanceTo(sa.Coords)
        elif this.IsEquator10 && sa.IsEquator10 then // 两者都在赤道十面内
            let left = if this.Index > sa.Index then this else sa
            let right = if this.Index < sa.Index then this else sa

            Mathf.Min(
                left.Coords.DistanceTo(right.Coords),
                right.Coords.DistanceTo(left.Coords + AxialCoords(SphereAxial.Width, 0))
            )
        elif this.Type = TypeEnum.PoleVertices then
            if this.TypeIdx = 1 then
                2 * SphereAxial.Div - sa.Coords.R
            else
                sa.Coords.R + SphereAxial.Div
        elif sa.Type = TypeEnum.PoleVertices then
            if sa.TypeIdx = 1 then
                2 * SphereAxial.Div - this.Coords.R
            else
                this.Coords.R + SphereAxial.Div
        else
            let rec distanceOnePole (sa1: SphereAxial) (sa2: SphereAxial) =
                if sa1.IsNorth5 then
                    // 北极五面
                    match Mathf.PosMod(sa2.Index - sa1.Index, 20) with
                    | 6
                    | 7 ->
                        // sa 在逆斜列（/）上的情况，直接按平面求距离
                        if sa1.Index < sa2.Index then
                            sa2.Coords.DistanceTo sa1.Coords
                        else
                            sa2.Coords.DistanceTo <| sa1.Coords + AxialCoords(SphereAxial.Width, 0)
                    | 4
                    | 5
                    | 10
                    | 11 ->
                        // sa 在左边逆斜列（/）的情况
                        let rotLeft =
                            sa1.Coords.RotateCounterClockwiseAround(AxialCoords(-(sa1.Column + 1) * SphereAxial.Div, 0))

                        if sa1.Index < sa2.Index then
                            sa2.Coords.DistanceTo rotLeft
                        else
                            sa2.Coords.DistanceTo <| rotLeft + AxialCoords(SphereAxial.Width, 0)
                    | 8
                    | 9 ->
                        // sa 在左边隔一列的逆斜列（/）的情况
                        let rotLeft2 =
                            sa1.Coords
                                .RotateCounterClockwiseAround(AxialCoords(-(sa1.Column + 1) * SphereAxial.Div, 0))
                                .RotateCounterClockwiseAround(AxialCoords(-(sa1.Column + 2) * SphereAxial.Div, 0))

                        if sa1.Index < sa2.Index then
                            sa2.Coords.DistanceTo rotLeft2
                        else
                            sa2.Coords.DistanceTo <| rotLeft2 + AxialCoords(SphereAxial.Width, 0)
                    | 14
                    | 15 ->
                        // 14，15 是边界情况，可能看作左边隔一列的逆斜列（/）近，也可能看作右边隔一列的斜列（\）近
                        let rot2Left =
                            sa1.Coords
                                .RotateCounterClockwiseAround(AxialCoords(-(sa1.Column + 1) * SphereAxial.Div, 0))
                                .RotateCounterClockwiseAround(AxialCoords(-(sa1.Column + 2) * SphereAxial.Div, 0))

                        let rot2Right =
                            sa1.Coords
                                .RotateClockwiseAround(AxialCoords(-sa1.Column * SphereAxial.Div, 0))
                                .RotateClockwiseAround(AxialCoords(-(sa1.Column - 1) * SphereAxial.Div, 0))

                        Mathf.Min(
                            (if sa1.Index < sa2.Index then
                                 sa2.Coords.DistanceTo rot2Left
                             else
                                 sa2.Coords.DistanceTo <| rot2Left + AxialCoords(SphereAxial.Width, 0)),
                            if sa1.Index < sa2.Index then
                                rot2Right.DistanceTo <| sa2.Coords + AxialCoords(SphereAxial.Width, 0)
                            else
                                rot2Right.DistanceTo sa2.Coords
                        )
                    | 12
                    | 13 ->
                        // sa 在右边隔一列的斜列（\）的情况
                        let rotRight2 =
                            sa1.Coords
                                .RotateClockwiseAround(AxialCoords(-sa1.Column * SphereAxial.Div, 0))
                                .RotateClockwiseAround(AxialCoords(-(sa1.Column - 1) * SphereAxial.Div, 0))

                        if sa1.Index < sa2.Index then
                            rotRight2.DistanceTo <| sa2.Coords + AxialCoords(SphereAxial.Width, 0)
                        else
                            rotRight2.DistanceTo sa2.Coords
                    | 16
                    | 17
                    | 18
                    | 19 ->
                        // sa 在右边斜列（\）上的情况
                        let rotRight =
                            sa1.Coords.RotateClockwiseAround(AxialCoords(-sa1.Column * SphereAxial.Div, 0))

                        if sa1.Index < sa2.Index then
                            rotRight.DistanceTo <| sa2.Coords + AxialCoords(SphereAxial.Width, 0)
                        else
                            rotRight.DistanceTo sa2.Coords
                    | _ -> failwith "distanceOnePole North5 不可能的情况" // TODO: 去掉异常
                elif sa1.IsSouth5 then
                    // 南极五面
                    match Mathf.PosMod(sa2.Index - sa1.Index, 20) with
                    | 1
                    | 2
                    | 3
                    | 4 ->
                        // sa 在左边斜列（\）上的情况
                        let rotLeft =
                            sa1.Coords.RotateClockwiseAround(
                                AxialCoords(-(sa1.Column + 1) * SphereAxial.Div, SphereAxial.Div)
                            )

                        if sa1.Index < sa2.Index then
                            sa2.Coords.DistanceTo rotLeft
                        else
                            sa2.Coords.DistanceTo <| rotLeft + AxialCoords(SphereAxial.Width, 0)
                    | 7
                    | 8 ->
                        // sa 在左边隔一列的斜列（\）上的情况
                        let rotLeft2 =
                            sa1.Coords
                                .RotateClockwiseAround(
                                    AxialCoords(-(sa1.Column + 1) * SphereAxial.Div, SphereAxial.Div)
                                )
                                .RotateClockwiseAround(
                                    AxialCoords(-(sa1.Column + 2) * SphereAxial.Div, SphereAxial.Div)
                                )

                        if sa1.Index < sa2.Index then
                            sa.Coords.DistanceTo rotLeft2
                        else
                            sa2.Coords.DistanceTo <| rotLeft2 + AxialCoords(SphereAxial.Width, 0)
                    | 5
                    | 6 ->
                        // 5，6 是边界情况，可能看作左边隔一列的逆斜列（/）近，也可能看作右边隔一列的斜列（\）近
                        let rot2Left =
                            sa1.Coords
                                .RotateClockwiseAround(
                                    AxialCoords(-(sa1.Column + 1) * SphereAxial.Div, SphereAxial.Div)
                                )
                                .RotateClockwiseAround(
                                    AxialCoords(-(sa1.Column + 2) * SphereAxial.Div, SphereAxial.Div)
                                )

                        let rot2Right =
                            sa1.Coords
                                .RotateCounterClockwiseAround(
                                    AxialCoords(-sa1.Column * SphereAxial.Div, SphereAxial.Div)
                                )
                                .RotateCounterClockwiseAround(
                                    AxialCoords(-(sa1.Column - 1) * SphereAxial.Div, SphereAxial.Div)
                                )

                        let leftDist =
                            if sa1.Index < sa2.Index then
                                sa2.Coords.DistanceTo rot2Left
                            else
                                sa2.Coords.DistanceTo <| rot2Left + AxialCoords(SphereAxial.Width, 0)

                        let rightDist =
                            if sa1.Index < sa2.Index then
                                rot2Right.DistanceTo <| sa2.Coords + AxialCoords(SphereAxial.Width, 0)
                            else
                                rot2Right.DistanceTo sa2.Coords

                        Mathf.Min(leftDist, rightDist)
                    | 11
                    | 12 ->
                        // sa 在右边隔一列的逆斜列（/）上的情况
                        let rotRight2 =
                            sa1.Coords
                                .RotateCounterClockwiseAround(
                                    AxialCoords(-sa1.Column * SphereAxial.Div, SphereAxial.Div)
                                )
                                .RotateCounterClockwiseAround(
                                    AxialCoords(-(sa1.Column - 1) * SphereAxial.Div, SphereAxial.Div)
                                )

                        if sa1.Index < sa2.Index then
                            rotRight2.DistanceTo <| sa2.Coords + AxialCoords(SphereAxial.Width, 0)
                        else
                            rotRight2.DistanceTo sa2.Coords
                    | 9
                    | 10
                    | 15
                    | 16 ->
                        // sa 在右边逆斜列（/）上的情况
                        let rotRight =
                            sa1.Coords.RotateCounterClockwiseAround(
                                AxialCoords(-sa1.Column * SphereAxial.Div, SphereAxial.Div)
                            )

                        if sa1.Index < sa2.Index then
                            rotRight.DistanceTo <| sa2.Coords + AxialCoords(SphereAxial.Width, 0)
                        else
                            rotRight.DistanceTo sa2.Coords
                    | 13
                    | 14 ->
                        // sa 在逆斜列（/）上的情况，直接按平面求距离
                        if sa1.Index < sa2.Index then
                            sa1.Coords.DistanceTo <| sa2.Coords + AxialCoords(SphereAxial.Width, 0)
                        else
                            sa1.Coords.DistanceTo sa2.Coords
                    | _ -> failwith "distanceOnePole South5 不可能的情况" // TODO: 去掉异常
                else
                    distanceOnePole sa2 sa1

            distanceOnePole this sa
