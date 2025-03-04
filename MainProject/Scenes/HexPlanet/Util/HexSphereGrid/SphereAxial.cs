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
    public int Index => Type switch
    {
        TypeEnum.PoleVertices => TypeIdx == 0 ? 0 : 3,
        TypeEnum.MidVertices => TypeIdx / 2 * 4 + 1 + TypeIdx % 2,
        TypeEnum.Edges or TypeEnum.EdgesSpecial => TypeIdx / 6 * 4 + (TypeIdx % 6 + 1) / 2,
        TypeEnum.Faces or TypeEnum.FacesSpecial => TypeIdx / 4 * 4 + TypeIdx % 4,
        _ => -1
    };

    // 获取列索引，从右到左 0 ~ 4
    public int Column => Type switch
    {
        TypeEnum.PoleVertices => 0,
        TypeEnum.MidVertices => TypeIdx / 2,
        TypeEnum.Edges or TypeEnum.EdgesSpecial => TypeIdx / 6,
        TypeEnum.Faces or TypeEnum.FacesSpecial => TypeIdx / 4,
        _ => -1
    };

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
    public bool North5 =>
        (Type is TypeEnum.Faces or TypeEnum.FacesSpecial && TypeIdx % 4 == 0) ||
        (Type is TypeEnum.Edges or TypeEnum.EdgesSpecial && TypeIdx % 6 == 0) ||
        (Type == TypeEnum.PoleVertices && TypeIdx == 0);

    // 在南边的 5 个面上
    public bool South5 =>
        (Type is TypeEnum.Faces or TypeEnum.FacesSpecial && TypeIdx % 4 == 3) ||
        (Type is TypeEnum.Edges or TypeEnum.EdgesSpecial && TypeIdx % 6 == 5) ||
        (Type == TypeEnum.PoleVertices && TypeIdx == 1);

    public bool Pole10 => North5 || South5;
    public bool Equator10 => !Pole10;

    public bool EquatorWest =>
        (Type is TypeEnum.Edges or TypeEnum.EdgesSpecial && (TypeIdx % 6 == 1 || TypeIdx % 6 == 2))
        || (Type is TypeEnum.Faces or TypeEnum.FacesSpecial && TypeIdx % 4 == 1)
        || (Type is TypeEnum.MidVertices && TypeIdx % 2 == 0);

    public bool EquatorEast =>
        (Type is TypeEnum.Edges or TypeEnum.EdgesSpecial && (TypeIdx % 6 == 3 || TypeIdx % 6 == 4))
        || (Type is TypeEnum.Faces or TypeEnum.FacesSpecial && TypeIdx % 4 == 2)
        || (Type is TypeEnum.MidVertices && TypeIdx % 2 == 1);

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
}