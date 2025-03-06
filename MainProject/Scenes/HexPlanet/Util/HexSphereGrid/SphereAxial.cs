using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexPlaneGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

public readonly struct SphereAxial(int q, int r, SphereAxial.TypeEnum type, int typeIdx)
{
    public enum TypeEnum
    {
        // 正二十面体南北极顶点。
        // 索引：0 北极点，1 南极点
        PoleVertices,

        // 正二十面体中间的十个顶点。
        // 索引：0、1 第一组竖向四面的中间右侧从北到南两点，2、3 第二组，以此类推，8、9 第五组（最后一组）
        MidVertices,

        // 正二十面体 边上的普通点（可以简单用六边形坐标搜索邻居）
        // 索引：0 ~ 5 第一组竖向四面的从北到南六边（左侧三边不算），6 ~ 11 第二组，以此类推，24 ~ 29 第五组（最后一组）
        Edges,

        // 正二十面体 边上的特殊点
        // 索引：0 ~ 5 第一组竖向四面的从北到南六边（左侧三边不算），6 ~ 11 第二组，以此类推，24 ~ 29 第五组（最后一组）
        // 索引 % 6 == 0 || 5（第一边和最后一边）时，相邻的面索引是当前面索引 - 4
        // 其它情况说明在南北回归线西边与 MidVertices 相邻
        EdgesSpecial,

        // 正二十面体 面上的普通点（可以简单用六边形坐标搜索邻居）
        // 索引：0 ~ 3 第一组竖向四面从北到南，4 ~ 7 第二组，以此类推，16 ~ 19 第五组（最后一组）
        Faces,

        // 正二十面体 面上的特殊点（需要用特殊规则搜索邻居
        // 索引：0 ~ 3 第一组竖向四面从北到南，4 ~ 7 第二组，以此类推，16 ~ 19 第五组（最后一组）
        // 相邻的面索引 + 4 即可
        FacesSpecial,
    }

    public readonly AxialCoords Coords = new(q, r);
    public readonly TypeEnum Type = type;
    public readonly int TypeIdx = typeIdx;

    public bool SpecialNeighbor => Type is TypeEnum.EdgesSpecial or TypeEnum.FacesSpecial;

    // 正二十面体索引，0 ~ 19
    public readonly int Index = type switch
    {
        TypeEnum.PoleVertices => typeIdx == 0 ? 0 : 3,
        TypeEnum.MidVertices => typeIdx / 2 * 4 + 1 + typeIdx % 2,
        TypeEnum.Edges or TypeEnum.EdgesSpecial => typeIdx / 6 * 4 + (typeIdx % 6 + 1) / 2,
        TypeEnum.Faces or TypeEnum.FacesSpecial => typeIdx / 4 * 4 + typeIdx % 4,
        _ => -1
    };

    // 获取列索引，从右到左 0 ~ 4
    public readonly int Column = type switch
    {
        TypeEnum.PoleVertices => 0,
        TypeEnum.MidVertices => typeIdx / 2,
        TypeEnum.Edges or TypeEnum.EdgesSpecial => typeIdx / 6,
        TypeEnum.Faces or TypeEnum.FacesSpecial => typeIdx / 4,
        _ => -1
    };

    // 获取行索引，从上到下 0 ~ 3
    public readonly int Row = type switch
    {
        TypeEnum.PoleVertices => typeIdx == 0 ? 0 : 3,
        TypeEnum.MidVertices => 1 + typeIdx % 2,
        TypeEnum.Edges or TypeEnum.EdgesSpecial => (typeIdx % 6 + 1) / 2,
        TypeEnum.Faces or TypeEnum.FacesSpecial => typeIdx % 4,
        _ => -1
    };

    // 在北边的 5 个面上
    public bool IsNorth5 => Row == 0;
    // (type is TypeEnum.Faces or TypeEnum.FacesSpecial && typeIdx % 4 == 0) ||
    // (type is TypeEnum.Edges or TypeEnum.EdgesSpecial && typeIdx % 6 == 0) ||
    // (type == TypeEnum.PoleVertices && typeIdx == 0);

    // 在南边的 5 个面上
    public bool IsSouth5 => Row == 3;
    // (type is TypeEnum.Faces or TypeEnum.FacesSpecial && typeIdx % 4 == 3) ||
    // (type is TypeEnum.Edges or TypeEnum.EdgesSpecial && typeIdx % 6 == 5) ||
    // (type == TypeEnum.PoleVertices && typeIdx == 1);

    // 属于极地十面
    public bool IsPole10 => IsNorth5 || IsSouth5;

    // 属于赤道十面
    public bool IsEquator10 => !IsPole10;

    public bool IsEquatorWest => Row == 1;
    // (type is TypeEnum.Edges or TypeEnum.EdgesSpecial && (typeIdx % 6 == 1 || typeIdx % 6 == 2))
    // || (type is TypeEnum.Faces or TypeEnum.FacesSpecial && typeIdx % 4 == 1)
    // || (type is TypeEnum.MidVertices && typeIdx % 2 == 0);

    public bool IsEquatorEast => Row == 2;
    // (type is TypeEnum.Edges or TypeEnum.EdgesSpecial && (typeIdx % 6 == 3 || typeIdx % 6 == 4))
    // || (type is TypeEnum.Faces or TypeEnum.FacesSpecial && typeIdx % 4 == 2)
    // || (type is TypeEnum.MidVertices && typeIdx % 2 == 1);

    private static int Width => HexMetrics.Divisions * 5;
    private static int Div => HexMetrics.Divisions;

    private static bool ValidateAxial(int q, int r)
    {
        // q 在 (-Width, 0] 之间
        if (q > 0 || q <= -Width)
            return false;
        // r 在 (-ColWidth, 2 * ColWidth) 之间)
        if (r < -Div || r > 2 * Div)
            return false;
        // 北极点
        if (r == -Div)
            return q == 0;
        // 南极点
        if (r == 2 * Div)
            return q == Div;
        if (r < 0)
            return Mathf.PosMod(q, Div) > -r;
        if (r > Div)
            return Mathf.PosMod(q, Div) <= 2 * Div - r;
        return true;
    }

    // TODO：现在只能先分情况全写一遍了…… 有点蠢，后续优化
    public int DistanceTo(SphereAxial sa)
    {
        if (Column == sa.Column) // 同一列可以直接按平面求距离
            return Coords.DistanceTo(sa.Coords);
        if (IsEquator10 && sa.IsEquator10) // 两者都在赤道十面内
        {
            var left = Index > sa.Index ? this : sa;
            var right = Index < sa.Index ? this : sa;
            return Mathf.Min(left.Coords.DistanceTo(right.Coords),
                right.Coords.DistanceTo(left.Coords + new AxialCoords(Width, 0)));
        }

        // 有其中一个是极点的话，则直接求 R 的差值即可
        if (Type == TypeEnum.PoleVertices)
            return TypeIdx == 1
                ? 2 * Div - sa.Coords.R
                : sa.Coords.R + Div;
        if (sa.Type == TypeEnum.PoleVertices)
            return sa.TypeIdx == 1
                ? 2 * Div - Coords.R
                : Coords.R + Div;
        return DistanceOnePole(sa);
    }

    private int DistanceOnePole(SphereAxial sa)
    {
        if (IsNorth5)
        {
            // 北极五面
            switch (Mathf.PosMod(sa.Index - Index, 20))
            {
                case 6:
                case 7:
                    // sa 在逆斜列上的情况，直接按平面求距离
                {
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(Coords)
                        : sa.Coords.DistanceTo(Coords + new AxialCoords(Width, 0));
                }
                case 4:
                case 5:
                case 10:
                case 11:
                    // sa 在左边逆斜列的情况
                    var rotLeft = Coords.RotateLeftAround(new AxialCoords(-(Column + 1) * Div, 0));
                {
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(rotLeft)
                        : sa.Coords.DistanceTo(rotLeft + new AxialCoords(Width, 0));
                }
                case 8:
                case 9:
                    // sa 在左边隔一列的逆斜列的情况
                    var rotLeft2 = Coords
                        .RotateLeftAround(new AxialCoords(-(Column + 1) * Div, 0))
                        .RotateLeftAround(new AxialCoords(-(Column + 2) * Div, 0));
                {
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(rotLeft2)
                        : sa.Coords.DistanceTo(rotLeft2 + new AxialCoords(Width, 0));
                }
                case 14:
                case 15:
                    // 14，15 是边界情况，可能看作左边隔一列的逆斜列近，也可能看作右边隔一列的斜列近
                    var rot2Left = Coords
                        .RotateLeftAround(new AxialCoords(-(Column + 1) * Div, 0))
                        .RotateLeftAround(new AxialCoords(-(Column + 2) * Div, 0));
                    var rot2Right = Coords
                        .RotateRightAround(new AxialCoords(-Column * Div, 0))
                        .RotateRightAround(new AxialCoords(-(Column - 1) * Div, 0));
                {
                    return Mathf.Min(
                        Index < sa.Index
                            ? sa.Coords.DistanceTo(rot2Left)
                            : sa.Coords.DistanceTo(rot2Left + new AxialCoords(Width, 0)),
                        Index < sa.Index
                            ? rot2Right.DistanceTo(sa.Coords)
                            : rot2Right.DistanceTo(sa.Coords + new AxialCoords(Width, 0)));
                }
                case 12:
                case 13:
                    // sa 在右边隔一列的斜列的情况
                    var rotRight2 = Coords
                        .RotateRightAround(new AxialCoords(-Column * Div, 0))
                        .RotateRightAround(new AxialCoords(-(Column - 1) * Div, 0));
                {
                    return Index < sa.Index
                        ? rotRight2.DistanceTo(sa.Coords)
                        : rotRight2.DistanceTo(sa.Coords + new AxialCoords(Width, 0));
                }
                case 16:
                case 17:
                case 18:
                case 19:
                    // sa 在右边斜列上的情况
                    var rotRight = Coords
                        .RotateRightAround(new AxialCoords(-Column * Div, 0));
                {
                    return Index < sa.Index
                        ? rotRight.DistanceTo(sa.Coords)
                        : rotRight.DistanceTo(sa.Coords + new AxialCoords(Width, 0));
                }
            }
        }
        else if (IsSouth5)
        {
            // 南极五面
            switch (Mathf.PosMod(sa.Index - Index, 20))
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    // sa 在左边斜列上的情况
                    var rotLeft = Coords.RotateLeftAround(new AxialCoords(-(Column + 1) * Div, Div));
                {
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(rotLeft)
                        : sa.Coords.DistanceTo(rotLeft + new AxialCoords(Width, 0));
                }
                case 7:
                case 8:
                    // sa 在左边隔一列的斜列上的情况
                    var rotLeft2 = Coords
                        .RotateLeftAround(new AxialCoords(-(Column + 1) * Div, Div))
                        .RotateLeftAround(new AxialCoords(-(Column + 2) * Div, Div));
                {
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(rotLeft2)
                        : sa.Coords.DistanceTo(rotLeft2 + new AxialCoords(Width, 0));
                }
                case 5:
                case 6:
                    // 5，6 是边界情况，可能看作左边隔一列的逆斜列近，也可能看作右边隔一列的斜列近
                    var rot2Left = Coords
                        .RotateLeftAround(new AxialCoords(-(Column + 1) * Div, Div))
                        .RotateLeftAround(new AxialCoords(-(Column + 2) * Div, Div));
                    var rot2Right = Coords
                        .RotateRightAround(new AxialCoords(-Column * Div, Div))
                        .RotateRightAround(new AxialCoords(-(Column - 1) * Div, Div));
                {
                    return Mathf.Min(
                        Index < sa.Index
                            ? sa.Coords.DistanceTo(rot2Left)
                            : sa.Coords.DistanceTo(rot2Left + new AxialCoords(Width, 0)),
                        Index < sa.Index
                            ? rot2Right.DistanceTo(sa.Coords)
                            : rot2Right.DistanceTo(sa.Coords + new AxialCoords(Width, 0)));
                }
                case 11:
                case 12:
                    // sa 在右边隔一列的逆斜列上的情况
                    var rotRight2 = Coords
                        .RotateRightAround(new AxialCoords(-Column * Div, Div))
                        .RotateRightAround(new AxialCoords(-(Column - 1) * Div, Div));
                {
                    return Index < sa.Index
                        ? rotRight2.DistanceTo(sa.Coords)
                        : rotRight2.DistanceTo(sa.Coords + new AxialCoords(Width, 0));
                }
                case 9:
                case 10:
                case 15:
                case 16:
                    // sa 在右边逆斜列上的情况
                    var rotRight = Coords
                        .RotateRightAround(new AxialCoords(-Column * Div, Div));
                {
                    return Index < sa.Index
                        ? rotRight.DistanceTo(sa.Coords)
                        : rotRight.DistanceTo(sa.Coords + new AxialCoords(Width, 0));
                }
                case 13:
                case 14:
                    // sa 在逆斜列上的情况，直接按平面求距离
                {
                    return Index < sa.Index
                        ? Coords.DistanceTo(sa.Coords)
                        : Coords.DistanceTo(sa.Coords + new AxialCoords(Width, 0));
                }
            }
        }
        else
            return sa.DistanceOnePole(this);

        throw new System.NotImplementedException(); // 按道理不应该走到这里
    }
}