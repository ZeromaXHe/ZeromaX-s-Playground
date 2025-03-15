using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexPlaneGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

public readonly struct SphereAxial
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

        // 无效坐标
        Invalid,
    }

    public readonly AxialCoords Coords;
    public readonly TypeEnum Type;
    public readonly int TypeIdx;

    public SphereAxial(int q, int r)
    {
        Coords = new AxialCoords(q, r);
        if (!ValidateAxial(q, r))
        {
            Type = TypeEnum.Invalid;
            TypeIdx = -1;
        }
        else if (r == -Div || r == 2 * Div)
        {
            Type = TypeEnum.PoleVertices;
            TypeIdx = r == -Div ? 0 : 1;
        }
        else if (r < 0)
        {
            if (-q % Div == 0)
            {
                Type = TypeEnum.EdgesSpecial;
                TypeIdx = -q / Div * 6;
            }
            else
            {
                Type = -q % Div == Div + r - 1 ? TypeEnum.FacesSpecial : TypeEnum.Faces;
                TypeIdx = -q / Div * 4;
            }
        }
        else if (r == 0)
        {
            if (-q % Div == 0)
            {
                Type = TypeEnum.MidVertices;
                TypeIdx = -q / Div * 2;
            }
            else
            {
                Type = -q % Div == Div - 1 ? TypeEnum.EdgesSpecial : TypeEnum.Edges;
                TypeIdx = -q / Div * 6 + 1;
            }
        }
        else if (r < Div)
        {
            if (-q % Div == 0)
            {
                Type = TypeEnum.Edges;
                TypeIdx = -q / Div * 6 + 3;
            }
            else if (-q % Div == r)
            {
                Type = TypeEnum.Edges;
                TypeIdx = -q / Div * 6 + 2;
            }
            else
            {
                Type = TypeEnum.Faces;
                TypeIdx = -q / Div * 4 + (-q % Div > r ? 1 : 2);
            }
        }
        else if (r == Div)
        {
            if (-q % Div == 0)
            {
                Type = TypeEnum.MidVertices;
                TypeIdx = -q / Div * 2 + 1;
            }
            else
            {
                Type = -q % Div == Div - 1 ? TypeEnum.EdgesSpecial : TypeEnum.Edges;
                TypeIdx = -q / Div * 6 + 4;
            }
        }
        else
        {
            if (-q % Div == r - Div)
            {
                Type = TypeEnum.EdgesSpecial;
                TypeIdx = -q / Div * 6 + 5;
            }
            else
            {
                Type = -q % Div == Div - 1 ? TypeEnum.FacesSpecial : TypeEnum.Faces;
                TypeIdx = -q / Div * 4 + 3;
            }
        }
    }

    public override string ToString() => $"({Coords}, {Type}, {TypeIdx})";
    public bool SpecialNeighbor => Type is TypeEnum.EdgesSpecial or TypeEnum.FacesSpecial;

    // 正二十面体索引，0 ~ 19
    public int Index => Type switch
    {
        TypeEnum.PoleVertices => TypeIdx == 0 ? 0 : 3,
        TypeEnum.MidVertices => TypeIdx / 2 * 4 + 1 + TypeIdx % 2,
        TypeEnum.Edges or TypeEnum.EdgesSpecial => TypeIdx / 6 * 4 + (TypeIdx % 6 + 1) / 2,
        TypeEnum.Faces or TypeEnum.FacesSpecial => TypeIdx / 4 * 4 + TypeIdx % 4,
        _ => -1
    };

    // 获取列索引，从右到左 0 ~ 4
    public int Column => Type is TypeEnum.PoleVertices && TypeIdx == 1 ? 0 :
        Type is not TypeEnum.Invalid ? -Coords.Q / Div : -1;

    // 获取行索引，从上到下 0 ~ 3
    public int Row => Type switch
    {
        TypeEnum.PoleVertices => TypeIdx == 0 ? 0 : 3,
        TypeEnum.MidVertices => 1 + TypeIdx % 2,
        TypeEnum.Edges or TypeEnum.EdgesSpecial => (TypeIdx % 6 + 1) / 2,
        TypeEnum.Faces or TypeEnum.FacesSpecial => TypeIdx % 4,
        _ => -1
    };

    // 在北边的 5 个面上
    public bool IsNorth5 => Row == 0;

    // 在南边的 5 个面上
    public bool IsSouth5 => Row == 3;

    // 属于极地十面
    public bool IsPole10 => IsNorth5 || IsSouth5;

    // 属于赤道十面
    public bool IsEquator10 => !IsPole10;
    public bool IsEquatorWest => Row == 1;
    public bool IsEquatorEast => Row == 2;

    private static int Width => Div * 5;
    public static int Div { get; set; }

    public bool IsValid() => Type != TypeEnum.Invalid;

    public static bool ValidateAxial(int q, int r)
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
            return q == -Div;
        if (r < 0)
            return -q % Div < Div + r;
        if (r > Div)
            return -q % Div > Div - r;
        return true;
    }

    // 距离左边最近的边的 Q 差值
    // （当 Column 4 向左跨越回 Column 0 时，保持返回与普通情况一致性，即：将 Column 0 视作 6 的位置计算）
    private int LeftEdgeDiffQ()
    {
        return Row switch
        {
            0 => Coords.Q + Coords.R + Div + Column * Div,
            1 => Coords.Q + Div + Column * Div,
            2 => Coords.Q + Coords.R + Column * Div,
            3 => Coords.Q + Div + Column * Div,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    // 距离右边最近的边的 Q 差值（向右不存在特殊情况）
    private int RightEdgeDiffQ()
    {
        return Row switch
        {
            0 => -Column * Div - Coords.Q,
            1 => -Column * Div - Coords.Q - Coords.R,
            2 => -Column * Div - Coords.Q,
            3 => -Column * Div - Coords.Q - Coords.R + Div,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    // 左右边最近的边的 Q 差值（特殊情况的处理规则同左边情况）
    private int LeftRightEdgeDiffQ()
    {
        return Row switch
        {
            0 => Coords.R + Div,
            1 => Div - Coords.R,
            2 => Coords.R,
            3 => 2 * Div - Coords.R,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// 获取原始正二十面体的三角形的三个顶点
    /// </summary>
    /// <returns>按照非平行于 XZ 平面的边的单独点第一个，然后两个平行边上的点按顺时针顺序排列，返回三个点的数组</returns>
    private IEnumerable<SphereAxial> TriangleVertices()
    {
        var nextColumn = (Column + 1) % 5;
        return Row switch
        {
            0 =>
            [
                new SphereAxial(0, -Div),
                new SphereAxial(-Column * Div, 0),
                new SphereAxial(-nextColumn * Div, 0)
            ],
            1 =>
            [
                new SphereAxial(-nextColumn * Div, Div),
                new SphereAxial(-nextColumn * Div, 0),
                new SphereAxial(-Column * Div, 0)
            ],
            2 =>
            [
                new SphereAxial(-Column * Div, 0),
                new SphereAxial(-Column * Div, Div),
                new SphereAxial(-nextColumn * Div, Div)
            ],
            3 =>
            [
                new SphereAxial(-Div, 2 * Div),
                new SphereAxial(-nextColumn * Div, Div),
                new SphereAxial(-Column * Div, Div)
            ],
            _ => null
        };
    }

    // 转经纬度
    public LongitudeLatitudeCoords ToLongitudeAndLatitude()
    {
        switch (Type)
        {
            case TypeEnum.PoleVertices:
                return new LongitudeLatitudeCoords(0f, 90f * (TypeIdx == 0 ? 1 : -1));
            case TypeEnum.MidVertices:
                var longitude = TypeIdx / 2 * 72f - TypeIdx % 2 * 36f;
                var latitude = TypeIdx % 2 == 0 ? 29.141262794f : -29.141262794f;
                return new LongitudeLatitudeCoords(longitude, latitude);
            case TypeEnum.Edges:
            case TypeEnum.EdgesSpecial:
            case TypeEnum.Faces:
            case TypeEnum.FacesSpecial:
                var tri = TriangleVertices().ToArray();
                var triCoords = tri.Select(sa => sa.ToLongitudeAndLatitude()).ToArray();
                var horizontalCoords1 = triCoords[0].Slerp(triCoords[1],
                    (float)Mathf.Abs(Coords.R - tri[0].Coords.R) / Div);
                var horizontalCoords2 = triCoords[0].Slerp(triCoords[2],
                    (float)Mathf.Abs(Coords.R - tri[0].Coords.R) / Div);
                return horizontalCoords1.Slerp(horizontalCoords2,
                    (float)(Row % 2 == 1 ? LeftEdgeDiffQ() : RightEdgeDiffQ()) / LeftRightEdgeDiffQ());
            default:
                throw new ArgumentException($"暂不支持的类型：{Type}");
        }
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
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(Coords)
                        : sa.Coords.DistanceTo(Coords + new AxialCoords(Width, 0));
                case 4:
                case 5:
                case 10:
                case 11:
                    // sa 在左边逆斜列的情况
                    var rotLeft = Coords.RotateCounterClockwiseAround(new AxialCoords(-(Column + 1) * Div, 0));
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(rotLeft)
                        : sa.Coords.DistanceTo(rotLeft + new AxialCoords(Width, 0));
                case 8:
                case 9:
                    // sa 在左边隔一列的逆斜列的情况
                    var rotLeft2 = Coords
                        .RotateCounterClockwiseAround(new AxialCoords(-(Column + 1) * Div, 0))
                        .RotateCounterClockwiseAround(new AxialCoords(-(Column + 2) * Div, 0));
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(rotLeft2)
                        : sa.Coords.DistanceTo(rotLeft2 + new AxialCoords(Width, 0));
                case 14:
                case 15:
                    // 14，15 是边界情况，可能看作左边隔一列的逆斜列近，也可能看作右边隔一列的斜列近
                    var rot2Left = Coords
                        .RotateCounterClockwiseAround(new AxialCoords(-(Column + 1) * Div, 0))
                        .RotateCounterClockwiseAround(new AxialCoords(-(Column + 2) * Div, 0));
                    var rot2Right = Coords
                        .RotateClockwiseAround(new AxialCoords(-Column * Div, 0))
                        .RotateClockwiseAround(new AxialCoords(-(Column - 1) * Div, 0));
                    return Mathf.Min(
                        Index < sa.Index
                            ? sa.Coords.DistanceTo(rot2Left)
                            : sa.Coords.DistanceTo(rot2Left + new AxialCoords(Width, 0)),
                        Index < sa.Index
                            ? rot2Right.DistanceTo(sa.Coords + new AxialCoords(Width, 0))
                            : rot2Right.DistanceTo(sa.Coords));
                case 12:
                case 13:
                    // sa 在右边隔一列的斜列的情况
                    var rotRight2 = Coords
                        .RotateClockwiseAround(new AxialCoords(-Column * Div, 0))
                        .RotateClockwiseAround(new AxialCoords(-(Column - 1) * Div, 0));
                    return Index < sa.Index
                        ? rotRight2.DistanceTo(sa.Coords + new AxialCoords(Width, 0))
                        : rotRight2.DistanceTo(sa.Coords);
                case 16:
                case 17:
                case 18:
                case 19:
                    // sa 在右边斜列上的情况
                    var rotRight = Coords
                        .RotateClockwiseAround(new AxialCoords(-Column * Div, 0));
                    return Index < sa.Index
                        ? rotRight.DistanceTo(sa.Coords + new AxialCoords(Width, 0))
                        : rotRight.DistanceTo(sa.Coords);
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
                    var rotLeft = Coords.RotateClockwiseAround(new AxialCoords(-(Column + 1) * Div, Div));
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(rotLeft)
                        : sa.Coords.DistanceTo(rotLeft + new AxialCoords(Width, 0));
                case 7:
                case 8:
                    // sa 在左边隔一列的斜列上的情况
                    var rotLeft2 = Coords
                        .RotateClockwiseAround(new AxialCoords(-(Column + 1) * Div, Div))
                        .RotateClockwiseAround(new AxialCoords(-(Column + 2) * Div, Div));
                    return Index < sa.Index
                        ? sa.Coords.DistanceTo(rotLeft2)
                        : sa.Coords.DistanceTo(rotLeft2 + new AxialCoords(Width, 0));
                case 5:
                case 6:
                    // 5，6 是边界情况，可能看作左边隔一列的逆斜列近，也可能看作右边隔一列的斜列近
                    var rot2Left = Coords
                        .RotateClockwiseAround(new AxialCoords(-(Column + 1) * Div, Div))
                        .RotateClockwiseAround(new AxialCoords(-(Column + 2) * Div, Div));
                    var rot2Right = Coords
                        .RotateCounterClockwiseAround(new AxialCoords(-Column * Div, Div))
                        .RotateCounterClockwiseAround(new AxialCoords(-(Column - 1) * Div, Div));
                    var leftDist = Index < sa.Index
                        ? sa.Coords.DistanceTo(rot2Left)
                        : sa.Coords.DistanceTo(rot2Left + new AxialCoords(Width, 0));
                    var rightDist = Index < sa.Index
                        ? rot2Right.DistanceTo(sa.Coords + new AxialCoords(Width, 0))
                        : rot2Right.DistanceTo(sa.Coords);
                    return Mathf.Min(leftDist, rightDist);
                case 11:
                case 12:
                    // sa 在右边隔一列的逆斜列上的情况
                    var rotRight2 = Coords
                        .RotateCounterClockwiseAround(new AxialCoords(-Column * Div, Div))
                        .RotateCounterClockwiseAround(new AxialCoords(-(Column - 1) * Div, Div));
                    return Index < sa.Index
                        ? rotRight2.DistanceTo(sa.Coords + new AxialCoords(Width, 0))
                        : rotRight2.DistanceTo(sa.Coords);
                case 9:
                case 10:
                case 15:
                case 16:
                    // sa 在右边逆斜列上的情况
                    var rotRight = Coords
                        .RotateCounterClockwiseAround(new AxialCoords(-Column * Div, Div));
                    return Index < sa.Index
                        ? rotRight.DistanceTo(sa.Coords + new AxialCoords(Width, 0))
                        : rotRight.DistanceTo(sa.Coords);
                case 13:
                case 14:
                    // sa 在逆斜列上的情况，直接按平面求距离
                    return Index < sa.Index
                        ? Coords.DistanceTo(sa.Coords + new AxialCoords(Width, 0))
                        : Coords.DistanceTo(sa.Coords);
            }
        }
        else
            return sa.DistanceOnePole(this);

        throw new NotImplementedException(); // 按道理不应该走到这里
    }
}