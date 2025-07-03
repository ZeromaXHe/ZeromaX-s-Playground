namespace TO.Domains.Functions.HexGridCoords

open Godot
open TO.Domains.Types.HexGridCoords

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:07:29
module SphereAxial =
    let toString (this: SphereAxial) =
        $"{this.Coords |> AxialCoords.toString} {this.Type} {this.TypeIdx}"

    let specialNeighbor (this: SphereAxial) =
        this.Type = SphereAxialTypeEnum.EdgesSpecial
        || this.Type = SphereAxialTypeEnum.FacesSpecial

    // 正二十面体索引，0 ~ 19
    let index (this: SphereAxial) =
        match this.Type with
        | SphereAxialTypeEnum.PoleVertices -> if this.TypeIdx = 0 then 0 else 3
        | SphereAxialTypeEnum.MidVertices -> this.TypeIdx / 2 * 4 + 1 + this.TypeIdx % 2
        | SphereAxialTypeEnum.Edges
        | SphereAxialTypeEnum.EdgesSpecial -> this.TypeIdx / 6 * 4 + (this.TypeIdx % 6 + 1) / 2
        | SphereAxialTypeEnum.Faces
        | SphereAxialTypeEnum.FacesSpecial -> this.TypeIdx / 4 * 4 + this.TypeIdx % 4
        | _ -> -1
    // 获取列索引，从右到左 0 ~ 4
    let column (this: SphereAxial) =
        if this.Type = SphereAxialTypeEnum.PoleVertices && this.TypeIdx = 1 then
            0
        elif this.Type <> SphereAxialTypeEnum.Invalid then
            -this.Coords.Q / SphereAxial.Div
        else
            -1
    // 获取行索引，从上到下 0 ~ 3
    let row (this: SphereAxial) =
        match this.Type with
        | SphereAxialTypeEnum.PoleVertices -> if this.TypeIdx = 0 then 0 else 3
        | SphereAxialTypeEnum.MidVertices -> 1 + this.TypeIdx % 2
        | SphereAxialTypeEnum.Edges
        | SphereAxialTypeEnum.EdgesSpecial -> (this.TypeIdx % 6 + 1) / 2
        | SphereAxialTypeEnum.Faces
        | SphereAxialTypeEnum.FacesSpecial -> this.TypeIdx % 4
        | _ -> -1
    // 在北边的 5 个面上
    let isNorth5 (this: SphereAxial) = row this = 0
    // 在南边的 5 个面上
    let isSouth5 (this: SphereAxial) = row this = 3
    // 属于极地十面
    let isPole10 (this: SphereAxial) = isNorth5 this || isSouth5 this
    // 属于赤道十面
    let isEquator10 (this: SphereAxial) = not <| isPole10 this
    let isEquatorWest (this: SphereAxial) = row this = 1
    let isEquatorEast (this: SphereAxial) = row this = 2

    let isValid (this: SphereAxial) =
        this.Type <> SphereAxialTypeEnum.Invalid
    // 距离左边最近的边的 Q 差值
    // （当 Column 4 向左跨越回 Column 0 时，保持返回与普通情况一致性，即：将 Column 0 视作 6 的位置计算）
    let private leftEdgeDiffQ (this: SphereAxial) =
        match row this with
        | 0 -> this.Coords.Q + this.Coords.R + SphereAxial.Div + column this * SphereAxial.Div
        | 1 -> this.Coords.Q + SphereAxial.Div + column this * SphereAxial.Div
        | 2 -> this.Coords.Q + this.Coords.R + column this * SphereAxial.Div
        | 3 -> this.Coords.Q + SphereAxial.Div + column this * SphereAxial.Div
        | _ -> failwith "Invalid row" // TODO: 去掉异常

    /// 距离右边最近的边的 Q 差值（向右不存在特殊情况）
    let private rightEdgeDiffQ (this: SphereAxial) =
        match row this with
        | 0 -> -column this * SphereAxial.Div - this.Coords.Q
        | 1 -> -column this * SphereAxial.Div - this.Coords.Q - this.Coords.R
        | 2 -> -column this * SphereAxial.Div - this.Coords.Q
        | 3 -> -column this * SphereAxial.Div - this.Coords.Q - this.Coords.R + SphereAxial.Div
        | _ -> failwith "Invalid row" // TODO: 去掉异常

    /// 左右边最近的边的 Q 差值（特殊情况的处理规则同左边情况）
    let private leftRightEdgeDiffQ (this: SphereAxial) =
        match row this with
        | 0 -> this.Coords.R + SphereAxial.Div
        | 1 -> SphereAxial.Div - this.Coords.R
        | 2 -> this.Coords.R
        | 3 -> 2 * SphereAxial.Div - this.Coords.R
        | _ -> failwith "Invalid row" // TODO: 去掉异常

    /// <summary>
    /// 获取原始正二十面体的三角形的三个顶点
    /// </summary>
    /// <returns>按照非平行于 XZ 平面的边的单独点第一个，然后两个平行边上的点按顺时针顺序排列，返回三个点的数组</returns>
    let private triangleVertices (this: SphereAxial) =
        let nextColumn = (column this + 1) % 5
        let sa = this // F# 闭包不能传 this 这种 byref

        match row this with
        | 0 ->
            seq {
                SphereAxial(0, -SphereAxial.Div)
                SphereAxial(-column sa * SphereAxial.Div, 0)
                SphereAxial(-nextColumn * SphereAxial.Div, 0)
            }
        | 1 ->
            seq {
                SphereAxial(-nextColumn * SphereAxial.Div, SphereAxial.Div)
                SphereAxial(-nextColumn * SphereAxial.Div, 0)
                SphereAxial(-column sa * SphereAxial.Div, 0)
            }
        | 2 ->
            seq {
                SphereAxial(-column sa * SphereAxial.Div, 0)
                SphereAxial(-column sa * SphereAxial.Div, SphereAxial.Div)
                SphereAxial(-nextColumn * SphereAxial.Div, SphereAxial.Div)
            }
        | 3 ->
            seq {
                SphereAxial(-SphereAxial.Div, 2 * SphereAxial.Div)
                SphereAxial(-nextColumn * SphereAxial.Div, SphereAxial.Div)
                SphereAxial(-column sa * SphereAxial.Div, SphereAxial.Div)
            }
        | _ -> null
    // 转经纬度
    let rec toLonLat (this: SphereAxial) =
        match this.Type with
        | SphereAxialTypeEnum.PoleVertices -> LonLatCoords(0f, 90f * (if this.TypeIdx = 0 then 1f else -1f))
        | SphereAxialTypeEnum.MidVertices ->
            let longitude = float32 (this.TypeIdx / 2) * 72f - float32 (this.TypeIdx % 2) * 36f

            let latitude =
                if this.TypeIdx % 2 = 0 then
                    29.141262794f
                else
                    -29.141262794f

            LonLatCoords(longitude, latitude)
        | SphereAxialTypeEnum.Edges
        | SphereAxialTypeEnum.EdgesSpecial
        | SphereAxialTypeEnum.Faces
        | SphereAxialTypeEnum.FacesSpecial ->
            let tri = triangleVertices this |> Seq.toArray
            let triCoords = tri |> Array.map toLonLat

            let horizontalCoords1 =
                triCoords[0]
                |> LonLatCoords.slerp
                    triCoords[1]
                    (float32 <| Mathf.Abs((this.Coords.R - tri[0].Coords.R) / SphereAxial.Div))

            let horizontalCoords2 =
                triCoords[0]
                |> LonLatCoords.slerp
                    triCoords[2]
                    (float32 <| Mathf.Abs((this.Coords.R - tri[0].Coords.R) / SphereAxial.Div))

            horizontalCoords1
            |> LonLatCoords.slerp
                horizontalCoords2
                (float32
                 <| (if row this % 2 = 1 then
                         leftEdgeDiffQ this
                     else
                         rightEdgeDiffQ this)
                    / leftRightEdgeDiffQ this)
        | _ -> failwith $"暂不支持的类型：{this.Type}" // TODO: 去掉异常

    let distanceTo (sa: SphereAxial) (this: SphereAxial) =
        // TODO：现在只能先分情况全写一遍了…… 有点蠢，后续优化
        if column this = column sa then // 同一列可以直接按平面求距离
            this.Coords |> AxialCoords.distanceTo sa.Coords
        elif isEquator10 this && isEquator10 sa then // 两者都在赤道十面内
            let left = if index this > index sa then this else sa
            let right = if index this < index sa then this else sa

            Mathf.Min(
                left.Coords |> AxialCoords.distanceTo right.Coords,
                right.Coords |> AxialCoords.distanceTo
                <| (left.Coords |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
            )
        elif this.Type = SphereAxialTypeEnum.PoleVertices then
            if this.TypeIdx = 1 then
                2 * SphereAxial.Div - sa.Coords.R
            else
                sa.Coords.R + SphereAxial.Div
        elif sa.Type = SphereAxialTypeEnum.PoleVertices then
            if sa.TypeIdx = 1 then
                2 * SphereAxial.Div - this.Coords.R
            else
                this.Coords.R + SphereAxial.Div
        else
            let rec distanceOnePole (sa1: SphereAxial) (sa2: SphereAxial) =
                if isNorth5 sa1 then
                    // 北极五面
                    match Mathf.PosMod(index sa2 - index sa1, 20) with
                    | 6
                    | 7 ->
                        // sa 在逆斜列（/）上的情况，直接按平面求距离
                        if index sa1 < index sa2 then
                            sa2.Coords |> AxialCoords.distanceTo sa1.Coords
                        else
                            sa2.Coords |> AxialCoords.distanceTo
                            <| (sa1.Coords |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                    | 4
                    | 5
                    | 10
                    | 11 ->
                        // sa 在左边逆斜列（/）的情况
                        let rotLeft =
                            sa1.Coords |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-(column sa1 + 1) * SphereAxial.Div, 0)

                        if index sa1 < index sa2 then
                            sa2.Coords |> AxialCoords.distanceTo rotLeft
                        else
                            sa2.Coords |> AxialCoords.distanceTo
                            <| (rotLeft |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                    | 8
                    | 9 ->
                        // sa 在左边隔一列的逆斜列（/）的情况
                        let rotLeft2 =
                            sa1.Coords |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-(column sa1 + 1) * SphereAxial.Div, 0)
                            |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-(column sa1 + 2) * SphereAxial.Div, 0)

                        if index sa1 < index sa2 then
                            sa2.Coords |> AxialCoords.distanceTo rotLeft2
                        else
                            sa2.Coords |> AxialCoords.distanceTo
                            <| (rotLeft2 |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                    | 14
                    | 15 ->
                        // 14，15 是边界情况，可能看作左边隔一列的逆斜列（/）近，也可能看作右边隔一列的斜列（\）近
                        let rot2Left =
                            sa1.Coords |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-(column sa1 + 1) * SphereAxial.Div, 0)
                            |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-(column sa1 + 2) * SphereAxial.Div, 0)

                        let rot2Right =
                            sa1.Coords |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-column sa1 * SphereAxial.Div, 0)
                            |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-(column sa1 - 1) * SphereAxial.Div, 0)

                        Mathf.Min(
                            (if index sa1 < index sa2 then
                                 sa2.Coords |> AxialCoords.distanceTo rot2Left
                             else
                                 sa2.Coords |> AxialCoords.distanceTo
                                 <| (rot2Left |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))),
                            if index sa1 < index sa2 then
                                rot2Right |> AxialCoords.distanceTo
                                <| (sa2.Coords |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                            else
                                rot2Right |> AxialCoords.distanceTo sa2.Coords
                        )
                    | 12
                    | 13 ->
                        // sa 在右边隔一列的斜列（\）的情况
                        let rotRight2 =
                            sa1.Coords |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-column sa1 * SphereAxial.Div, 0)
                            |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-(column sa1 - 1) * SphereAxial.Div, 0)

                        if index sa1 < index sa2 then
                            rotRight2 |> AxialCoords.distanceTo
                            <| (sa2.Coords |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                        else
                            rotRight2 |> AxialCoords.distanceTo sa2.Coords
                    | 16
                    | 17
                    | 18
                    | 19 ->
                        // sa 在右边斜列（\）上的情况
                        let rotRight =
                            sa1.Coords |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-column sa1 * SphereAxial.Div, 0)

                        if index sa1 < index sa2 then
                            rotRight |> AxialCoords.distanceTo
                            <| (sa2.Coords |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                        else
                            rotRight |> AxialCoords.distanceTo sa2.Coords
                    | _ -> failwith "distanceOnePole North5 不可能的情况" // TODO: 去掉异常
                elif isSouth5 sa1 then
                    // 南极五面
                    match Mathf.PosMod(index sa2 - index sa1, 20) with
                    | 1
                    | 2
                    | 3
                    | 4 ->
                        // sa 在左边斜列（\）上的情况
                        let rotLeft =
                            sa1.Coords |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-(column sa1 + 1) * SphereAxial.Div, SphereAxial.Div)

                        if index sa1 < index sa2 then
                            sa2.Coords |> AxialCoords.distanceTo rotLeft
                        else
                            sa2.Coords |> AxialCoords.distanceTo
                            <| (rotLeft |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                    | 7
                    | 8 ->
                        // sa 在左边隔一列的斜列（\）上的情况
                        let rotLeft2 =
                            sa1.Coords |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-(column sa1 + 1) * SphereAxial.Div, SphereAxial.Div)
                            |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-(column sa1 + 2) * SphereAxial.Div, SphereAxial.Div)

                        if index sa1 < index sa2 then
                            sa.Coords |> AxialCoords.distanceTo rotLeft2
                        else
                            sa2.Coords |> AxialCoords.distanceTo
                            <| (rotLeft2 |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                    | 5
                    | 6 ->
                        // 5，6 是边界情况，可能看作左边隔一列的逆斜列（/）近，也可能看作右边隔一列的斜列（\）近
                        let rot2Left =
                            sa1.Coords |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-(column sa1 + 1) * SphereAxial.Div, SphereAxial.Div)
                            |> AxialCoords.rotateClockwiseAround
                            <| AxialCoords(-(column sa1 + 2) * SphereAxial.Div, SphereAxial.Div)

                        let rot2Right =
                            sa1.Coords |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-column sa1 * SphereAxial.Div, SphereAxial.Div)
                            |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-(column sa1 - 1) * SphereAxial.Div, SphereAxial.Div)

                        let leftDist =
                            if index sa1 < index sa2 then
                                sa2.Coords |> AxialCoords.distanceTo rot2Left
                            else
                                sa2.Coords |> AxialCoords.distanceTo
                                <| (rot2Left |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))

                        let rightDist =
                            if index sa1 < index sa2 then
                                rot2Right |> AxialCoords.distanceTo
                                <| (sa2.Coords |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                            else
                                rot2Right |> AxialCoords.distanceTo sa2.Coords

                        Mathf.Min(leftDist, rightDist)
                    | 11
                    | 12 ->
                        // sa 在右边隔一列的逆斜列（/）上的情况
                        let rotRight2 =
                            sa1.Coords |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-column sa1 * SphereAxial.Div, SphereAxial.Div)
                            |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-(column sa1 - 1) * SphereAxial.Div, SphereAxial.Div)

                        if index sa1 < index sa2 then
                            rotRight2 |> AxialCoords.distanceTo
                            <| (sa2.Coords |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                        else
                            rotRight2 |> AxialCoords.distanceTo sa2.Coords
                    | 9
                    | 10
                    | 15
                    | 16 ->
                        // sa 在右边逆斜列（/）上的情况
                        let rotRight =
                            sa1.Coords |> AxialCoords.rotateCounterClockwiseAround
                            <| AxialCoords(-column sa1 * SphereAxial.Div, SphereAxial.Div)

                        if index sa1 < index sa2 then
                            rotRight |> AxialCoords.distanceTo
                            <| (sa2.Coords |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                        else
                            rotRight |> AxialCoords.distanceTo sa2.Coords
                    | 13
                    | 14 ->
                        // sa 在逆斜列（/）上的情况，直接按平面求距离
                        if index sa1 < index sa2 then
                            sa1.Coords |> AxialCoords.distanceTo
                            <| (sa2.Coords |> AxialCoords.add <| AxialCoords(SphereAxial.Width, 0))
                        else
                            sa1.Coords |> AxialCoords.distanceTo sa2.Coords
                    | _ -> failwith "distanceOnePole South5 不可能的情况" // TODO: 去掉异常
                else
                    distanceOnePole sa2 sa1

            distanceOnePole this sa
