namespace TO.Domains.Types.HexGridCoords

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
    static member val Div = 5 with get, set // TODO: 变量本不应出现在此，待优化
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
